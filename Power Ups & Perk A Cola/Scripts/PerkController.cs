using System.Collections;
using UnityEngine;

public class PerkController : MonoBehaviour
{
    // Enum cho các loại Perk-A-Cola
    public enum PerkAColaType
    {
        Juggernog,
        SpeedCola,
        QuickRevive,
        DeadShot,
        DoubleTap,
        Revive,
        StaminUp,
        MuleKick
    }

    [SerializeField]
    public PerkAColaType perkAColaType; // Loại Perk-A-Cola
    // public GameObject buyPrompt; // Thông báo mua
    public GameObject perkEffectPrefab; // Prefab chứa PerkEffect

    [Header("Perk-A-Cola Sounds")]
    public AudioClip perkSound;

    [Header("Perk Icons")]
    public GameObject Icon; // Icon của Perk

    private bool playerInRange; // Kiểm tra người chơi có trong phạm vi tương tác không
    private Player player;
    [Header("Perk Costs")]
    [SerializeField]
    public float Cost = 3000f;
    [SerializeField]

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
        gameObject.layer = 11; //item layer number
    }

    void Start()
    {
        HideAllPerkIcons(); // Ẩn icon perk lúc ban đầu
    }

    void Update()
    {
        // // Nếu người chơi trong phạm vi và nhấn phím B
        // if (playerInRange && Input.GetKeyDown(KeyCode.B))
        // {
        //     BuyPerkACola();
        // }
    }

    public void Interact(GameObject playerObj)
    {
        player = playerObj.GetComponent<Player>(); // Lấy script Player
        // Kiểm tra layer của đối tượng
        int layer = gameObject.layer;
        if (layer == LayerMask.NameToLayer("PerkMachine"))
        {
            BuyPerkACola();
        }
    }

    // Hàm để mua Perk-A-Cola
    public void BuyPerkACola()
    {
        if (player == null)
        {
            Debug.LogError("Không tìm thấy script Player trên đối tượng Player!");
            return;
        }

        // Kiểm tra nếu người chơi đã có Perk này
        Player.PerkType perkType = ConvertToPlayerPerkType(perkAColaType);
        if (player.HasPerk(perkType))
        {
            Debug.Log("Player đã có Perk này, không thể mua lại.");
            return;
        }

        Debug.Log("Player đã mua Perk-A-Cola: " + perkAColaType);

        if (player.playerStats.point >= Cost)
        {
            player.playerStats.point -= Cost;
            // Tạo hiệu ứng Perk
            ApplyPerkACola(player);

            // Hiển thị icon của Perk
            ShowPerkIcon();

            // Phát âm thanh khi kích hoạt Perk-A-Cola
            PlayPerkSound();

            // Cập nhật UI
            GameObject playerStatsUI = GameObject.FindWithTag("PlayerStatsUI");
            PlayerStatusIndicator playerIndicator = playerStatsUI.GetComponent<PlayerStatusIndicator>();
            playerIndicator.SetPoints(player.playerStats.point);

            UpdateCost upUI = GetComponentInChildren<UpdateCost>();
            if (upUI != null)
            {
                upUI.UpdateOwned();
            }
        }
        else
        {
            Debug.Log("Không đủ tiền");
        }
    }

    // Hàm chuyển đổi từ PerkAColaType sang Player.PerkType
    public Player.PerkType ConvertToPlayerPerkType(PerkAColaType perkAColaType)
    {
        switch (perkAColaType)
        {
            case PerkAColaType.Juggernog:
                return Player.PerkType.Juggernog;
            case PerkAColaType.SpeedCola:
                return Player.PerkType.SpeedCola;
            case PerkAColaType.QuickRevive:
                return Player.PerkType.QuickRevive;
            case PerkAColaType.StaminUp:
                return Player.PerkType.StaminUp;
            default:
                Debug.LogError("Không tìm thấy PerkType tương ứng!");
                return Player.PerkType.Juggernog; // Giá trị mặc định
        }
    }

    // Áp dụng Perk-A-Cola và tạo hiệu ứng
    void ApplyPerkACola(Player playerScript)
    {
        // Tạo PerkEffect object để quản lý hiệu ứng Perk
        GameObject perkEffectObj = Instantiate(perkEffectPrefab, playerScript.transform.position, Quaternion.identity);
        PerkEffect perkEffect = perkEffectObj.GetComponent<PerkEffect>();

        if (perkEffect != null)
        {
            // Truyền loại Perk để áp dụng hiệu ứng
            perkEffect.Initialize(perkAColaType);

            // Thêm Perk vào danh sách Perk đã sở hữu của người chơi
            playerScript.AddPerk(ConvertToPlayerPerkType(perkAColaType));
        }
        else
        {
            Debug.LogError("PerkEffect không được tìm thấy trên prefab!");
        }
    }

    // Hàm hiển thị icon của Perk
    void ShowPerkIcon()
    {
        if (Icon != null)
        {
            Icon.SetActive(true); // Hiển thị icon của Perk đã mua
        }
    }

    // Hàm ẩn tất cả các icon Perk
    void HideAllPerkIcons()
    {
        if (Icon != null)
        {
            Icon.SetActive(false);
        }
    }

    // Phát âm thanh khi kích hoạt Perk-A-Cola
    void PlayPerkSound()
    {
        if (perkSound != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(perkSound);
            Debug.Log("Phát âm thanh Perk-A-Cola: " + perkSound.name);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy AudioClip hoặc AudioManager chưa được thiết lập!");
        }
    }
}
