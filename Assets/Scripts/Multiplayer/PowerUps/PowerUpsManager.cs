﻿using MLAPI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

namespace Multiplayer
{
    public class PowerUpsManager : MonoBehaviour
    {

        [Range(0, 1)]
        public float triggerChance = 0.6f;
        public NetworkObject[] classicModePrefabs;
        public NetworkObject accuracyModePrefab;
        public List<NetworkObject> powerUpInstances = new List<NetworkObject>();
        private Action powerUpTrigger;

        public void SetUpPowerUpsTrigger()
        {
            switch ((GameMode)PlayerPrefs.GetInt("GameMode"))
            {
                case GameMode.Classic:
                    powerUpTrigger = TriggerPowerUpClassic;
                    break;
                case GameMode.Accuracy:
                    powerUpTrigger = TriggerPowerUpAccuracy;
                    break;
            }
        }

        public void TriggerPowerUp()
        {
            powerUpTrigger();
        }

        #region Gamemode triggers
        private void TriggerPowerUpClassic()
        {
            if (UnityEngine.Random.Range(0f, 1f) <= triggerChance)
            {
                Vector3 powerUpPosition = new Vector3(UnityEngine.Random.Range(-30f, 30f),
                                                      UnityEngine.Random.Range(-2f, 2f), 0);
                var prefab = classicModePrefabs[UnityEngine.Random.Range(0, classicModePrefabs.Length)];
                var powerUp = Instantiate(prefab, powerUpPosition, prefab.transform.rotation);
                powerUpInstances.Add(powerUp);
                powerUp.Spawn();
            }
        }

        private void TriggerPowerUpAccuracy()
        {
            foreach (NetworkObject _instance in powerUpInstances)
            {
                // if there is an instance of powerUp, we don't create more
                if (_instance != null)
                {
                    return;
                }
            }
            Vector3 powerUpPosition = new Vector3(UnityEngine.Random.Range(-30f, 30f),
                                                  UnityEngine.Random.Range(-2f, 2f), 0);
            var powerUp = Instantiate(accuracyModePrefab, powerUpPosition, accuracyModePrefab.transform.rotation);
            powerUpInstances.Add(powerUp);
            powerUp.Spawn();
        }
        #endregion

        public void ClearPowerUps()
        {
            foreach (NetworkObject powerUp in powerUpInstances)
            {
                if (powerUp != null)
                {
                    powerUp.GetComponent<AbstractPowerUp>().RemoveBuff();
                    Destroy(powerUp);
                }
            }
            powerUpInstances.Clear();
        }
    }
}
