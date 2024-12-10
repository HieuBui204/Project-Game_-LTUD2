using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GunPose : MonoBehaviour
{
    private Player player;
    private GameObject guns;
    public Animator animator;
    // Phương thức để thay đổi hướng nhân vật và kích hoạt súng
    private void Awake()
    {
        guns = GameObject.Find("Guns");
        player = this.GetComponent<Player>();
    }

    private void Start()
    {
        SetupStarterGun();
    }

    public void ChangePlayerArmsPose(string gunId)
    {
        if (player.playerStats.isHolding != null)
        {
            if (player.playerStats.isHolding.StartsWith("Pistols"))
            {
                animator.SetBool("isUsing_Pistols", false);
            }
            else
            {
                animator.SetBool("isUsing_" + player.playerStats.isHolding, false);
            }

        }

        if (gunId.StartsWith("Pistols"))
        {
            StartCoroutine(ActiveChangeGunPose_ToPistols());
        }
        else
        {
            StartCoroutine(ActiveChangeGunPose(gunId));
        }

        ActivateGun(gunId);
        player.playerStats.isHolding = gunId;
    }

    // Phương thức để active object con trong guns dựa trên tên
    public void ActivateGun(string gunId)
    {
        foreach (Transform gun in guns.transform)
        {
            if (gun.GetComponent<Weapon>().gunId == gunId)
            {
                gun.gameObject.SetActive(true);
            }
            else
            {
                gun.gameObject.SetActive(false);
            }
        }
    }

    public void DeActiveGun()
    {
        foreach (Transform gun in guns.transform)
        {
            gun.gameObject.SetActive(false);
        }
    }

    private IEnumerator ActiveChangeGunPose(string gunName)
    {
        // Kích hoạt animation
        animator.SetTrigger("ChangeTo_" + gunName);
        animator.SetBool("isUsing_" + gunName, true);

        // Chờ đợi kết thúc của animation
        yield return new WaitForSeconds(1.0f); // Thay thế 1.0f bằng thời gian thực của animation của bạn
    }

    private IEnumerator ActiveChangeGunPose_ToPistols()
    {
        // Kích hoạt animation
        animator.SetTrigger("ChangeTo_Pistols_All");
        animator.SetBool("isUsing_Pistols", true);

        // Chờ đợi kết thúc của animation
        yield return new WaitForSeconds(1.0f); // Thay thế 1.0f bằng thời gian thực của animation của bạn
    }

    public void SetupStarterGun()
    {
        string startgunid = player.StarterGunId();

        if (startgunid.StartsWith("Pistols"))
        {
            animator.SetTrigger("ChangeTo_Pistols_All");
            animator.SetBool("isUsing_Pistols", true);
            ActivateGun(startgunid);
        }
        else
        {
            Debug.Log("Súng khởi đầu không hợp lệ (phải là súng lục)");
        }
    }
}
