using Multiplayer;
using TMPro;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public enum MenuPage
    {
        MainMenu,      // Starting page
        LobbyPanel,    // Panel to host or find a game
        Settings,      // Settings panel
    }

    private MenuPage currentPage;
    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private GameObject nameNotSetWarning;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject welcomePanel;

    public void Start()
    {
        playerName.text = PlayerPrefs.GetString("PlayerName");
        currentPage = MenuPage.MainMenu;
        // Check if user opens an app for the first time. If he is, then we show welcome panel
        if (CheckFirstEnter())
        {
            welcomePanel.SetActive(true);
            PlayerPrefs.SetInt("FirstEnter", 1);
        }
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
                    case MenuPage.Settings:
                        settingsPanel.SetActive(false);
                        mainMenu.SetActive(true);
                        currentPage = MenuPage.MainMenu;
                        break;
                }
            }
        }
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

    public void ToSettings()
    {
        mainMenu.SetActive(false);
        settingsPanel.SetActive(true);
        currentPage = MenuPage.Settings;
    }

    public void ToLobby()
    {
        lobbyPanel.SetActive(true);
        mainMenu.SetActive(false);
        currentPage = MenuPage.LobbyPanel;
    }

    public bool CheckFirstEnter()
    {
        return PlayerPrefs.GetInt("FirstEnter") != 1;
    }
}
