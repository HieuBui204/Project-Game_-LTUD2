using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public enum PowerUpType
    {
        Nuke,
        Insta_Kill,
        Double_Point,
        Fire_Sale,
        Carpenter,
        Max_Ammo
    }

    [SerializeField]
    public PowerUpType powerUpType; // Loại Power Up

    [Header("Power Up Sounds")]
    public AudioClip powUpSound;

    // Thời gian hiệu lực của PowerUp
    [SerializeField]
    private float Duration = 20f;

    // Tham chiếu đến script PowerUpUI để gọi chức năng hiển thị
    public PowerUpUI powerUpUI;

    // Tham chiếu đến prefab của hiệu ứng PowerUp
    public GameObject effectPrefab;

    private GameObject powerUpUIObj;



    private void Awake()
    {
        powerUpUIObj = GameObject.FindGameObjectWithTag("PowerUpUI");
        if (powerUpUIObj != null)
        {
            powerUpUI = powerUpUIObj.GetComponent<PowerUpUI>();
        }
        else
        {
            Debug.LogWarning("PowerUpUI chưa được gán!");
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã nhặt Power Up.");
            ApplyPowerUp(other.gameObject);
            PlayPowerUpSound();
            powerUpUI.ActivateUI(Duration, powerUpType); // Kích hoạt UI và truyền thời gian countdown cùng loại PowerUp
            gameObject.SetActive(false); // Ẩn Power Up sau khi nhặt
        }
    }

    void ApplyPowerUp(GameObject player)
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();
        if (weapon == null)
        {
            Debug.LogError("Weapon không tồn tại!");
            return;
        }

        switch (powerUpType)
        {
            case PowerUpType.Insta_Kill:
                // Tạo và khởi tạo hiệu ứng Insta-Kill
                GameObject instaKillEffect = Instantiate(effectPrefab, Vector3.zero, Quaternion.identity);
                PowerUpEffect instaKillPowerUp = instaKillEffect.GetComponent<PowerUpEffect>();
                if (instaKillPowerUp != null)
                {
                    instaKillPowerUp.Initialize(weapon, Duration, powerUpType);
                }
                break;
            case PowerUpType.Nuke:
                // Tạo và khởi tạo hiệu ứng Nuke
                GameObject nukeEffect = Instantiate(effectPrefab, Vector3.zero, Quaternion.identity);
                PowerUpEffect nukePowerUp = nukeEffect.GetComponent<PowerUpEffect>();
                if (nukePowerUp != null)
                {
                    nukePowerUp.Initialize(weapon, Duration, powerUpType);
                }
                break;
            case PowerUpType.Double_Point:
                // Tạo và khởi tạo hiệu ứng Double Point
                GameObject doublePointEffect = Instantiate(effectPrefab, Vector3.zero, Quaternion.identity);
                PowerUpEffect doublePointPowerUp = doublePointEffect.GetComponent<PowerUpEffect>();
                if (doublePointPowerUp != null)
                {
                    doublePointPowerUp.Initialize(weapon, Duration, powerUpType);
                }
                break;
            case PowerUpType.Fire_Sale:
                // Tạo và khởi tạo hiệu ứng Fire Sale
                GameObject fireSaleEffect = Instantiate(effectPrefab, Vector3.zero, Quaternion.identity);
                PowerUpEffect fireSalePowerUp = fireSaleEffect.GetComponent<PowerUpEffect>();
                if (fireSalePowerUp != null)
                {
                    fireSalePowerUp.Initialize(weapon, Duration, powerUpType);
                }
                break;
            case PowerUpType.Carpenter:
                // Tạo và khởi tạo hiệu ứng Carpenter
                GameObject carpenterEffect = Instantiate(effectPrefab, Vector3.zero, Quaternion.identity);
                PowerUpEffect carpenterPowerUp = carpenterEffect.GetComponent<PowerUpEffect>();
                if (carpenterPowerUp != null)
                {
                    carpenterPowerUp.Initialize(weapon, Duration, powerUpType);
                }
                break;
            case PowerUpType.Max_Ammo:
                // Tạo và khởi tạo hiệu ứng Max Ammo
                GameObject maxAmmoEffect = Instantiate(effectPrefab, Vector3.zero, Quaternion.identity);
                PowerUpEffect maxAmmoPowerUp = maxAmmoEffect.GetComponent<PowerUpEffect>();
                if (maxAmmoPowerUp != null)
                {
                    maxAmmoPowerUp.Initialize(weapon, Duration, powerUpType);
                }
                break;
            default:
                break;
        }
    }

    void PlayPowerUpSound()
    {
        AudioClip clipToPlay = powUpSound;

        if (clipToPlay != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(clipToPlay);
            Debug.Log("Phát âm thanh: " + clipToPlay.name);
        }
        else
        {
            Debug.LogWarning("AudioClip cho PowerUpType " + powerUpType + " không được gán hoặc AudioManager chưa được thiết lập!");
        }
    }
}
