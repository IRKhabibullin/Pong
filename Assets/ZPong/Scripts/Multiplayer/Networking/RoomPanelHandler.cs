using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer
{
    public class RoomPanelHandler : MonoBehaviour
    {
        // private DiscoveryResponse room;
        private ConnectionManager _cm;

        [SerializeField] private Sprite[] gameModeIcons;

        // public void SetRoomData(DiscoveryResponse response, ConnectionManager connectionManager)
        // {
        //     _cm = connectionManager;
        //     room = response;
        //     transform.Find("HostName").GetComponent<TextMeshProUGUI>().text = room.HostName;
        //     transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(ConnectToMatch);
        //     transform.Find("GameModeIcon").GetComponent<Image>().sprite = gameModeIcons[room.gameMode];
        // }
        //
        // private void ConnectToMatch()
        // {
        //     _cm.Join(room.RoomUri);
        // }
    }
}
