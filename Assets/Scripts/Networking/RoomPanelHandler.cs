using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class RoomPanelHandler : MonoBehaviour
    {
        /*private DiscoveryResponse room;*/
        private GameController _gc;
        /*public void SetRoomData(DiscoveryResponse response, GameController _gc)
        {
            room = response;
            this._gc = _gc;
            transform.Find("IPAddress").GetComponent<TextMeshProUGUI>().text = room.HostName;
            transform.Find("PlayersCount").GetComponent<TextMeshProUGUI>().text = room.PlayersInRoom.ToString();

            transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(ConnectToMatch);
        }

        private void ConnectToMatch()
        {
            PongNetworkManager.singleton.StartClient(room.RoomUri);
            _gc.EnterGame();
        }*/
    }
}
