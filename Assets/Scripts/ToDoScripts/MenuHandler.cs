using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour {

    void Start() {
        string ipAddress = GetLocalIPv4();
        GameObject.Find("IPAddressText").GetComponent<TextMeshProUGUI>().text = ipAddress;
    }

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

    public void StartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
