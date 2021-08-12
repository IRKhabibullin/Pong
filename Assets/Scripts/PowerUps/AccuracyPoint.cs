using UnityEngine;

public class AccuracyPoint : AbstractPowerUp
{
    protected override void OnTriggerEnter(Collider collider)
    {
        if (!IsServer) return;
        if (collider.gameObject.CompareTag("Ball"))
        {
            GameObject.Find("GameManager").GetComponent<GameController>().PowerUpTouchHandler();
            Destroy(gameObject);
        }
    }
}
