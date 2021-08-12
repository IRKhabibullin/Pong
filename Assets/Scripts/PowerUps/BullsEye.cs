using UnityEngine;
using MLAPI.Messaging;
using MLAPI;
using System.Collections.Generic;

/// <summary>
/// Buff, showing ball trajectory. Shows reflections from side walls and platforms
/// </summary>
public class BullsEye : AbstractPowerUp {

    [SerializeField] private int _maxIterations = 3;
    [SerializeField] private LayerMask aimLayers;
    [SerializeField] private LayerMask backWallLayer;

    private GameController _gc;
    private Rigidbody ball;
    private LineRenderer aim_line;
    private List<Vector3> positions = new List<Vector3>();

    private ClientRpcParams triggeredClientRpcParams;


    protected override void Start() {
        _gc = GameObject.Find("GameManager").GetComponent<GameController>();
        base.Start();
    }

    public override void ApplyBuff()
    {
        base.ApplyBuff();
        ball = _gc.ballController.GetComponent<Rigidbody>();
        triggeredClientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { _gc.lastTouched.GetComponent<NetworkObject>().OwnerClientId }
            }
        };
        ApplyBuffClientRpc(triggeredClientRpcParams);
    }

    /// <summary>
    /// Called only on client which activated buff
    /// </summary>
    /// <param name="clientRpcParams"></param>
    [ClientRpc]
    public void ApplyBuffClientRpc(ClientRpcParams clientRpcParams)
    {
        aim_line = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Recursive raycasting, reflecting from side walls and platforms
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="direction"></param>
    private void RayCast(Vector3 pos, Vector3 direction)
    {
        Physics.Raycast(pos + direction * 0.001f, direction, out RaycastHit hit, Mathf.Infinity, aimLayers);
        if (hit.collider != null && positions.Count <= _maxIterations - 1)
        {
            positions.Add(hit.point);
            if (hit.collider.gameObject.layer == backWallLayer) return;
            RayCast(hit.point, Vector3.Reflect(direction, hit.normal));
        }
    }

    /// <summary>
    /// Calculate ball trajectory on the server and pass it to client, which activated buff
    /// </summary>
    protected override void Update() {
        if (!IsServer) return;
        if (applied) {
            positions.Clear();
            positions.Add(ball.position);
            RayCast(ball.position, ball.velocity);
            SetRayPositionsClientRpc(positions.ToArray(), triggeredClientRpcParams);
        }
        base.Update();
    }

    [ClientRpc]
    public void SetRayPositionsClientRpc(Vector3[] positions, ClientRpcParams clientRpcParams)
    {
        aim_line.positionCount = positions.Length;
        aim_line.SetPositions(positions);
    }
}
