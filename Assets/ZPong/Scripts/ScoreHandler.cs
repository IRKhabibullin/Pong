using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

/// <summary>
/// Game score handling. Player objects must be tagged as "Player1" and "Player2".
/// </summary>
public class ScoreHandler : NetworkBehaviour {

    private Dictionary<string, int> scores = new Dictionary<string, int>();
    //private NetworkDictionary<string, int> networkScores = new NetworkDictionary<string, int>();
    private Dictionary<string, TextMeshProUGUI> scoreTexts = new Dictionary<string, TextMeshProUGUI>();
    [SerializeField] private int winScore = 10;
    [SerializeField] private GameObject scoreCanvas;
    [SerializeField] private GameObject scoreFrame;

    public void InitScore()
    {
        scores.Clear();
        scores.Add("Player1", 0);
        scores.Add("Player2", 0);
        InitScoreTexts();
    }

    public void InitScoreServer()
    {
        /*networkScores.Clear();
        networkScores.Add("Player1", 0);
        networkScores.Add("Player2", 0);*/
        InitScoreClientRpc();
    }

    /// <summary>
    /// Inits score related UI on a clients
    /// </summary>
    [ClientRpc]
    public void InitScoreClientRpc()
    {
        InitScoreTexts();
        //networkScores.OnDictionaryChanged += OnScoreChanged;
    }

    public void InitScoreTexts()
    {
        scoreCanvas.SetActive(true);
        scoreFrame.SetActive(true);
        scoreTexts.Clear();
        foreach (var player_name in scores.Keys)
        {
            TextMeshProUGUI scoreText = GameObject.Find(player_name + "Score").GetComponent<TextMeshProUGUI>();
            scoreText.text = scores[player_name].ToString();
            scoreTexts.Add(player_name, scoreText);
        }

        /*foreach (var player_name in networkScores.Keys)
        {
            TextMeshProUGUI scoreText = GameObject.Find(player_name + "Score").GetComponent<TextMeshProUGUI>();
            scoreText.text = networkScores[player_name].ToString();
            scoreTexts.Add(player_name, scoreText);
        }*/
    }

    /*private void OnScoreChanged(NetworkDictionaryEvent<string, int> changeEvent)
    {
        RedrawScore();
    }*/
    public void ClearScores()
    {
        scoreCanvas.SetActive(false);
        scoreFrame.SetActive(false);
        foreach (var score in new List<string>(scores.Keys))
        {
            scores[score] = 0;
        }
        /*foreach (var score in new List<string>(networkScores.Keys))
        {
            networkScores[score] = 0;
        }*/
        RedrawScore();
    }

    /// <summary>
    /// Called when score has changed
    /// </summary>
    public void RedrawScore()
    {
        foreach (var player in scores.Keys)
        {
            scoreTexts[player].text = scores[player].ToString();
        }
        /*foreach (var player in networkScores.Keys)
        {
            scoreTexts[player].text = networkScores[player].ToString();
        }*/
    }

    /// <summary>
    /// Increases score of passed player. Called only on the server
    /// </summary>
    public bool MUpdateScore(string player)
    {
        /*networkScores[player]++;
        return networkScores.Values.Contains(winScore);*/
        return true;
    }

    /// <summary>
    /// Increases score of passed player. For singleplayer
    /// </summary>
    public bool SUpdateScore(string player)
    {
        scores[player]++;
        RedrawScore();
        return scores.ContainsValue(winScore);
    }
}
