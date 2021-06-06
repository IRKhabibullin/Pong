using MLAPI;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsManager : MonoBehaviour {

	[Range(0, 1)]
	public float triggerChance;
	public NetworkObject[] powerUpPrefabs;
	public List<NetworkObject> powerUpInstances;

    void Start() {
        powerUpInstances = new List<NetworkObject>();
    }

    public void TriggerPowerUp() {
    	if (Random.Range(0f, 1f) < triggerChance) {
    		Vector3 powerUpPosition = new Vector3(Random.Range(-30f, 30f), Random.Range(-2f, 2f), 0);
            var prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
            var powerUp = Instantiate(prefab, powerUpPosition, prefab.transform.rotation);
            powerUpInstances.Add(powerUp);
            powerUp.Spawn();
        }
    }

    public void clearPowerUps() {
    	foreach (NetworkObject powerUp in powerUpInstances) {
            // todo should remove from list if powerUp destroyed due to life ending or any other case
            if (powerUp != null) {
                powerUp.GetComponent<AbstractPowerUp>().RemoveBuff();
                Destroy(powerUp);
            }
    	}
    }
}
