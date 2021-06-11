using UnityEngine;
using MLAPI.Messaging;
using MLAPI;
using System.Collections.Generic;

public class BullsEye : AbstractPowerUp {

    private GameController _gc;
    private Rigidbody ball;
    private LineRenderer aim_line;
    [SerializeField] private int _maxIterations = 3;
    private int _count;
    private List<Vector3> positions = new List<Vector3>();

    public LayerMask aimLayers;
    private LayerMask backWallLayer;

    private Vector3 testVelocity;

    private ClientRpcParams triggeredClientRpcParams;


    protected override void Start() {
        _gc = GameObject.Find("GameManager").GetComponent<GameController>();
        base.Start();

        if (_gc.testMode) {
            float x_axis_velocity = Random.Range(-3 * 25 / 4, 3 * 25 / 4);
            float y_axis_velocity = Mathf.Sqrt(25 * 25 - x_axis_velocity * x_axis_velocity);
            testVelocity = new Vector3(x_axis_velocity, y_axis_velocity, 0);
        }
    }

    protected override void OnTriggerEnter(Collider collider)
    {
        if (!IsServer) return;
        base.OnTriggerEnter(collider);
    }

    public override void ApplyBuff()
    {
        applied = true;
        ball = _gc.ballController.GetComponent<Rigidbody>();
        backWallLayer = LayerMask.NameToLayer("BackWall");
        var lastFenderClientId = _gc.lastFender.GetComponent<NetworkObject>().OwnerClientId;
        triggeredClientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { lastFenderClientId }
            }
        };
        ApplyBuffClientRpc(triggeredClientRpcParams);
    }

    [ClientRpc]
    public void ApplyBuffClientRpc(ClientRpcParams clientRpcParams)
    {
        aim_line = GetComponent<LineRenderer>();
    }

    [ClientRpc]
    public void SetRayPositionsClientRpc(Vector3[] positions, ClientRpcParams clientRpcParams)
    {
        aim_line.positionCount = positions.Length;
        aim_line.SetPositions(positions);
    }

    protected override void Update() {
    	base.Update();
        if (!IsServer) return;
        if (applied) {
            _count = 0;
            /*aim_line.positionCount = 1;
            aim_line.SetPosition(0, ball.position);*/
            positions.Clear();
            positions.Add(ball.position);
            Vector3 velocity = _gc.testMode ? testVelocity : ball.velocity;
            RayCast(ball.position, velocity);
            /*Vector3[] positions = new Vector3[aim_line.positionCount]; aim_line.GetPositions(positions);*/
            /*aim_line.GetPositions(positions);*/
            SetRayPositionsClientRpc(positions.ToArray(), triggeredClientRpcParams);
        }
    }

    private void RayCast(Vector3 pos, Vector3 direction) {
        Physics.Raycast(pos + direction * 0.001f, direction, out RaycastHit hit, Mathf.Infinity, aimLayers);
        if (hit.collider != null && _count <= _maxIterations - 1) {
            _count++;
            Vector3 reflectAngle = Vector3.Reflect(direction, hit.normal);
            /*aim_line.positionCount = _count + 1;
            aim_line.SetPosition(_count, hit.point);*/
            positions.Add(hit.point);
            if (hit.collider.gameObject.layer == backWallLayer) {
                return;
            }
            RayCast(hit.point, reflectAngle);
        }
    }
}
