using System.Collections;
using UnityEngine;

public class PerkEffect : MonoBehaviour
{
    private Player player; // Tham chiếu đến người chơi
    private PerkController.PerkAColaType perkType; // Loại Perk-A-Cola
    private GameObject playerObj;
    private bool isApplied = false; // Đảm bảo hiệu ứng chỉ áp dụng một lần

    private void Awake()
    {
        // Tìm đối tượng người chơi và tham chiếu đến Player script
        playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<Player>();
        }
    }

    // Khởi tạo Perk với loại Perk và thiết lập hiệu ứng
    public void Initialize(PerkController.PerkAColaType perkType)
    {
        this.perkType = perkType;

        // Kiểm tra người chơi có hợp lệ hay không
        if (player == null)
        {
            Debug.LogError("Không tìm thấy đối tượng Player hoặc script Player!");
            return;
        }

        ApplyEffect(); // Áp dụng hiệu ứng perk
    }

    // Áp dụng hiệu ứng dựa trên loại Perk
    private void ApplyEffect()
    {
        if (isApplied) return; // Đảm bảo hiệu ứng không bị áp dụng nhiều lần
        isApplied = true;

        switch (perkType)
        {
            case PerkController.PerkAColaType.Juggernog:
                player.playerStats.maxHealth += 50; // Tăng máu của người chơi
                player.playerStats.curHealth += 50; // Thêm máu hiện tại
                Debug.Log("Đã áp dụng Juggernog: Tăng máu.");
                break;

            case PerkController.PerkAColaType.SpeedCola:
                player.playerStats.speedMultiplier = 1.2f; // Tăng tốc độ di chuyển
                Debug.Log("Đã áp dụng SpeedCola: Tăng tốc độ.");
                break;

            case PerkController.PerkAColaType.QuickRevive:
                player.playerStats.lifeCount++; // Kích hoạt hiệu ứng hồi sinh
                Debug.Log("Đã áp dụng QuickRevive: Kích hoạt hồi sinh.");
                break;

            case PerkController.PerkAColaType.StaminUp:
                player.playerStats.maxStamina += 25; // Tăng thể lực
                Debug.Log("Đã áp dụng StaminUp: Tăng thể lực.");
                break;

            default:
                Debug.LogWarning("Loại Perk-A-Cola không hợp lệ.");
                break;
        }
    }

    // Hàm gỡ bỏ Perk (nếu cần, ví dụ khi người chơi mất perk)
    public void RemoveEffect()
    {
        if (!isApplied) return; // Nếu hiệu ứng chưa được áp dụng thì không cần gỡ

        switch (perkType)
        {
            case PerkController.PerkAColaType.Juggernog:
                player.playerStats.maxHealth -= 50; // Giảm lại lượng máu đã tăng
                if (player.playerStats.curHealth > player.playerStats.maxHealth)
                {
                    player.playerStats.curHealth = player.playerStats.maxHealth; // Đảm bảo máu không vượt quá giới hạn
                }
                Debug.Log("Đã gỡ bỏ Juggernog: Giảm máu.");
                break;

            case PerkController.PerkAColaType.SpeedCola:
                player.playerStats.speedMultiplier = 1; // Trả tốc độ về bình thường
                Debug.Log("Đã gỡ bỏ SpeedCola: Giảm tốc độ.");
                break;

            // case PerkController.PerkAColaType.QuickRevive:
            //     player.SetQuickRevive(false); // Gỡ bỏ khả năng hồi sinh
            //     Debug.Log("Đã gỡ bỏ QuickRevive: Hủy hồi sinh.");
            //     break;

            default:
                Debug.LogWarning("Loại Perk-A-Cola không hợp lệ để gỡ bỏ.");
                break;
        }

        Destroy(gameObject); // Xóa gameObject chứa hiệu ứng sau khi gỡ bỏ
    }
}
