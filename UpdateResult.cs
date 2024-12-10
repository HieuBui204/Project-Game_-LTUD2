using TMPro;
using UnityEngine;

public class UpdateResult : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI txtSurWave;
    public TextMeshProUGUI txtTime;
    public TextMeshProUGUI txtKills;
    // public TextMeshProUGUI txtHeadshots;
    public TextMeshProUGUI txtScore;
    private RecordScript recordScript;

    public void UpdateResults(float wave, float time, float kills, float score)
    {
        if (txtSurWave != null)
        {
            txtSurWave.text = "You survived " + wave.ToString() + " waves!!";
        }
        if (txtTime != null)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            txtTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        if (txtKills != null)
        {
            txtKills.text = kills.ToString();
        }
        // if (txtHeadshots != null)
        // {
        //     txtHeadshots.text = "Headshots: " + headshots.ToString();
        // }
        if (txtScore != null)
        {
            txtScore.text = score.ToString();
        }
    }
}
