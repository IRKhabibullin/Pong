using UnityEngine;
using static GameController;

public interface IMatchController
{
    public IPlatformController Pitcher { get; set; } // player who started round
    public GameObject LastTouched { get; set; }  // player last reflected ball
    public GameState GameState { get; set; }

    public void EnterMatch();
    public void ExitMatch();
    public bool MovementAllowed();
    public void StartRound();
    public void BackWallTouchHandler(string playerTag);
    public void PlatformTouchHandler(GameObject platform);
    public void PowerUpTouchHandler();
}

public interface IPlatformController
{
    public int LaunchDirection { get; set; }
    /// <summary>
    /// Initial setting
    /// </summary>
    /// <param name="side">On which side platform is placed</param>
    public void SetUp(int side);
    public void SetColor(int playerNumber);
    public void SetRotation(float rotation);
    public void SetSpeed(float direction);
    public float GetCurrentAngle();

    /// <summary>
    /// Returns where the ball should be placed on a platform at start of the round
    /// </summary>
    public Vector2 GetBallStartPosition();

    /// <summary>
    /// Resets position and rotation of platform
    /// </summary>
    public void ResetPlatform();
}

public interface IBallController
{
    public Rigidbody Rb { get; set; }

    public GameObject gameObject { get; }

    public void MoveBall(Vector3 position);

    public void LaunchBall();

    public void ResetBall();

    public void StopBall();

    public void ChangeMaterial(string material_key);
}