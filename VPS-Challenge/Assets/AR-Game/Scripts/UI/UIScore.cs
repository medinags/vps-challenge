using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] places = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] names = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] scores = new TextMeshProUGUI[3];
    
    public void SetScore(int place, string playerName, int playerScore)
    {
        places[place].text = place.ToString();
        names[place].text = playerName;
        scores[place].text = playerScore.ToString();
    }


}
