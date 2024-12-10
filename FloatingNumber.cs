using UnityEngine;
using TMPro;

public class FloatingNumber : MonoBehaviour
{
    public float moveSpeed = 50f; // Tốc độ di chuyển theo hướng
    public float duration = 1f;  // Thời gian tồn tại
    public Vector3 direction = Vector3.up; // Hướng di chuyển
    public TextMeshProUGUI textMesh; // Text hiển thị số điểm

    private float elapsedTime;
    private void Start()
    {
        // Random góc từ 0 đến 90 độ (3h đến 6h) và chuyển đổi thành hướng
        float randomAngle = Random.Range(0f, 90f); // Trong khoảng 0 đến 90 độ
        float radians = randomAngle * Mathf.Deg2Rad; // Chuyển đổi từ độ sang radian
        direction = new Vector3(Mathf.Cos(radians), -Mathf.Sin(radians), 0f).normalized; // Tạo vector hướng
    }
    void Update()
    {
        // Di chuyển theo hướng đã chọn
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // Tăng thời gian đã trôi qua
        elapsedTime += Time.deltaTime;

        // Giảm alpha dần
        float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
        if (textMesh != null)
        {
            var color = textMesh.color;
            textMesh.color = new Color(color.r, color.g, color.b, alpha);
        }

        // Hủy đối tượng sau khi hết thời gian
        if (elapsedTime >= duration)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
        }
    }
}
