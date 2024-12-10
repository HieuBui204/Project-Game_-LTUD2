using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class PowerUpEffect : MonoBehaviour
{
    private Weapon weapon;
    private float duration;
    private PowerUpController.PowerUpType powerUpType; 
    private GameObject playerObj;
    private Player player;

    private void Awake()
    {
        playerObj = GameObject.FindWithTag("Player");
        player = playerObj.GetComponent<Player>();
    }

    public void Initialize(Weapon weapon, float duration, PowerUpController.PowerUpType powerUpType)
    {
        this.weapon = weapon;
        this.duration = duration;
        this.powerUpType = powerUpType; // Gán giá trị powerUpType

        // Thực thi hiệu ứng dựa trên loại Power Up
        ApplyEffect();

        StartCoroutine(EffectDuration());
    }

    private void ApplyEffect()
    {
        switch (powerUpType)
        {
            case PowerUpController.PowerUpType.Insta_Kill:
                player.playerStats.damageMultiplier = 999999;
                Debug.Log("Đã áp dụng hiệu ứng Insta-Kill.");
                break;
            case PowerUpController.PowerUpType.Double_Point:
                player.playerStats.pointMultiplier = 2;
                Debug.Log("Đã áp dụng hiệu ứng Insta-Kill.");
                break;
            case PowerUpController.PowerUpType.Nuke:
                DealNukeDamage(); // Gọi hàm deal damage cho tất cả enemy
                Debug.Log("Đã áp dụng hiệu ứng Nuke.");
                break;
            default:
                break;
        }
    }

    private void DealNukeDamage()
    {
        // Tìm tất cả các enemy có tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            // Lấy component Enemy để gọi hàm DamageEnemy
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.DamageEnemy(9999); // Gọi hàm DamageEnemy() với lượng damage là 9999
            }
        }
        Destroy(gameObject); // Hủy GameObject hiệu ứng sau khi hết thời gian

        Debug.Log("Đã áp dụng hiệu ứng Nuke cho tất cả enemy.");
    }

    private IEnumerator EffectDuration()
    {
        Debug.Log("Bắt đầu đếm ngược: " + duration + " giây.");
        yield return new WaitForSeconds(duration);

        // Kết thúc hiệu ứng
        if (powerUpType == PowerUpController.PowerUpType.Insta_Kill)
        {
            player.playerStats.damageMultiplier = 1;
        }
        if (powerUpType == PowerUpController.PowerUpType.Double_Point)
        {
            player.playerStats.pointMultiplier = 1;
        }
        Debug.Log("Thời gian hiệu lực đã kết thúc.");
        Destroy(gameObject); // Hủy GameObject hiệu ứng sau khi hết thời gian
    }
}
