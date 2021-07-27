using System.Collections.Generic;
using Networking;
using UnityEngine;

public class RoomListController : MonoBehaviour
{
    [SerializeField]
    private GameObject gamePanelPrefab;
    [SerializeField]
    private GameObject panelsList;

    [SerializeField] private ConnectionManager connectionManager;

    private readonly Dictionary<long, DiscoveryResponse> rooms = new Dictionary<long, DiscoveryResponse>();
    public List<string> roomsNames = new List<string>();

    private void UpdateRoomsList()
    {
        foreach (Transform item in panelsList.transform)
        {
            Destroy(item.gameObject);
        }
        foreach (DiscoveryResponse room in rooms.Values)
        {
            GameObject gamePanel = Instantiate(gamePanelPrefab, panelsList.transform, false);
            gamePanel.GetComponent<RoomPanelHandler>().SetRoomData(room, connectionManager);
        }
    }

    public void AddRoomData(DiscoveryResponse room)
    {
        Debug.Log("Adding room data");
        if (!rooms.ContainsKey(room.ServerId))
        {
            roomsNames.Add(room.HostName);
        }
        rooms[room.ServerId] = room;
        UpdateRoomsList();
    }

    public void ClearRooms()
    {
        rooms.Clear();
        roomsNames.Clear();
        UpdateRoomsList();
    }
}
