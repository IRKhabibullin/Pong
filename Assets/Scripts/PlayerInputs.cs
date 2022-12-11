using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private InputAction horizontalAxis;
    private Platform platform;

    private void UpdateCurrentMoveSpeed()
    {
        platform.UpdateCurrentMoveSpeedServerRpc(new Vector3(
            horizontalAxis.ReadValue<Vector2>().x * GlobalSettings.Platform.MaxMoveSpeed * Time.deltaTime,
            0,
            0
        ));
    }

    private void Start()
    {
        horizontalAxis = GetComponent<PlayerInput>().actions[GlobalSettings.Settings.HorizontalAxis];
        platform = GetComponent<Platform>();
    }

    private void Update()
    {
        UpdateCurrentMoveSpeed();
    }
}
