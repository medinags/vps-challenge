using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleAPI : MonoBehaviour
{
    [Header("Post Data")]
    public string urlBasePostData = "https://docs.google.com/forms/d/e/1FAIpQLSehTZyJkHAqa4Iz18p4eKlMM9etmT_7exjZU6oTwup8W1vOpg/formResponse";
    [SerializeField] private string entry1 = "entry.206471073";
    [SerializeField] private string entry2 = "entry.1252642934";
    [SerializeField] private string entry3 = "entry.569341463";
    [SerializeField] private string entry4 = "entry.339142722";
    [SerializeField] private string GoogleSheetName = "Form Responses 1";

    [Header("Get Data")]
    public string googleURL = "https://docs.google.com/spreadsheets/d/1cupHscB9fI22nHw0eYBA6Wn-iEb9wC7b_HJKtAGk5s0/edit?usp=sharing";
    public List<PlayerData> playerDataList = new List<PlayerData>();
    private string AllData;
    public event Action OnPlayerDataLoaded; 

    [ContextMenu("Write")]
    public void TestPostData()
    {
        PostData("Ed", "250", "Fontaine Place Notre Dame");
    }
    public void PostData(string player, string score, string location, string useVPS = "1")
    {
        StartCoroutine(WriteData(player, score, location, useVPS));
    }

    private IEnumerator WriteData(string player,string score, string location, string useVPS = "1")
    {
        WWWForm form = new WWWForm();
        form.AddField(entry1, player);
        form.AddField(entry2, useVPS);
        form.AddField(entry3, score);
        form.AddField(entry4, location);

        using (UnityWebRequest www = UnityWebRequest.Post(urlBasePostData, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Data Sent Successfully");
            }
        }
    }

    [ContextMenu("Leer")]
    public void GetDataInDB()
    {
        StartCoroutine(ReadData());
    }

    private IEnumerator ReadData()
    {
        UnityWebRequest webGoogleSheets = UnityWebRequest.Get(googleURL);
        yield return webGoogleSheets.SendWebRequest();

        if (webGoogleSheets.result == UnityWebRequest.Result.Success)
        {
            AllData = webGoogleSheets.downloadHandler.text;
            string content = ExtractContentFromData(AllData, GoogleSheetName);
            ProcessData(content);
            LogPlayerData();
            OnPlayerDataLoaded?.Invoke();
        }
        else
        {
            Debug.LogError($"Error fetching data: {webGoogleSheets.result}");
            yield break;
        }
        
        webGoogleSheets.Dispose();
    }

    private string ExtractContentFromData(string data, string sheetName)
    {
        int indexStartContent = data.IndexOf(sheetName);
        string contentAndAll = data.Substring(indexStartContent + sheetName.Length);
        int indexFinishContent = contentAndAll.IndexOf("\"");
        string content = contentAndAll.Substring(0, indexFinishContent);
        return content.TrimStart();
    }


    private void ProcessData(string content)
    {
        string[] lines = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');

            PlayerData playerData = new PlayerData
            {
                Timestamp = fields[0],
                Player = fields[1],
                UseVPS = fields[2] == "1",
                Score = int.Parse(fields[3]),
                Location = fields[4]
            };

            playerDataList.Add(playerData);
        }
    }

    private void LogPlayerData()
    {
        foreach (PlayerData pd in playerDataList)
        {
            Debug.Log($"Timestamp: {pd.Timestamp}, Player: {pd.Player}, Score: {pd.Score}, Location: {pd.Location}");
        }
    }
}


[Serializable]
public class PlayerData
{
    public string Timestamp;
    public string Player;
    public bool UseVPS;
    public int Score;
    public string Location;
}
