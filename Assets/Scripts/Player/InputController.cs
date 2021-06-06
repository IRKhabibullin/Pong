using UnityEngine;
using static GameController;

public class InputController : MonoBehaviour
{
    private float leftWallPosition;
    private float rightWallPosition;
    private float lastAngle;
    private float startAngle;
    private float angleBetweenTouches;
    private Camera mainCamera;
    private GameController gameController;

    [SerializeField] private PlatformController platform;

    void Start()
    {
        mainCamera = Camera.main;
        leftWallPosition = GameObject.Find("LeftSideWall").transform.position.x;
        rightWallPosition = GameObject.Find("RightSideWall").transform.position.x;
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
    }

    void Update()
    {
        if (gameController.testMode)
            CheckForDebugTouch();
        if (platform.IsLocalPlayer)
        {
            CheckForKeyboard();
            CheckForTouch();
        }
    }


    /// <summary>
    /// Check for touches in debug mode. Allows to place ball wherever on a board
    /// </summary>
    private void CheckForDebugTouch()
    {
        if (Input.touchCount != 1)
            return;
        var touch = Input.GetTouch(0);
        if (Camera.main is null)
            return;
        var touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        if (!(touchPosition.x < rightWallPosition) || !(touchPosition.x > leftWallPosition))
            return;
        touchPosition.z = 0f;
        if (gameController.ballController != null)
            gameController.ballController.MoveBall(touchPosition);
    }
    void CheckForTouch()
    {
        if (Input.touchCount == 1)
        {
            // we're moving
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = mainCamera.ScreenToWorldPoint(touch.position);
            if (Mathf.Abs(touchPosition.x) < rightWallPosition)
            {
                return;
            }
            if (touchPosition.x < leftWallPosition)
            {
                platform.MoveServerRpc(-1f);
            }
            if (touchPosition.x > rightWallPosition)
            {
                platform.MoveServerRpc(1f);
            }
        }
        else if (Input.touchCount == 2)
        {
            // we're rotating
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);
            Vector2 firstTouchPosition = mainCamera.ScreenToWorldPoint(firstTouch.position);
            Vector2 secondTouchPosition = mainCamera.ScreenToWorldPoint(secondTouch.position);
            if (Mathf.Abs(firstTouchPosition.x) < rightWallPosition || Mathf.Abs(secondTouchPosition.x) < rightWallPosition)
            {
                return;
            }
            angleBetweenTouches = CalcCurrentAngle(firstTouchPosition, secondTouchPosition);
            // todo maybe chech that player could make both touches on one side
            if (firstTouch.phase.Equals(TouchPhase.Began) || secondTouch.phase.Equals(TouchPhase.Began))
            {
                lastAngle = platform.GetCurrentAngle();
                startAngle = angleBetweenTouches;
            }
            else if (firstTouch.phase.Equals(TouchPhase.Moved) || secondTouch.phase.Equals(TouchPhase.Moved))
            {
                platform.Rotate(lastAngle + angleBetweenTouches - startAngle);
            }
        }
    }

    void CheckForKeyboard()
    {
        if (gameController.gameState.Value == GameStates.Prepare)
        {
            return;
        }
        var direction = Input.GetAxis("Horizontal");
        if (direction != 0.0 || platform.mSpeed != Vector3.zero)
        {
            platform.MoveServerRpc(direction);
        }
    }

    float CalcCurrentAngle(Vector2 firstPoint, Vector2 secondPoint)
    {
        Vector2 direction = firstPoint.x < secondPoint.x ? secondPoint - firstPoint : firstPoint - secondPoint;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
