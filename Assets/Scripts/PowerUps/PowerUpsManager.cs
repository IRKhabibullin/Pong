using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsManager : MonoBehaviour {

	[Range(0, 1)]
	public float triggerChance;
	public GameObject[] powerUpPrefabs;
	private List<GameObject> powerUpInstances;

    void Start() {
        powerUpInstances = new List<GameObject>();
    }

    public void TriggerPowerUp() {
    	if (Random.Range(0f, 1f) < triggerChance) {
    		Vector2 powerUpPosition = new Vector2(Random.Range(-30f, 30f), Random.Range(-2f, 2f));
    		powerUpInstances.Add(Instantiate(powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)], powerUpPosition, Quaternion.identity));
    	}
    }

    public void clearPowerUps() {
    	foreach (GameObject powerUp in powerUpInstances) {
            // todo should remove from list if powerUp destroyed due to life ending or any other case
            if (powerUp != null) {
                powerUp.GetComponent<AbstractPowerUp>().RemoveBuff();
                Destroy(powerUp);
            }
    	}
    }
}
