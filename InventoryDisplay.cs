using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    private GameObject backInventory;
    private GameObject beltInventory;
    private GameObject guns;
    private string wasHolding = null;

    void Awake()
    {
        backInventory = GameObject.Find("BackInventory");
        beltInventory = GameObject.Find("BeltInventory");
        guns = GameObject.Find("Guns");
        wasHolding = GetComponent<Player>().playerStats.isHolding;
        if (backInventory == null || beltInventory == null || guns == null)
        {
            Debug.LogError("Not Found.");
        }
    }

    void Update()
    {
        OnChangeHoldingGunEvent();
    }

    void OnChangeHoldingGunEvent()
    {
        string isHolding = GetComponent<Player>().playerStats.isHolding;
        if (wasHolding != isHolding)
        {
            UpdateInventoryDisplay();
        }
        wasHolding = isHolding;
    }

    void UpdateInventoryDisplay()
    {
        string notequipgunId = GetComponent<Player>().NotEquipedInventoryWP();
        if (guns == null || backInventory == null || beltInventory == null || notequipgunId == null) return;
        GameObject targetInventory = notequipgunId.StartsWith("Pistols") ? beltInventory : backInventory;
        ResetInventoryDisplay();
        foreach (Transform gunDisplay in targetInventory.transform)
        {
            if (gunDisplay.name == notequipgunId)
            {
                gunDisplay.gameObject.SetActive(true);
            }
        }
    }

    void ResetInventoryDisplay()
    {
        foreach (Transform gunDisplay in backInventory.transform)
        {
            gunDisplay.gameObject.SetActive(false);
        }

        foreach (Transform gunDisplay in beltInventory.transform)
        {
            gunDisplay.gameObject.SetActive(false);
        }
    }
}
