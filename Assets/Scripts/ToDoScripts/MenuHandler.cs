using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour {

    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private GameObject nameNotSetWarning;

    /*void Start() {
        string ipAddress = GetLocalIPv4();
        GameObject.Find("IPAddressText").GetComponent<TextMeshProUGUI>().text = ipAddress;
    }*/

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

    public void SelectClassicMode() {
        PlayerPrefs.SetInt("GameMode", (int)GameController.GameMode.Classic);
    }

    public void SelectAccuracyMode() {
        PlayerPrefs.SetInt("GameMode", (int)GameController.GameMode.Accuracy);
    }
    #endregion

    public void ExitGame() {
        Application.Quit();
    }

    public void Host()
    {
        if (NameIsSet())
        {
            gameObject.SetActive(false);
            connectionManager.Host();
        }
    }

    public void Join()
    {
        if (NameIsSet())
        {
            gameObject.SetActive(false);
            connectionManager.Join();
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
