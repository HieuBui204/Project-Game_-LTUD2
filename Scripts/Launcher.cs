using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] Transform grenadePrefab;  // Prefab của quả lựu đạn
    [SerializeField] Transform spawnPoint;     // Vị trí phóng lựu đạn
    [SerializeField] float launchForce = 50f;  // Lực phóng
    [SerializeField] Color aimLineColor = Color.green;
    Vector2 velocity, currentMousePos;

    // public void RepareGrenade(Player player)
    // {
    //     // Khi ấn giữ phím G, vẽ đường hướng dẫn
    //     if (player.playerStats.grenadeCount > 0)
    //     {
    //         if (Input.GetKeyDown(KeyCode.G))
    //         {
    //             currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //             Vector2 direction = (currentMousePos - (Vector2)spawnPoint.position).normalized; // Hướng từ spawnPoint đến vị trí chuột
    //             velocity = direction * launchForce; // Tính vận tốc

    //             // Vẽ đường thẳng từ spawnPoint đến vị trí chuột để hiển thị hướng
    //             Debug.DrawLine(spawnPoint.position, (Vector2)spawnPoint.position + direction * 2, aimLineColor);

    //         }

    //         // Khi thả phím G, phóng lựu đạn
    //         if (Input.GetKeyUp(KeyCode.G))
    //         {
    //             LaunchGrenade();
    //             player.playerStats.grenadeCount -= 1;

    //         }
    //     }

    // }

    // Phóng lựu đạn
    public void LaunchGrenade()
    {
        currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (currentMousePos - (Vector2)spawnPoint.position).normalized; // Hướng từ spawnPoint đến vị trí chuột
        Transform grenade = Instantiate(grenadePrefab, spawnPoint.position, Quaternion.identity);  // Khởi tạo lựu đạn
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * launchForce; // Áp dụng vận tốc ban đầu
        }
    }
}