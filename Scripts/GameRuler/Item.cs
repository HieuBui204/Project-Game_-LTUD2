using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Item : MonoBehaviour
{
    public string itemId;
    public string itemName;
    public enum InteractionType { NONE, PickUp, Buy }
    public InteractionType interactType;
    public enum ItemType { NONE, gun }
    public ItemType itemType;
    public float itemCost;
    public bool fromMysteryBox = false;
    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
        gameObject.layer = 8; //item layer number
    }

    public void Interact(GameObject playerObj)
    {
        Player player = playerObj.GetComponent<Player>();
        int layer = gameObject.layer;

        switch (interactType)
        {
            case InteractionType.PickUp:
                break;

            case InteractionType.Buy:
                if (layer == LayerMask.NameToLayer("Item"))
                {
                    if (player.playerStats.point >= itemCost)
                    {
                        player.AddToInventory(itemId, player.GetCurrentHoldingIndex());
                        FindObjectOfType<GunPose>().ChangePlayerArmsPose(itemId);
                        Debug.Log("Buy Item: " + itemId + " For: " + itemCost);

                        player.playerStats.point -= itemCost;
                        player.playerStats.isHolding = itemId;

                        GameObject guns = GameObject.Find("Guns");
                        foreach (Transform gun in guns.transform)
                        {
                            if (gun.GetComponent<Weapon>().gunId == itemId)
                            {
                                gun.GetComponent<Weapon>().ResetsubAmmo();
                            }
                        }

                        GameObject playerStatsUI = GameObject.FindWithTag("PlayerStatsUI");
                        playerStatsUI.GetComponent<PlayerStatusIndicator>().SetPoints(player.playerStats.point);
                    }
                    else
                    {
                        Debug.Log("Not enough money!");
                    }

                }

                break;

            default:
                Debug.Log("NULLITEM!");
                break;
        }
    }

}
