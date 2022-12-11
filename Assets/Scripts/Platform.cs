using System;
using Unity.Netcode;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Vector3 CurrentMoveSpeed { get; private set; }
    public Vector3 CurrentRotationSpeed { get; private set; }

    [ServerRpc]
    public void UpdateCurrentMoveSpeedServerRpc(Vector3 newMoveSpeed)
    {
        CurrentMoveSpeed = newMoveSpeed;
    }

    private void Update()
    {   
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (!IsFacingSideWall())
        {
            transform.position += CurrentMoveSpeed;
        }
    }

    private void ResetPosition()
    {
        var position = transform.position;
        position = new Vector3(position.x * 0, position.y * 1, position.z * 1);
        transform.position = position;
    }
    
    private bool IsFacingSideWall()
    {
        return Physics.Raycast(
            transform.position,
            new Vector3(Mathf.Sign(CurrentMoveSpeed.x), 0, 0), 
            transform.localScale.x / 2.0f,
            GlobalSettings.Settings.SideWallLayer);
    }

    private void OnEnable()
    {
        EventsManager.RoundChannel.OnRoundFinished += ResetPosition;
    }

    private void OnDisable()
    {
        if (!EventsManager.HasInstance)
            return;
        
        EventsManager.RoundChannel.OnRoundFinished -= ResetPosition;
    }
}
