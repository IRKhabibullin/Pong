using Unity.Netcode;
using UnityEngine;

public class KeyboardInput : NetworkBehaviour
{
    private PlatformOld platformOld;

    private void Start()
    {
        if (!IsLocalPlayer)
            enabled = false;

        platformOld = GetComponent<PlatformOld>();
    }

    private void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        var moveDirection = Input.GetAxisRaw("Horizontal");
        platformOld.SetMovementSpeedServerRpc((MovementDirection)moveDirection);

        var rotateDirection = Input.GetAxisRaw("Vertical");
        platformOld.SetRotationSpeedServerRpc((MovementDirection)rotateDirection);
    }
}
