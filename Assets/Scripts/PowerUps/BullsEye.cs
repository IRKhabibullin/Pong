using UnityEngine;
using MLAPI.Messaging;
using MLAPI;

public class BullsEye : AbstractPowerUp {

    private GameController _gc;
    private Rigidbody ball;
    private LineRenderer aim_line;
    [SerializeField] private int _maxIterations = 3;
    private int _count;

    public LayerMask aimLayers;
    private LayerMask backWallLayer;
    private LayerMask platformLayer;

    private Vector3 testVelocity;
    private bool thisPlayerBuffed;


    protected override void Start() {
        _gc = GameObject.Find("GameManager").GetComponent<GameController>();
        thisPlayerBuffed = false;
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
        var lastFenderClientId = _gc.lastFender.GetComponent<NetworkObject>().OwnerClientId;
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { lastFenderClientId }
            }
        };
        ApplyBuffClientRpc(clientRpcParams);
    }

    [ClientRpc]
    public void ApplyBuffClientRpc(ClientRpcParams clientRpcParams)
    {
        applied = true;
        thisPlayerBuffed = true;
        ball = _gc.ballController.GetComponent<Rigidbody>();
        aim_line = GetComponent<LineRenderer>();
        backWallLayer = LayerMask.NameToLayer("BackWall");
        platformLayer = LayerMask.NameToLayer("Platform");
    }

    protected override void Update() {
        if (!thisPlayerBuffed) return;
    	base.Update();
        if (applied) {
            _count = 0;
            aim_line.positionCount = 1;
            aim_line.SetPosition(0, ball.position);
            Vector3 velocity = _gc.testMode ? testVelocity : ball.velocity;
            RayCast(ball.position, velocity);
        }
    }

    private void RayCast(Vector3 pos, Vector3 direction) {
        Physics.Raycast(pos + direction * 0.001f, direction, out RaycastHit hit, Mathf.Infinity, aimLayers);
        if (hit.collider != null && _count <= _maxIterations - 1) {
            _count++;
            Vector3 reflectAngle = Vector3.Reflect(direction, hit.normal);
            aim_line.positionCount = _count + 1;
            aim_line.SetPosition(_count, hit.point);
            if (hit.collider.gameObject.layer == backWallLayer) {
                return;
            }
            RayCast(hit.point, reflectAngle);
        }
    }
}
