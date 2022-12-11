using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public enum MovementDirection
{
    None = 0,
    Left = -1,
    Right = 1
}

/// <summary>
/// Component which updates transform values of object. Works only on the server. Clients are synced via NetworkTransform component
/// </summary>
[RequireComponent(typeof(NetworkTransform))]
public class MovementController : NetworkBehaviour
{
    [SerializeField] private LayerMask sideWallLayer;
    private PlatformOld platformOld;  // todo better to use interface

    private void Awake()
    {
        platformOld = GetComponent<PlatformOld>();
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        if (!IsFacingSideWall())
            transform.position += platformOld.mMoveSpeed;
    }

    private bool IsFacingSideWall()
    {
        return Physics.Raycast(transform.position, new Vector3(Mathf.Sign(platformOld.mMoveSpeed.x), 0, 0), transform.localScale.x / 2.0f, LayerMask.GetMask("SideWall"));
    }

    private void Rotate()
    {
        if (!IsAtMaxAngle())
        {
            transform.Rotate(platformOld.mRotationSpeed);
        }
    }

    private float GetCurrentAngle()
    {
        return ((transform.localRotation.eulerAngles.z + 180f) % 360f) - 180f;
    }

    private bool IsAtMaxAngle()
    {
        float newAngle = GetCurrentAngle() + platformOld.mRotationSpeed.z;
        return Math.Abs(newAngle) > platformOld.maxAngle;
    }
}
