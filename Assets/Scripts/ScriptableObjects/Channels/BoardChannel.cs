using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/Board channel")]
public class BoardChannel : BaseChannel
{
    public event Action OnBallLaunched;
    public event Action<BoardSide> OnPlatformTouched;
    public event Action<BoardSide> OnBackWallTouched;

    public void RaisePlatformTouchEvent(BoardSide boardSide)
    {
        OnPlatformTouched?.Invoke(boardSide);
    }

    public void RaiseBackWallTouchEvent(BoardSide boardSide)
    {
        OnBackWallTouched?.Invoke(boardSide);
    }

    public void RaiseBallLaunchEvent()
    {
        OnBallLaunched?.Invoke();
    }

    public void ClearSubscriptions()
    {
        OnBallLaunched = null;
        OnPlatformTouched = null;
        OnBackWallTouched = null;
    }
}

public enum BoardSide
{
    Blue,
    Red
}
