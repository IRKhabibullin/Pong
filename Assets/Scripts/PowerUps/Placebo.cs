using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placebo : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.name == "Ball") {
            Destroy(gameObject);
        }
    }
}
