using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public TextMeshProUGUI timeText; // Thời gian sống
    public TextMeshProUGUI killText; // Số kill
    public TextMeshProUGUI scoreText; // Điểm
    CodeScripts codeScripts;
    void Start()
    {
        codeScripts = new CodeScripts();

    }
    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void SaveNewRecord()
    {
        if (LoginScript.userID != null)
        {
            Debug.Log($"Logged-in UserID: {LoginScript.userID}");
            string ss = LoginScript.userID;
            var profile = codeScripts.GetProfileByUserID(ss);
            if (profile != null)
            {
                // Chuyển đổi dữ liệu từ UI thành giá trị int
                int.TryParse(timeText.text, out int time);
                int.TryParse(killText.text, out int kill);
                int.TryParse(scoreText.text, out int score);

                // Lưu record
                codeScripts.SaveNewRecord(profile.ID, time, score, kill);
                Debug.Log("Record saved successfully!");
            }
            else
            {
                Debug.LogError("Profile not found for the given UserID.");
            }
        }
        else
        {
            Debug.LogError("UserID is null. Make sure the user is logged in.");
        }
    }

}
