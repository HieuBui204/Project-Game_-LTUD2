using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI mess;
    [SerializeField]
    GameObject contain;
    private Player player;
    void Awake()
    {
        // Tìm đối tượng Player trong scene
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Không tìm thấy đối tượng Player trong scene!");
            return;
        }

        // Tìm PerkController từ đối tượng có tag "Machine"
        PerkController perkController = GameObject.FindGameObjectWithTag("Machine").GetComponent<PerkController>();
        if (perkController == null)
        {
            Debug.LogError("Không tìm thấy PerkController trên đối tượng với tag 'Machine'!");
            return;
        }

        // Kiểm tra nếu người chơi đã có Perk này
        Player.PerkType perkType = perkController.ConvertToPlayerPerkType(perkController.perkAColaType); // Lấy loại Perk từ perkController
        if (player.HasPerk(perkType))
        {
            mess.text = "Owned";
            contain.SetActive(false);
            Debug.Log("Player đã có Perk này, không thể mua lại.");
            return;
        }
    }
}
