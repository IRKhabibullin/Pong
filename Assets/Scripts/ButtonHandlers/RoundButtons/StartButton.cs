public class StartButton : AbstractButtonHandler
{
    protected override void RaiseOnClickEvent()
    {
        EventsManager.RoundChannel.RaiseStartButtonPressedEvent();
    }
}
