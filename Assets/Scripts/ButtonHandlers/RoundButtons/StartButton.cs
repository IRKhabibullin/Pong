public class StartButton : AbstractButtonHandler
{
    protected override void RaiseOnClickEvent()
    {
        EventsManager.Instance.RoundChannel.RaiseStartButtonPressedEvent();
    }
}
