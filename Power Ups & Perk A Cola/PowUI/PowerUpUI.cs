using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PowerUpUI : MonoBehaviour
{
    [Header("Tham chiếu đến các UI theo tên")]
    public GameObject nukeUI;
    public GameObject instaKillUI;
    public GameObject doublePointUI;
    public GameObject fireSaleUI;
    public GameObject carpenterUI;
    public GameObject maxAmmoUI;

    [Header("Tham chiếu đến các Countdown Text")]
    public Text nukeCountDownText;
    public Text instaKillCountDownText;
    public Text doublePointCountDownText;
    public Text fireSaleCountDownText;
    public Text carpenterCountDownText;
    public Text maxAmmoCountDownText;

    [Header("Tham chiếu đến các Time Bar")]
    public Image nukeTimeBar; // Sử dụng Image với Fill
    public Image instaKillTimeBar;
    public Image doublePointTimeBar;
    public Image fireSaleTimeBar;
    public Image carpenterTimeBar;
    public Image maxAmmoTimeBar;

    // Dictionary để ánh xạ PowerUpType với UI, Text và TimeBar tương ứng
    private Dictionary<PowerUpController.PowerUpType, (GameObject ui, Text countdownText, Image timeBar)> powerUpUIs;

    private void Awake()
    {
        // Khởi tạo Dictionary
        powerUpUIs = new Dictionary<PowerUpController.PowerUpType, (GameObject, Text, Image)>
        {
            { PowerUpController.PowerUpType.Nuke, (nukeUI, nukeCountDownText, nukeTimeBar) },
            { PowerUpController.PowerUpType.Insta_Kill, (instaKillUI, instaKillCountDownText, instaKillTimeBar) },
            { PowerUpController.PowerUpType.Double_Point, (doublePointUI, doublePointCountDownText, doublePointTimeBar) },
            { PowerUpController.PowerUpType.Fire_Sale, (fireSaleUI, fireSaleCountDownText, fireSaleTimeBar) },
            { PowerUpController.PowerUpType.Carpenter, (carpenterUI, carpenterCountDownText, carpenterTimeBar) },
            { PowerUpController.PowerUpType.Max_Ammo, (maxAmmoUI, maxAmmoCountDownText, maxAmmoTimeBar) }
        };

        // Ẩn tất cả UI lúc khởi tạo
        DeactivateAllPowerUpUIs();
    }

    // Hàm để kích hoạt UI và bắt đầu đếm ngược
    public void ActivateUI(float duration, PowerUpController.PowerUpType powerUpType)
    {
        if (powerUpUIs.ContainsKey(powerUpType))
        {
            var (ui, countdownText, timeBar) = powerUpUIs[powerUpType];

            if (ui != null && countdownText != null && timeBar != null)
            {
                ui.SetActive(true);

                // Đặt giá trị ban đầu cho Time Bar (fillAmount từ 1.0 đến 0.0)
                timeBar.fillAmount = 1.0f;

                StartCoroutine(StartCountdown(duration, powerUpType));
            }
            else
            {
                Debug.LogWarning($"UI, Countdown Text hoặc Time Bar cho PowerUpType {powerUpType} chưa được gán!");
            }
        }
        else
        {
            Debug.LogWarning($"PowerUpType {powerUpType} không có trong Dictionary!");
        }
    }

    // Coroutine để hiển thị countdown và cập nhật Time Bar
    private IEnumerator StartCountdown(float duration, PowerUpController.PowerUpType powerUpType)
    {
        float remainingTime = duration;

        var (ui, countdownText, timeBar) = powerUpUIs[powerUpType];

        while (remainingTime > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = remainingTime.ToString("F1") + "s";
            }

            if (timeBar != null)
            {
                // Cập nhật fillAmount theo tỉ lệ thời gian còn lại
                timeBar.fillAmount = remainingTime / duration;
            }

            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        if (countdownText != null)
        {
            countdownText.text = "0s";
        }

        if (timeBar != null)
        {
            timeBar.fillAmount = 0;
        }

        // Ẩn UI của PowerUp sau khi countdown kết thúc
        if (ui != null)
        {
            ui.SetActive(false);
        }
    }

    // Hàm để ẩn tất cả các PowerUp UI
    public void DeactivateAllPowerUpUIs()
    {
        foreach (var powerUpUI in powerUpUIs.Values)
        {
            if (powerUpUI.ui != null)
            {
                powerUpUI.ui.SetActive(false);
            }

            if (powerUpUI.countdownText != null)
            {
                powerUpUI.countdownText.text = "";
            }

            if (powerUpUI.timeBar != null)
            {
                powerUpUI.timeBar.fillAmount = 0;
            }
        }
    }
}
