using TMPro;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public enum MenuPage
    {
        MainMenu,    // Starting page
        LobbyPanel,  // Panel to host or find a game
    }

    private MenuPage currentPage;
    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private GameObject nameNotSetWarning;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject lobbyPanel;

    public void Start()
    {
        playerName.text = PlayerPrefs.GetString("PlayerName");
        currentPage = MenuPage.MainMenu;
    }

    public void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                switch (currentPage)
                {
                    case MenuPage.LobbyPanel:
                        lobbyPanel.SetActive(false);
                        mainMenu.SetActive(true);
                        currentPage = MenuPage.MainMenu;
                        break;
                }
            }
        }
    }

    public void SelectGameMode(int modeIndex)
    {
        PlayerPrefs.SetInt("GameMode", modeIndex);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void Host()
    {
        if (NameIsSet())
            connectionManager.Host();
    }

    public void Find()
    {
        if (NameIsSet())
            connectionManager.Find();
    }

    private bool NameIsSet()
    {
        if (playerName.text == "​")
        {
            nameNotSetWarning.SetActive(true);
            return false;
        }
        PlayerPrefs.SetString("PlayerName", playerName.text);
        return true;
    }

    public void ToLobby()
    {
        lobbyPanel.SetActive(true);
        mainMenu.SetActive(false);
        currentPage = MenuPage.LobbyPanel;
    }
}
