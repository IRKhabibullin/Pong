using System.Collections.Generic;
using Networking;
using UnityEngine;

public class RoomListController : MonoBehaviour
{
    [SerializeField]
    private GameObject gamePanelPrefab;
    [SerializeField]
    private GameObject panelsList;

    private GameController _gc;

    /*private readonly Dictionary<long, DiscoveryResponse> rooms = new Dictionary<long, DiscoveryResponse>();*/
    public List<string> roomsNames = new List<string>();

    private void Awake()
    {
        _gc = GameObject.Find("GameManager").GetComponent<GameController>();
    }

    private void UpdateRoomsList()
    {
        foreach (Transform item in panelsList.transform)
        {
            Destroy(item.gameObject);
        }
        /*foreach (DiscoveryResponse room in rooms.Values)
        {
            GameObject gamePanel = Instantiate(gamePanelPrefab, panelsList.transform, false);
            gamePanel.GetComponent<RoomPanelHandler>().SetRoomData(room, _gc);
        }*/
    }

    /*public void AddRoomData(DiscoveryResponse room)
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
    }*/
}
