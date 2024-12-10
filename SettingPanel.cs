using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingPanel : MonoBehaviour
{
    // UI Components
    [Header("Buttons")]
    public Button saveSettingButton;
    public Button backToMainButton;

    [Header("Toggles")]
    public Toggle damageIndicatorToggle;
    public GameObject damageIndicatorPrefab;
    public Toggle bloodSplashToggle;
    public GameObject bloodPrefab;

    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;

    [Header("Menu Panel")]
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    private const string BloodSplashKey = "BloodSplash";
    private const string DamageIndicatorKey = "DamageIndicator";
    private const string MasterVolumeKey = "MasterVolume";

    public void Start()
    {
        // Đảm bảo menu ban đầu tắt
        pauseMenuPanel.SetActive(false);

        // Gán sự kiện cho các UI Components
        if (saveSettingButton != null)
            saveSettingButton.onClick.AddListener(SaveSettings);

        if (backToMainButton != null)
            backToMainButton.onClick.AddListener(BackToMain);

        if (bloodSplashToggle != null)
            bloodSplashToggle.onValueChanged.AddListener(SetToggleBloodSplash);

        if (damageIndicatorToggle != null)
            damageIndicatorToggle.onValueChanged.AddListener(SetToggleDamageIndicator);

        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(SetMaxVolume);

        // Tải cài đặt đã lưu
        LoadSettings();
    }

    public void Update()
    {
        // Kiểm tra phím ESC để mở/tắt menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Tạm dừng game và hiển thị menu
    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true); // Hiển thị menu
        Time.timeScale = 0; // Dừng thời gian trong game
    }

    // Tiếp tục game và tắt menu
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false); // Ẩn menu
        Time.timeScale = 1; // Tiếp tục thời gian
    }

    // Điều chỉnh âm lượng tổng (Master Volume)
    public void SetMaxVolume(float volume)
    {
        // Chỉnh giá trị âm lượng cho AudioMixer (sử dụng Logarithmic Volume)
        audioMixer.SetFloat("MasterVolume", volume);
    }

    // Bật/tắt hiệu ứng Blood Splash
    public void SetToggleBloodSplash(bool isToggle)
    {
        if (bloodPrefab != null)
        {
            bloodPrefab.SetActive(isToggle);
        }
    }

    // Bật/tắt Damage Indicator
    public void SetToggleDamageIndicator(bool isToggle)
    {
        if (damageIndicatorPrefab != null)
        {
            damageIndicatorPrefab.SetActive(isToggle);
        }
    }

    // Lưu cài đặt vào PlayerPrefs
    public void SaveSettings()
    {
        PlayerPrefs.SetInt(BloodSplashKey, bloodSplashToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt(DamageIndicatorKey, damageIndicatorToggle.isOn ? 1 : 0);

        if (masterVolumeSlider != null)
            PlayerPrefs.SetFloat(MasterVolumeKey, masterVolumeSlider.value);

        PlayerPrefs.Save();
        Debug.Log("Cài đặt đã được lưu!");
    }

    // Tải cài đặt từ PlayerPrefs
    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey(BloodSplashKey))
        {
            bool isBloodSplashEnabled = PlayerPrefs.GetInt(BloodSplashKey) == 1;
            bloodSplashToggle.isOn = isBloodSplashEnabled;
            SetToggleBloodSplash(isBloodSplashEnabled);
        }

        if (PlayerPrefs.HasKey(DamageIndicatorKey))
        {
            bool isDamageIndicatorEnabled = PlayerPrefs.GetInt(DamageIndicatorKey) == 1;
            damageIndicatorToggle.isOn = isDamageIndicatorEnabled;
            SetToggleDamageIndicator(isDamageIndicatorEnabled);
        }

        if (PlayerPrefs.HasKey(MasterVolumeKey) && masterVolumeSlider != null)
        {
            float volume = PlayerPrefs.GetFloat(MasterVolumeKey);
            masterVolumeSlider.value = volume;
            SetMaxVolume(volume);
        }
    }

    // Trở về menu chính (nếu cần xử lý logic chuyển scene)
    public void BackToMain()
    {
        ResumeGame();
        Debug.Log("Trở về menu chính!"); // Thay bằng logic chuyển scene nếu cần
    }
}
