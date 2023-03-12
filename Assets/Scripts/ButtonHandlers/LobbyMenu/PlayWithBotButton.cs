public class PlayWithBotButton : AbstractButtonHandler
{
    protected override void RaiseOnClickEvent()
    {
        EventsManager.Instance.LobbyChannel.RaisePlayWithBotButtonPressedEvent();
    }
}
