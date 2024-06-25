using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class DataHandler : MonoBehaviour
{
    private GoogleAPI googleAPI;
    private List<string> fruitPlayerNames = new List<string>
    {
        "Apple", "Banana", "Cherry", "Date", "Elderberry",
        "Fig", "Grape", "Honeydew", "Kiwi", "Lemon"
    };
    void Start()
    {
        googleAPI = GetComponent<GoogleAPI>();
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

}
