using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIndicator : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI currentAmmo;
    [SerializeField]
    private TextMeshProUGUI subAmmo;
    [SerializeField]
    private TextMeshProUGUI gunName;
    [SerializeField]
    private List<GameObject> fireMode;

    void Start()
    {
        if (currentAmmo == null)
        {
            Debug.LogError("STATUS INDICATOR: No current ammo object referenced!");
        }
        if (subAmmo == null)
        {
            Debug.LogError("STATUS INDICATOR: No sub ammo object referenced!");
        }
        if (gunName == null)
        {
            Debug.LogError("STATUS INDICATOR: No gun name object referenced!");
        }
        if (fireMode == null)
        {
            Debug.LogError("STATUS INDICATOR: No fire mode object referenced!");
        }
    }


    public void SetAmmo(float curAmmo, float subAmmo)
    {
        currentAmmo.text = curAmmo.ToString();
        this.subAmmo.text = subAmmo.ToString();
    }

    public void SetGunName(string gunName)
    {
        this.gunName.text = gunName;
    }

    public void SetFireMode(string gunMode)
    {
        foreach (GameObject obj in fireMode)
        {
            if (obj.name == gunMode)
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }
        }
    }
}
