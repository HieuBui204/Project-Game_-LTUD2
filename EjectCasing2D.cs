using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EjectCasing2D : MonoBehaviour
{
    public GameObject[] casingPrefabs;       // Prefab của vỏ đạn
    public Transform ejectPoint;          // Vị trí xuất hiện của vỏ đạn
    public float ejectForce = 2f;         // Lực đẩy vỏ đạn ra ngoài
    public float ejectTorque = 1f;       // Lực xoay của vỏ đạn
    public float casingLifetime = 5f;     // Thời gian tồn tại của vỏ đạn

    public void Eject()
    {
        Debug.Log("Shellllll");
        if (casingPrefabs.Length > 0 && ejectPoint != null)
        {
            // Chọn ngẫu nhiên một prefab từ danh sách
            GameObject selectedCasing = casingPrefabs[Random.Range(0, casingPrefabs.Length)];
            // Tạo vỏ đạn tại vị trí ejectPoint
            GameObject casing = Instantiate(selectedCasing, ejectPoint.position, ejectPoint.rotation);

            // Thêm lực đẩy và xoay cho vỏ đạn
            Rigidbody2D rb = casing.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 ejectDirection = ejectPoint.right; // Lực đẩy theo hướng bên phải của ejectPoint
                rb.AddForce(ejectDirection * ejectForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-ejectTorque, ejectTorque), ForceMode2D.Impulse);
            }

            // Hủy vỏ đạn sau thời gian casingLifetime
            Destroy(casing, casingLifetime);
        }
    }
}

