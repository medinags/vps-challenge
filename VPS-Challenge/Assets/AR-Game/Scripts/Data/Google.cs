using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Google : MonoBehaviour
{
    [Header("Post Data")]
    public string urlBasePostData = "https://docs.google.com/forms/d/e/1FAIpQLSefxhjUfEQRmAz8mYCzWQRjK3CQeALtgwJgPpppStHvd5FW7A/formResponse";

    [Header("Entries in Google Sheets")]
    [SerializeField] private string entry1 = "entry.269494217";
    [SerializeField] private string entry2 = "entry.830528914";
    [SerializeField] private string entry3 = "entry.2134469976";
    private string GoogleSheetName = "Form Responses 1";
    [SerializeField] private string data1;
    [SerializeField] private string data2;
    [SerializeField] private string data3;

    [Header("Get Data")]
    public string googleURL;
    public List<PlayerData> playerDataList = new List<PlayerData>();
    private string AllData;

    [ContextMenu("Write")]
    public void PostData(string player, string score, string location)
    {
        StartCoroutine(WriteData(data1, data2, data3));
    }

    private IEnumerator WriteData(string player, string score, string location)
    {
        WWWForm form = new WWWForm();
        form.AddField(entry1, player);
        form.AddField(entry2, score);
        form.AddField(entry3, location);

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
    public void ReadSimple()
    {
        StartCoroutine(ReadSimpleCorutina());
    }

    private IEnumerator ReadSimpleCorutina()
    {
        UnityWebRequest webGoogleSheets = UnityWebRequest.Get(googleURL);
        yield return webGoogleSheets.SendWebRequest();

        if (webGoogleSheets.result == UnityWebRequest.Result.Success)
        {
            string content = ExtractContentFromData(AllData, GoogleSheetName);
            ProcessData(content);
            LogPlayerData();
    
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
                Score = int.Parse(fields[2]),
                Location = fields[3]
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
    public int Score;
    public string Location;
}
