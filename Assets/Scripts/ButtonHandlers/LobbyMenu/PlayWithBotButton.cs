public class PlayWithBotButton : AbstractButtonHandler
{
    protected override void RaiseOnClickEvent()
    {
        EventsManager.LobbyChannel.RaisePlayWithBotButtonPressedEvent();
    }
}
