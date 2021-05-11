﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreHandler : MonoBehaviour {

    private Dictionary<string, int> scores = new Dictionary<string, int>();
    private Dictionary<string, TextMeshProUGUI> scoreTexts = new Dictionary<string, TextMeshProUGUI>();

    void Start() {
    	/*foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
    		scores.Add(player.name, 0);
    		TextMeshProUGUI scoreText = GameObject.Find(player.name + "Score").GetComponent<TextMeshProUGUI>();
    		scoreText.text = scores[player.name].ToString();
    		scoreTexts.Add(player.name, scoreText);
		}*/
    }

    public void InitScore(string[] player_names)
    {
        scores.Clear();
        scoreTexts.Clear();
        foreach (var player_name in player_names)
        {
            scores.Add(player_name, 0);
            TextMeshProUGUI scoreText = GameObject.Find(player_name + "Score").GetComponent<TextMeshProUGUI>();
            scoreText.text = scores[player_name].ToString();
            scoreTexts.Add(player_name, scoreText);
        }
    }

    public void UpdateScore(string player)
    {
        Debug.Log($"UpdateScore {player}");
        scores[player]++;
    	scoreTexts[player].text = scores[player].ToString();
    }
}
