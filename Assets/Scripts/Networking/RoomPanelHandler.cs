using MLAPI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class RoomPanelHandler : MonoBehaviour
    {
        private DiscoveryResponse room;
        private ConnectionManager _cm;

        public void SetRoomData(DiscoveryResponse response, ConnectionManager connectionManager)
        {
            _cm = connectionManager;
            room = response;
            transform.Find("IPAddress").GetComponent<TextMeshProUGUI>().text = room.HostName;
            /*transform.Find("PlayersCount").GetComponent<TextMeshProUGUI>().text = room.PlayersInRoom.ToString();*/

            transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(ConnectToMatch);
        }

        private void ConnectToMatch()
        {
            _cm.Join(room.RoomUri);
        }

        private PongManager pnm;

        private PongManager pongManager
        {
            get
            {
                if (pnm != null) { return pnm; }
                return pnm = NetworkManager.Singleton as PongManager;
            }
        }
    }
}
