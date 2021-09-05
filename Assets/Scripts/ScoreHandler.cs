using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable.Collections;

/// <summary>
/// Game score handling. Player objects must be tagged as "Player1" and "Player2".
/// </summary>
public class ScoreHandler : NetworkBehaviour {

    private NetworkDictionary<string, int> scores = new NetworkDictionary<string, int>();
    private Dictionary<string, TextMeshProUGUI> scoreTexts = new Dictionary<string, TextMeshProUGUI>();
    [SerializeField] private int winScore;

    public void InitScore(bool playWithBot)
    {
        scores.Clear();
        scores.Add("Player1", 0);
        scores.Add("Player2", 0);
        if (playWithBot)
            InitScoreTexts();
        else
            InitScoreClientRpc();
    }

    /// <summary>
    /// Inits score related UI on a clients
    /// </summary>
    [ClientRpc]
    public void InitScoreClientRpc()
    {
        InitScoreTexts();
        scores.OnDictionaryChanged += OnScoreChanged;
    }

    public void InitScoreTexts()
    {
        scoreTexts.Clear();
        foreach (var player_name in scores.Keys)
        {
            TextMeshProUGUI scoreText = GameObject.Find(player_name + "Score").GetComponent<TextMeshProUGUI>();
            scoreText.text = scores[player_name].ToString();
            scoreTexts.Add(player_name, scoreText);
        }
    }

    private void OnScoreChanged(NetworkDictionaryEvent<string, int> changeEvent)
    {
        RedrawScore();
    }
    public void ClearScores()
    {
        foreach (var score in new List<string>(scores.Keys))
        {
            scores[score] = 0;
        }
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
    }

    /// <summary>
    /// Increases score of passed player. Called only on the server
    /// </summary>
    public bool UpdateScore(string player)
    {
        scores[player]++;
        return scores.Values.Contains(winScore);
    }
}
