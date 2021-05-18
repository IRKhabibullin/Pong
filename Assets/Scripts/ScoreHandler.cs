using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;

public class ScoreHandler : NetworkBehaviour {

    private NetworkDictionary<string, int> scores = new NetworkDictionary<string, int>();

    private Dictionary<string, TextMeshProUGUI> scoreTexts = new Dictionary<string, TextMeshProUGUI>();
    public int winScore;

    public void InitScore(string[] players_names)
    {
        scores.Clear();
        foreach (var player_name in players_names)
        {
            scores.Add(player_name, 0);
        }
        InitsCoreClientRpc(players_names);
    }

    [ClientRpc]
    public void InitsCoreClientRpc(string[] players_names)
    {
        scoreTexts.Clear();
        foreach (var player_name in players_names)
        {
            TextMeshProUGUI scoreText = GameObject.Find(player_name + "Score").GetComponent<TextMeshProUGUI>();
            scoreText.text = scores[player_name].ToString();
            scoreTexts.Add(player_name, scoreText);
        }

        scores.OnDictionaryChanged += OnDictChanged;
    }

    private void OnDictChanged(NetworkDictionaryEvent<string, int> changeEvent)
    {
        RedrawScore();
    }

    public void RedrawScore()
    {
        foreach (var player in scores.Keys)
        {
            scoreTexts[player].text = scores[player].ToString();
        }
    }

    public bool UpdateScore(string player)
    {
        scores[player]++;
        return scores.Values.Contains(winScore);
    }
}
