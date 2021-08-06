using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;

public class MenuHandler : MonoBehaviour {

    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private GameObject nameNotSetWarning;

    #region game modes
    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();
    }

    public void SelectSinglePlayer() {
  		PlayerPrefs.SetInt("PlayerMode", (int)GameController.PlayerMode.Singleplayer);
    }

    public void SelectMultiPlayer() {
  		PlayerPrefs.SetInt("PlayerMode", (int)GameController.PlayerMode.Multiplayer);
    }

    public void SelectGameMode(int modeIndex)
    {
        PlayerPrefs.SetInt("GameMode", modeIndex);
    }
    #endregion

    public void ExitGame() {
        Application.Quit();
    }

    public void Host()
    {
        if (NameIsSet())
        {
            connectionManager.Host(playerName.text);
        }
    }

    public void Find()
    {
        if (NameIsSet())
        {
            connectionManager.Find(playerName.text);
        }
    }

    private bool NameIsSet()
    {
        if (playerName.text == "​")
        {
            nameNotSetWarning.SetActive(true);
            return false;
        }
        return true;
    }
}
