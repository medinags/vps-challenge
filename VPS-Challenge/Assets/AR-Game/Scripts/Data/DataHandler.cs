using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class DataHandler : MonoBehaviour
{
    [SerializeField] private VPSCoverageController coverageController;
    private GoogleAPI googleAPI;
   
    private List<string> fruitPlayerNames = new List<string>
    {
        "Apple", "Apricot", "Cherry", "Coconut", "Orange",
        "Fig", "Grape", "Mango", "Kiwi", "Lemon"
    };
    [SerializeField] private string debugLocation = "Fontaine Place Notre Dame";
    [SerializeField] private List<PlayerData> bestPlayers = new List<PlayerData>();
    private void Awake()
    {
        googleAPI = GetComponent<GoogleAPI>();
    }
    void Start()
    {
        //GameManager.Instance.OnGameStart += ProcessGameData;
        GameManager.Instance.OnGameStart += StartGame;
        //googleAPI.OnPlayerDataLoaded += ProcessGameData;
        googleAPI.GetDataInDB();
        coverageController.OnButtonsCreated += ButtonsCreated;
    }

    private void ButtonsCreated()
    {
        List<PlayerData> playersData = googleAPI.playerDataList;
        List<PlayerData> players = playersData.Where(p => p.UseVPS).ToList();

        foreach (var locationItem in coverageController.CoverageItems)
        {
            string location = locationItem.TitleLabelText;

            var three = GetBestThreePlayer(players, location, true);

            if (three.Count > 0)
            {
                PlayerData playerData = three[0];
                locationItem.QualityText = "Leader: " + playerData.Player + " - " + playerData.Score;
            }
            else
            {
                locationItem.QualityText = "Be the First!";
            }
        }
    }

    private void StartGame()
    {
        ProcessGamPlayerData();
    }
    private void ProcessGamPlayerData()
    {
        List<PlayerData> playersData = googleAPI.playerDataList;
        List<PlayerData> players = new List<PlayerData>();
      
        if (GameManager.Instance.UseVPS)
        {
            players = playersData.Where(p => p.UseVPS).ToList();
            //Debug.Log(VPSStateController.Instance.CurrentVPSLocationName);
            bestPlayers = GetBestThreePlayer(players, VPSStateController.Instance.CurrentVPSLocationName, true);
            var VPSPlayer = bestPlayers.ToArray();
            SetDataInUI(VPSPlayer);
        }
        else
        {
            players = playersData.Where(p => !p.UseVPS).ToList();
            bestPlayers = GetBestThreePlayer(players, "GPS");
            var NoVPSPlayer = bestPlayers.ToArray();
            SetDataInUI(NoVPSPlayer);
        }
    }

   
    private void SetDataInUI(PlayerData[] players)
    {
        int max = Math.Min(players.Length, 3);
        for (int i = 0; i < max; i++)
        {
            UIManager.Instance.UIScore.SetScore(i, players[i].Player, players[i].Score);
            //Debug.Log($"Player: {players[i].Player}, Score: {players[i].Score}");
        }
    }

    private List<PlayerData> GetBestThreePlayer(List<PlayerData> players, string currentVPSLocation, bool useVPS = false)
    {
        List<PlayerData> bestPlayers = new List<PlayerData>();

        if (!players.Any())
        {
            bestPlayers.Add(googleAPI.playerDataList[0]);
            return bestPlayers;
        }

        if (!useVPS)
        {
            //GPS
            players.Sort((x, y) => y.Score.CompareTo(x.Score));
 
            return players;
        }
        else
        {
            //VPS Debug; "Fontaine Place Notre Dame"
      
            var playersInLocation = players.Where(p => p.Location == currentVPSLocation).ToList();

            if (playersInLocation.Any())
            {
                playersInLocation.Sort((x, y) => y.Score.CompareTo(x.Score)); //OKAY
            }
            else
            {
                playersInLocation = new List<PlayerData>();
            }

            return playersInLocation;
        }
    }
    public void SavePlayerData()
    {
        string player = CheckAndReplacePlayerName(UIManager.Instance.PlayerName);

        string score = GamePlayManager.Instance.Score.ToString();
        string location = string.IsNullOrWhiteSpace(VPSStateController.Instance.CurrentVPSLocationName) ? "GPS" : 
            VPSStateController.Instance.CurrentVPSLocationName;
        string useVPS = GameManager.Instance.UseVPS ? "1" : "0";

        googleAPI.PostData(player, score, location, useVPS);
        string text = "Congratulations! You ranked: " + GetPlayerPosition(GamePlayManager.Instance.Score) + " on the leaderboard!";
        UIManager.Instance.SetPositionWithScore(text); 
    }

    public int GetPlayerPosition(int newScore)
    {
        for (int i = 0; i < bestPlayers.Count; i++)
        {
            if (newScore > bestPlayers[i].Score)
            {
                return i+1;
            }
        }
        return bestPlayers.Count;
    }

    public string CheckAndReplacePlayerName(string playerName)
    {
        string pattern = @"[^a-zA-Z0-9]";
        Regex regex = new Regex(pattern);

        if (string.IsNullOrWhiteSpace(playerName) || regex.IsMatch(playerName))
        {
            System.Random random = new System.Random();
            string randomFruitName = fruitPlayerNames[random.Next(fruitPlayerNames.Count)];

            return randomFruitName;
        }

        return playerName;
    }

}
