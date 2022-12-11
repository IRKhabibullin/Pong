using Unity.Netcode;
using UnityEngine;

public class PlatformOld : NetworkBehaviour
{
    [SerializeField] private PlatformParams platformParams;     // default platform params
    [SerializeField] private PlayerParams playerParams;

    private float maxSpeed;

    public float maxAngle { get; private set; }                 // max angle platform can rotate 
    public Vector3 mMoveSpeed { get; private set; }             // current speed including fixedDeltaTime
    public Vector3 mRotationSpeed { get; private set; }         // current rotation speed including fixedDeltaTime

    private void Awake()
    {
        maxSpeed = platformParams.maxSpeed;
        maxAngle = platformParams.maxAngle;
        SetPlatformWidthByMultiplier(1);
    }

    public void SetMaxSpeedByMultiplier(float multiplier)
    {
        maxSpeed = platformParams.maxSpeed * multiplier;
    }
    public void SetPlatformWidthByMultiplier(float multiplier)
    {
        transform.localScale = new Vector3(platformParams.width * multiplier, transform.localScale.y, transform.localScale.z);
    }

    #region client input handlers
    [ServerRpc]
    public void SetMovementSpeedServerRpc(MovementDirection direction)
    {
        mMoveSpeed = Vector3.right * maxSpeed * (int)direction * Time.fixedDeltaTime;
    }

    [ServerRpc]
    public void SetRotationSpeedServerRpc(MovementDirection direction)
    {
        mRotationSpeed = Vector3.forward * maxSpeed * (int)direction * Time.fixedDeltaTime;
    }
    #endregion

    //public string Name;
    //public float difficultySpeedRatio = 1f;
    // isLeader

    // Vector3 position (depends on player side)
    // int LaunchDirection (depends on player side)
}
