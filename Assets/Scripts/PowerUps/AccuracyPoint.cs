using UnityEngine;

public class AccuracyPoint : AbstractPowerUp
{
    protected override void OnTriggerEnter(Collider collider)
    {
        if (!IsServer) return;
        if (collider.gameObject.CompareTag("Ball"))
        {
            var player_name = GameObject.Find("GameManager").GetComponent<GameController>().lastFender.tag;
            GameObject.Find("GameManager").GetComponent<GameController>().PowerUpTouchHandler(player_name);
            Destroy(gameObject);
        }
    }
}
