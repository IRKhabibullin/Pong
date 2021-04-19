using UnityEngine;
using TMPro;

public class BullsEye : AbstractPowerUp {

    private GameController _gc;
    private Rigidbody2D ball;
    private LineRenderer aim_line;
    [SerializeField] private int _maxIterations = 3;
    private int _count;

    public LayerMask aimLayers;
    private LayerMask backWallLayer;
    private LayerMask platformLayer;

    private Vector2 testVelocity;


    protected override void Start() {
        _gc = GameObject.Find("GameManager").GetComponent<GameController>();
        ball = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        aim_line = GetComponent<LineRenderer>();
        backWallLayer = LayerMask.NameToLayer("BackWall");
        platformLayer = LayerMask.NameToLayer("Platform");
        base.Start();

        if (_gc.testMode) {
            float x_axis_velocity = Random.Range(-3 * 25 / 4, 3 * 25 / 4);
            float y_axis_velocity = Mathf.Sqrt(25 * 25 - x_axis_velocity * x_axis_velocity);
            testVelocity = new Vector2(x_axis_velocity, y_axis_velocity);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider) {
        target = _gc.lastFender;
        // GameObject.Find("CurrentAngle").GetComponent<TextMeshProUGUI>().text = $"{target}";
        base.OnTriggerEnter2D(collider);
    }

    // Update is called once per frame
    protected override void Update() {
    	base.Update();
        if (applied) {
            _count = 0;
            aim_line.positionCount = 1;
            aim_line.SetPosition(0, ball.position);
            Vector2 velocity = _gc.testMode ? testVelocity : ball.velocity;
            RayCast(ball.position, velocity);
        }
    }

    private void RayCast(Vector2 pos, Vector2 direction) {
        RaycastHit2D hit = Physics2D.Raycast(pos + direction * 0.001f, direction, Mathf.Infinity, aimLayers);
        if (hit.collider != null && _count <= _maxIterations - 1) {
            _count++;
            Vector2 reflectAngle = Vector2.Reflect(direction, hit.normal);
            aim_line.positionCount = _count + 1;
            aim_line.SetPosition(_count, hit.point);
            if (hit.collider.gameObject.layer == backWallLayer) {
                return;
            }
            if (hit.collider.gameObject.layer == platformLayer && hit.collider.gameObject != target) {
                return;
            }
            RayCast(hit.point, reflectAngle);
        }
    }
}
