using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class recordPre : MonoBehaviour
{
    public TextMeshProUGUI idText;          // Hiển thị ID
    // public TextMeshProUGUI profileIdText;   // Hiển thị Profile ID
    public TextMeshProUGUI timeSurviveText; // Hiển thị thời gian sống sót
    public TextMeshProUGUI killsText;       // Hiển thị số kills
    public TextMeshProUGUI scoresText;      // Hiển thị điểm số
    public TextMeshProUGUI dateText;        // Hiển thị ngày

    public void SetData(string id, string profileId, int timeSurvive, int kills, int scores, DateTime date)
    {
        idText.text = id;
        // profileIdText.text = profileId;
        timeSurviveText.text = timeSurvive.ToString(); // Hiển thị 2 chữ số thập phân
        killsText.text = kills.ToString();
        scoresText.text = scores.ToString();
        dateText.text = date.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
