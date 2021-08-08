using MLAPI;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

public class PowerUpsManager : MonoBehaviour {

	[Range(0, 1)]
	public float triggerChance;
	public NetworkObject[] classicModePrefabs;
	public NetworkObject[] accuracyModePrefabs;
	public List<NetworkObject> powerUpInstances;
    private GameMode gameMode;

    void Start() {
        powerUpInstances = new List<NetworkObject>();
    }

    public void SetUpPowerUps()
    {
        gameMode = (GameMode)PlayerPrefs.GetInt("GameMode");
    }

    public void TriggerPowerUp() {
        if (gameMode == GameMode.Classic)
        {
            if (Random.Range(0f, 1f) <= triggerChance) {
    		    Vector3 powerUpPosition = new Vector3(Random.Range(-30f, 30f), Random.Range(-2f, 2f), 0);
                var prefab = classicModePrefabs[Random.Range(0, classicModePrefabs.Length)];
                var powerUp = Instantiate(prefab, powerUpPosition, prefab.transform.rotation);
                powerUpInstances.Add(powerUp);
                powerUp.Spawn();
            }
        }
        else if (gameMode == GameMode.Accuracy)
        {
            foreach (NetworkObject _instance in powerUpInstances)
            {
                // if there is an instance of powerUp, we don't create more
                if (_instance != null)
                {
                    return;
                }
            }
            Vector3 powerUpPosition = new Vector3(Random.Range(-30f, 30f), Random.Range(-2f, 2f), 0);
            var prefab = accuracyModePrefabs[0];
            var powerUp = Instantiate(prefab, powerUpPosition, prefab.transform.rotation);
            powerUpInstances.Add(powerUp);
            powerUp.Spawn();
        }
    }

    public void ClearPowerUps() {
    	foreach (NetworkObject powerUp in powerUpInstances) {
            if (powerUp != null) {
                powerUp.GetComponent<AbstractPowerUp>().RemoveBuff();
                Destroy(powerUp);
            }
    	}
    }
}
