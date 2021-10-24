using System;
using UnityEngine;
using static GameController;

namespace Singleplayer
{
    /// <summary>
    /// Script to track input on a current client
    /// </summary>
    public class InputController : MonoBehaviour
    {
        private float leftWallPosition;
        private float rightWallPosition;
        private float startAngle;
        private float angleBetweenTouches;
        private Camera mainCamera;
        private GameController gameController;
        private IMatchController matchController;

        [SerializeField] private PlatformController platform;

        void Start()
        {
            mainCamera = Camera.main;
            leftWallPosition = GameObject.Find("LeftSideWall").transform.position.x;
            rightWallPosition = GameObject.Find("RightSideWall").transform.position.x;
            gameController = GameObject.Find("GameManager").GetComponent<GameController>();
            matchController = GameObject.Find("GameManager").GetComponent<IMatchController>();

            platform = GetComponent<PlatformController>();
            gameController.movementSlider.onValueChanged.AddListener(OnMovementSliderChanged);
        }

        void Update()
        {
            if (gameController.controlsType == "alternative") return;
            if (gameController.debugMode)
                CheckForDebugTouch();
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                CheckForKeyboard();
            }
            CheckForTouch();
        }

        public void OnMovementSliderChanged(float newValue)
        {
            platform.SetSpeed(newValue);
        }


        /// <summary>
        /// Check for touches in debug mode. Allows to place ball wherever on a board
        /// </summary>
        private void CheckForDebugTouch()
        {
            if (Camera.main is null || Input.touchCount != 1) return;
            var touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            if (touchPosition.x <= leftWallPosition || touchPosition.x >= rightWallPosition) return;
            touchPosition.z = 0f;
            if (gameController.ballController != null)
                gameController.ballController.MoveBall(touchPosition);
        }

        /// <summary>
        /// Reading input from mobile app
        /// </summary>
        void CheckForTouch()
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                // we're moving
                Vector3 touchPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, -mainCamera.transform.position.z));
                if (Mathf.Abs(touchPosition.x) < rightWallPosition)
                {
                    return;
                }
                if (touch.phase.Equals(TouchPhase.Ended))
                {
                    platform.SetSpeed(0);
                    return;
                }
                platform.SetSpeed(touchPosition.x < leftWallPosition ? -1 : 1);
            }
            else if (Input.touchCount == 2)
            {
                // we're rotating
                Touch firstTouch = Input.GetTouch(0);
                Touch secondTouch = Input.GetTouch(1);
                Vector2 firstTouchPosition = mainCamera.ScreenToWorldPoint(new Vector3(firstTouch.position.x, firstTouch.position.y, -mainCamera.transform.position.z));
                Vector2 secondTouchPosition = mainCamera.ScreenToWorldPoint(new Vector3(secondTouch.position.x, secondTouch.position.y, -mainCamera.transform.position.z));
                if (Mathf.Abs(firstTouchPosition.x) < rightWallPosition || Mathf.Abs(secondTouchPosition.x) < rightWallPosition) return;

                // if both touches on one side
                if ((firstTouchPosition.x < leftWallPosition && secondTouchPosition.x < leftWallPosition) ||
                    (firstTouchPosition.x > rightWallPosition && secondTouchPosition.x > rightWallPosition)) return;

                angleBetweenTouches = CalcCurrentAngle(firstTouchPosition, secondTouchPosition);
                // if player just put a finger on any side, we dont rotate, otherwise platform can immediately "jump" for a big angle
                if (firstTouch.phase.Equals(TouchPhase.Began) || secondTouch.phase.Equals(TouchPhase.Began))
                {
                    startAngle = angleBetweenTouches;
                }
                // if player already has two fingers put on a screen and he is moving any of them now, we rotate for angle diff from previous frame
                else if (firstTouch.phase.Equals(TouchPhase.Moved) || secondTouch.phase.Equals(TouchPhase.Moved))
                {
                    platform.SetRotation(angleBetweenTouches - startAngle);
                }

                // Can't move while rotating
                platform.SetSpeed(0);
            }
        }

        /// <summary>
        /// Reading input from desktop
        /// </summary>
        void CheckForKeyboard()
        {
            if (matchController.GameState == GameState.Prepare && !gameController.debugMode)
            {
                return;
            }
            var direction = Input.GetAxis("Horizontal");
            if (direction != 0.0 || platform.mSpeed != Vector3.zero)
            {
                platform.SetSpeed(direction);
            }
            var angle = Input.GetAxis("Vertical");
            if (angle != 0.0 || platform.mAngle != 0)
            {
                var newAngle = platform.GetCurrentAngle() + angle;
                newAngle = newAngle > 0 ? Math.Min(newAngle, 45f) : Math.Max(newAngle, -45f);
                platform.SetRotation(newAngle);
            }
        }

        float CalcCurrentAngle(Vector2 firstPoint, Vector2 secondPoint)
        {
            Vector2 direction = firstPoint.x < secondPoint.x ? secondPoint - firstPoint : firstPoint - secondPoint;
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        void OnDrawGizmos()
        {
            if (Input.touchCount == 2)
            {
                Vector3 firstTouch = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Vector3 secondTouch = Camera.main.ScreenToWorldPoint(Input.GetTouch(1).position);
                firstTouch.z = 0;
                secondTouch.z = 0;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(firstTouch, secondTouch);
            }
        }
    }
}