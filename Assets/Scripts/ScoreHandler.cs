using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable.Collections;

public class ScoreHandler : NetworkBehaviour {

    private NetworkDictionary<string, int> scores = new NetworkDictionary<string, int>();
    private Dictionary<string, TextMeshProUGUI> scoreTexts = new Dictionary<string, TextMeshProUGUI>();
    [SerializeField] private int winScore;

    /// <summary>
    /// Inits synced score values on the server. Doesn't preserve players, if there were any
    /// </summary>
    public void InitScore(string[] players_names)
    {
        scores.Clear();
        foreach (var player_name in players_names)
        {
            scores.Add(player_name, 0);
        }
        InitScoreClientRpc(players_names);
    }

    /// <summary>
    /// Inits score related UI on a clients
    /// </summary>
    [ClientRpc]
    public void InitScoreClientRpc(string[] players_names)
    {
        scoreTexts.Clear();
        foreach (var player_name in players_names)
        {
            TextMeshProUGUI scoreText = GameObject.Find(player_name + "Score").GetComponent<TextMeshProUGUI>();
            scoreText.text = scores[player_name].ToString();
            scoreTexts.Add(player_name, scoreText);
        }

        scores.OnDictionaryChanged += OnScoreChanged;
    }

    /// <summary>
    /// Cleares scores, but preserves same players
    /// </summary>
    public void ClearScores()
    {
        foreach (var score in new List<string>(scores.Keys))
        {
            scores[score] = 0;
        }
    }

    private void OnScoreChanged(NetworkDictionaryEvent<string, int> changeEvent)
    {
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
