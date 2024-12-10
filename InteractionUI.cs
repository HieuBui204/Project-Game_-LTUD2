using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    private GameObject instantiatedUI; // Đối tượng UI đã được instantiate
    private GameObject preDecObj; // Đối tượng được phát hiện trước đó
    private PlayerController playerController;

    [Header("UI Settings")]
    [SerializeField]
    private GameObject uiPrefab;
    [SerializeField]
    private Vector3 uiScale = Vector3.one; // Scale của UI
    [SerializeField]
    private Vector3 uiOffset = new Vector3(-1f, 0f, 0f); // Offset sang trái (tùy chỉnh theo nhu cầu)

    private Player player;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("Không tìm thấy InteractionSystem trong scene!");
        }

        if (uiPrefab == null)
        {
            Debug.LogError("UI Prefab chưa được gán trong Inspector!");
        }
    }

    void Update()
    {
        if (playerController != null)
        {
            GameObject curDecObj = playerController.detectedObject;

            if (curDecObj != preDecObj)
            {
                // Nếu có UI đang được instantiate, destroy nó
                if (instantiatedUI != null)
                {
                    Destroy(instantiatedUI);
                    instantiatedUI = null;
                }

                // Nếu detectedObject hiện tại có Tag là "Machine", instantiate UI
                if (curDecObj != null && curDecObj.CompareTag("Machine") && uiPrefab != null)
                {
                    PerkController machine = curDecObj.GetComponent<PerkController>();
                    if (machine != null)
                    {
                        // Kiểm tra xem người chơi đã có perk chưa
                        Player.PerkType perkType = machine.ConvertToPlayerPerkType(machine.perkAColaType);
                        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                        if (player.HasPerk(perkType)) // Kiểm tra xem player có perk chưa
                        {
                            Vector3 spawnPosition = curDecObj.transform.position + curDecObj.transform.TransformVector(uiOffset);
                            instantiatedUI = Instantiate(uiPrefab, spawnPosition, Quaternion.identity, curDecObj.transform);

                            UpdateCost updateCostComponent = instantiatedUI.GetComponentInChildren<UpdateCost>();
                            if (updateCostComponent != null)
                            {
                                updateCostComponent.UpdateOwned();
                            }
                        }
                        else
                        {
                            Vector3 spawnPosition = curDecObj.transform.position + curDecObj.transform.TransformVector(uiOffset);
                            instantiatedUI = Instantiate(uiPrefab, spawnPosition, Quaternion.identity, curDecObj.transform);

                            UpdateCost updateCostComponent = instantiatedUI.GetComponentInChildren<UpdateCost>();
                            if (updateCostComponent != null)
                            {
                                updateCostComponent.UpdateCostText(machine.Cost);
                            }
                        }
                    }
                    // if (machine != null && machine.perkAColaType == PerkController.PerkAColaType.Juggernog)
                    // {
                    //     //Custom cho Jugg
                    //     Vector3 spawnPosition = curDecObj.transform.position + curDecObj.transform.TransformVector(-0.8f, 0f, 0f);
                    //     instantiatedUI = Instantiate(uiPrefab, spawnPosition, Quaternion.identity, curDecObj.transform);

                    //     // Truy cập UpdateCost và cập nhật văn bản hiển thị với chi phí của máy
                    //     UpdateCost updateCostComponent = instantiatedUI.GetComponentInChildren<UpdateCost>();
                    //     if (updateCostComponent != null)
                    //     {
                    //         updateCostComponent.UpdateCostText(machine.Cost); // Truyền giá trị cost vào script UpdateCost
                    //     }
                    // }
                    else
                    {

                    }

                    // Thiết lập scale cho UI
                    instantiatedUI.transform.localScale = uiScale;
                }

                if (curDecObj != null && curDecObj.CompareTag("GunFrame") && uiPrefab != null)
                {
                    Item item = curDecObj.GetComponent<Item>();
                    if (item != null && item.fromMysteryBox == false)
                    {
                        Vector3 spawnPosition = curDecObj.transform.position + curDecObj.transform.TransformVector(uiOffset);
                        instantiatedUI = Instantiate(uiPrefab, spawnPosition, Quaternion.identity, curDecObj.transform);
                        instantiatedUI.transform.localScale = uiScale;

                        UpdateCost updateCostComponent = instantiatedUI.GetComponentInChildren<UpdateCost>();
                        if (updateCostComponent != null)
                        {
                            updateCostComponent.UpdateCostText(item.itemCost);
                        }
                    }
                    else if (item != null && item.fromMysteryBox == true)
                    {
                        Vector3 spawnPosition = curDecObj.transform.position + curDecObj.transform.TransformVector(-1.5f, 0.25f, 0f);
                        instantiatedUI = Instantiate(uiPrefab, spawnPosition, Quaternion.identity, curDecObj.transform);
                        instantiatedUI.transform.localScale = uiScale;

                        UpdateCost updateCostComponent = instantiatedUI.GetComponentInChildren<UpdateCost>();
                        if (updateCostComponent != null)
                        {
                            updateCostComponent.MysteryBoxUI_Gun();
                        }
                    }
                }

                if (curDecObj != null && curDecObj.CompareTag("Mystery Box") && uiPrefab != null)
                {
                    MysteryBox box = curDecObj.GetComponent<MysteryBox>();
                    if (box != null)
                    {
                        Vector3 spawnPosition = curDecObj.transform.position + curDecObj.transform.TransformVector(-3f, 0f, 0f);
                        instantiatedUI = Instantiate(uiPrefab, spawnPosition, Quaternion.identity, curDecObj.transform);
                        instantiatedUI.transform.localScale = new Vector3(2, 2, 2);

                        UpdateCost updateCostComponent = instantiatedUI.GetComponentInChildren<UpdateCost>();
                        if (updateCostComponent != null)
                        {
                            updateCostComponent.MysteryBoxUI(box.Cost);
                        }
                    }
                }

                // Cập nhật lại preDecObj
                preDecObj = curDecObj;
            }
        }
    }
}