using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.Rendering;
using System.Reflection;

public class DataHandler : MonoBehaviour
{
    private GoogleAPI googleAPI;
    private List<string> fruitPlayerNames = new List<string>
    {
        "Apple", "Banana", "Cherry", "Date", "Elderberry",
        "Fig", "Grape", "Honeydew", "Kiwi", "Lemon"
    };

    private void Awake()
    {
        googleAPI = GetComponent<GoogleAPI>();
    }
    void Start()
    {
        //GameManager.Instance.OnGameStart += ProcessGameData;
        GameManager.Instance.OnLocationFound += OnLocationFound;
        //googleAPI.OnPlayerDataLoaded += ProcessGameData;
        googleAPI.GetDataInDB();

    }

    private void OnLocationFound()
    {
        if (GameManager.Instance.UseVPS)
        {
            string location = VPSStateController.Instance.CurrentVPSLocationName;
            ProcessGameData(location);
        }
    }

    public void SavePlayerData()
    {
        string player = CheckAndReplacePlayerName(UIManager.Instance.PlayerName);
        string score = GamePlayManager.Instance.Score.ToString();
        string location = VPSStateController.Instance.CurrentVPSLocationName;
        googleAPI.PostData(player, score, location);
    }

    public string CheckAndReplacePlayerName(string playerName)
    {
        string pattern = @"[^a-zA-Z0-9]";
        Regex regex = new Regex(pattern);

        if (regex.IsMatch(playerName))
        {
            System.Random random = new System.Random();
            string randomFruitName = fruitPlayerNames[random.Next(fruitPlayerNames.Count)];

            return randomFruitName;
        }


        return playerName;
    }
    [SerializeField] private string debugLocation = "Fontaine Place Notre Dame";
    private void ProcessGameData(string location)
    {
        List<PlayerData> playerData = googleAPI.playerDataList;
        var vpsPlayers = playerData.Where(p => p.UseVPS).ToList();
        var gpsPlayers = playerData.Where(p => !p.UseVPS).ToList();

        if (vpsPlayers.Any())
        {

            var players = GetBestThreePlayerVPS(vpsPlayers, location).ToArray();
            for ( int i = 0; i < players.Length; i++)
            {
                UIManager.Instance.UIScore.SetScore(i, players[i].Player, players[i].Score);
            }
            foreach (var player in players)
            {
                Debug.Log($"Player: {player.Player}, Score: {player.Score}");

            }
        }
        if (gpsPlayers.Any())
        {

        }
    }

    private List<PlayerData> GetBestThreePlayerVPS(List<PlayerData> players, string location)
    {
        List<PlayerData> playersInLocation = players.Where(p => p.Location == location).ToList();
        if (playersInLocation.Any())
        {
            playersInLocation.Sort((x, y) => y.Score.CompareTo(x.Score));
            return playersInLocation;
        }
        playersInLocation = new List<PlayerData> { players[0] };
        return playersInLocation;
    }
}
