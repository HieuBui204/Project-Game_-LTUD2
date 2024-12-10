using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public class PlayerStats
    {
        public float maxHealth = 100;
        public float maxStamina = 10;
        public float movementSpeed = 1;
        public float maxSpeed = 20f; // Giới hạn tốc độ di chuyển
        public float crouchSpeed = 0.4f;
        public float jumpForce = 21;
        public float gravity = 3f;
        public float speedMultiplier = 1;
        public float damageMultiplier = 1;
        public float pointMultiplier = 1;
        public int grenadeCount = 3;
        public int lifeCount = 0;
        public string isHolding;
        private float _curStamina;
        public float curStamina
        {
            get { return _curStamina; }
            set { _curStamina = value; }
        }

        private float _point;
        public float point
        {
            get { return _point; }
            set { _point = value; }
        }

        private float _curHealth;
        public float curHealth
        {
            get { return _curHealth; }
            set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        public void Init()
        {
            curHealth = maxHealth;
            curStamina = maxStamina;

        }
    }

    public PlayerStats playerStats = new PlayerStats();
    private GunPose gunPose;

    [Header("Optional: ")]
    [SerializeField]
    private PlayerStatusIndicator playerIndicator;

    private Vector3 _groundPosition;
    public Vector3 GroundPosition
    {
        get { return _groundPosition; }
        set { _groundPosition = value; }
    }

    // Danh sách các Perk mà người chơi đã sở hữu
    private List<PerkType> acquiredPerks = new List<PerkType>();

    // Enum cho các loại Perk
    public enum PerkType
    {
        Juggernog,
        SpeedCola,
        QuickRevive,
        StaminUp
    }

    // List tượng trưng cho Inventory
    private List<string> inventory = new List<string>();

    void Awake()
    {
        // Làm trống inventory để đảm bảo không có item từ lần chơi trước
        inventory.Clear();

        gunPose = this.GetComponent<GunPose>();
        GameObject playerStatsUI = GameObject.FindWithTag("PlayerStatsUI");
        if (playerStatsUI != null)
        {
            playerIndicator = playerStatsUI.GetComponent<PlayerStatusIndicator>();
        }

        playerStats.Init();

        if (playerIndicator != null)
        {
            playerIndicator.SetHealth(playerStats.curHealth, playerStats.maxHealth);
        }

        playerStats.isHolding = StarterGunId();
    }

    void Update()
    {
        // Đổi súng 
        ChangGun();
        SetGroundPosition();
    }

    private void SetGroundPosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 100f, LayerMask.GetMask("Platform"));
        _groundPosition = hit.point;
    }

    private void OnDrawGizmos()
    {
        // Draw a sphere at groundPosition if it has been set (not zero)
        if ((Vector2)_groundPosition != Vector2.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_groundPosition, 0.2f); // Adjust size as needed
        }
    }
    // Hàm gây sát thương cho người chơi
    public void DamagePlayer(float damage)
    {
        playerStats.curHealth -= damage; // Giảm curHealth

        // // Kiểm tra nếu người chơi hết máu
        // if (playerStats.curHealth <= 0)
        // {
        //     if (playerStats.lifeCount > 0)
        //     {
        //         playerStats.lifeCount--;
        //         GameMaster.KillPlayer(this); // Gọi hàm KillPlayer từ GameMaster
        //     }
        //     else
        //     {
        //         Debug.Log("Player đã hết mạng và không thể hồi sinh!");
        //         Destroy(this.gameObject);
        //     }
        // }

        if (playerStats.curHealth <= 0)
        {
            GameMaster.KillPlayer(this); // Gọi hàm KillPlayer từ GameMaster 
        }

        // Cập nhật StatusIndicator (nếu có)
        if (playerIndicator != null)
        {
            playerIndicator.SetHealth(playerStats.curHealth, playerStats.maxHealth);
        }
    }

    public Vector2 GetPlayerPosition()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    // Hàm kiểm tra xem người chơi đã có Perk hay chưa
    public bool HasPerk(PerkType perkType)
    {
        return acquiredPerks.Contains(perkType);
    }

    // Hàm thêm Perk vào danh sách Perk đã sở hữu
    public void AddPerk(PerkType perkType)
    {
        if (!HasPerk(perkType))
        {
            acquiredPerks.Add(perkType);
            Debug.Log("Đã thêm Perk: " + perkType);
        }
        else
        {
            Debug.LogWarning("Người chơi đã sở hữu Perk này: " + perkType);
        }
    }

    // Hàm loại bỏ Perk khi hết thời gian hoặc hiệu ứng
    public void RemovePerk(PerkType perkType)
    {
        if (HasPerk(perkType))
        {
            acquiredPerks.Remove(perkType);
            Debug.Log("Đã xóa Perk: " + perkType);
        }
    }

    // Thêm item vào Inventory
    public void AddToInventory(string gunId, int itemIndex)
    {
        // Kiểm tra nếu Inventory có nhiều hơn 2 item
        if (!inventory.Contains(gunId))
        {
            if (inventory.Count >= 2)
            {
                // Kiểm tra chỉ số itemIndex có hợp lệ hay không
                if (itemIndex >= 0 && itemIndex < inventory.Count)
                {
                    // Thay thế item tại vị trí itemIndex
                    inventory[itemIndex] = gunId;
                    Debug.Log("Item replaced at index " + itemIndex + ": " + gunId);
                }
            }
            else
            {
                inventory.Add(gunId);
                Debug.Log("Item added to inventory: " + gunId);
            }
        }
        else
        {
            // Debug.LogWarning("Item already exists in inventory: " + item);
        }
    }

    public string NotEquipedInventoryWP(){
        foreach (string gunId in inventory)
        {
            if (gunId != playerStats.isHolding)
            {
                return gunId;
            }
        }
        return null;
    }

    // Xóa item khỏi Inventory
    public void RemoveFromInventory(string item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            Debug.Log("Item removed from inventory: " + item);
        }
        else
        {
            Debug.LogWarning("Item not found in inventory: " + item);
        }
    }

    public string StarterGunId()
    {
        string StarterGunNameId = "Pistols_00";
        AddToInventory(StarterGunNameId, 0);
        return StarterGunNameId;
    }


    public int GetCurrentHoldingIndex()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (playerStats.isHolding == inventory[i])
            {
                return i; // Trả về chỉ số của súng hiện đang cầm
            }
        }

        // Trả về -1 nếu không tìm thấy súng trong inventory
        return -1;
    }


    public void ChangGun()
    {
        if (inventory.Count == 2)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                string gunId = inventory[0];
                gunPose.ChangePlayerArmsPose(gunId);
                playerStats.isHolding = gunId;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                string gunId = inventory[1];
                gunPose.ChangePlayerArmsPose(gunId);
                playerStats.isHolding = gunId;
            }
        }
    }
}