using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class codeMenu : MonoBehaviour
{
    [Header("ALL BUTTON (GOb)")]
    public GameObject back;
    public GameObject play;
    public GameObject inv;
    public GameObject set;
    public GameObject info;
    public GameObject quit;

    [Header("ALL PANEL")]
    //Play Panel
    public GameObject playPanel;
    public GameObject playPanel_Endless;
    public GameObject playPanel_Create;
    public GameObject recordPanel;


    //Inventory Panel
    public GameObject invPanel;
    //Setting Panel
    public GameObject settingPanel;
    //Infomation Panel
    public GameObject infoPanel;

    [Header("COUNTDOWN PANEL")]
    public GameObject countdownPanel;      // Panel chứa các thành phần UI
    public TextMeshProUGUI countdownText;  // Text để hiển thị số đếm ngược
    public Button cancelButton;            // Button để hủy đếm ngược
    public Button openPanelButton;         // Button chính để mở panel

    private Coroutine countdownCoroutine;  // Lưu coroutine đếm ngược

    private void Start()
    {
        // Đảm bảo panel ban đầu ẩn đi
        countdownPanel.SetActive(false);

        // Gắn sự kiện cho button
        openPanelButton.onClick.AddListener(OpenPanel);
        cancelButton.onClick.AddListener(CancelCountdown);
    }

    // Hàm để mở panel và bắt đầu đếm ngược
    public void OpenPanel()
    {
        countdownPanel.SetActive(true);  // Hiển thị panel
        countdownCoroutine = StartCoroutine(CountdownCoroutine()); // Bắt đầu đếm ngược
    }

    // Hàm để tắt panel và huỷ đếm ngược nếu đang chạy
    public void CancelCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
        countdownPanel.SetActive(false);  // Ẩn panel
    }

    // Coroutine đếm ngược từ 3 xuống 0
    private IEnumerator CountdownCoroutine()
    {
        int countdown = 3;

        while (countdown >= 0)
        {
            countdownText.text = "Starting in ... " + countdown.ToString();
            yield return new WaitForSeconds(1);
            countdown--;
        }

        // Chuyển scene khi kết thúc đếm ngược
        LoadNextScene();
    }

    // Hàm để chuyển sang scene khác
    private void LoadNextScene()
    {
        if (mapSlider.currentMapIndex == 0)
        {
            // Chuyển đến scene có tên "NextScene"
            SceneManager.LoadScene("Map 1");
        }
        if (mapSlider.currentMapIndex == 1)
        {
            // Chuyển đến scene có tên "NextScene"
            SceneManager.LoadScene("Map 2");
        }

    }

    // Hàm xử lý khi nút Quit được nhấn
    public void OnQuitButtonClick()
    {
        Debug.Log("Application quitting...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnClickPlayBtn()
    {
        playPanel.SetActive(true);

        invPanel.SetActive(false);
        settingPanel.SetActive(false);
        infoPanel.SetActive(false);
    }
    public void OnClickPlay_Endless_Btn()
    {
        playPanel_Endless.SetActive(true);

        playPanel.SetActive(false);
        invPanel.SetActive(false);
        settingPanel.SetActive(false);
        infoPanel.SetActive(false);

        back.SetActive(true);
        play.SetActive(false);
        inv.SetActive(false);
        set.SetActive(false);
        info.SetActive(false);
        quit.SetActive(false);
    }

    public void OnClickInvBtn()
    {
        invPanel.SetActive(true);

        playPanel.SetActive(false);
        settingPanel.SetActive(false);
        infoPanel.SetActive(false);
    }
    public void OnClickSettBtn()
    {
        settingPanel.SetActive(true);

        playPanel.SetActive(false);
        invPanel.SetActive(false);
        infoPanel.SetActive(false);
    }
    public void OnClickInfoBtn()
    {
        infoPanel.SetActive(true);

        playPanel.SetActive(false);
        invPanel.SetActive(false);
        settingPanel.SetActive(false);
    }

    public void OnClickBackBtn()
    {
        playPanel.SetActive(true);
        playPanel_Endless.SetActive(false);

        back.SetActive(false);
        play.SetActive(true);
        inv.SetActive(true);
        set.SetActive(true);
        info.SetActive(true);
        quit.SetActive(true);
    }

    public void OnClickEndless_Solo()
    {
        playPanel_Endless.SetActive(false);
        playPanel_Create.SetActive(true);
    }

    public void OnClickRecordBtn()
    {
        recordPanel.SetActive(true);

        playPanel.SetActive(false);

    }
}
