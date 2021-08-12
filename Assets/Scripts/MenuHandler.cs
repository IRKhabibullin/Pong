using TMPro;
using UnityEngine;

public class MenuHandler : MonoBehaviour {

    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private GameObject nameNotSetWarning;

    public void Start()
    {
        playerName.text = PlayerPrefs.GetString("PlayerName");
        GameObject.Find("GameModeDropdown").GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt("GameMode");
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
}
