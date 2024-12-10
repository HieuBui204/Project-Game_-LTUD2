using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UpdateCost : MonoBehaviour
{
    [Header("contain1")]
    [SerializeField]
    public GameObject contain1;
    public TextMeshProUGUI costText1;
    [Header("contain2")]
    [SerializeField]
    public GameObject contain2;
    [SerializeField]

    public TextMeshProUGUI costText2;

    // Phương thức để cập nhật văn bản hiển thị chi phí
    public void UpdateCostText(float cost)
    {
        if (costText1 != null)
        {
            costText1.text = cost.ToString() + "$"; // Cập nhật văn bản hiển thị chi phí
        }
        else
        {
            Debug.LogError("TextMeshProUGUI cho chi phí chưa được gán!");
        }
    }

    public void UpdateOwned()
    {
        costText1.text = "Owned";
        contain2.SetActive(false);
    }

    public void MysteryBoxUI(float cost)
    {
        if (costText1 != null)
        {
            costText1.text = cost.ToString() + "$"; // Cập nhật văn bản hiển thị chi phí
        }
        else
        {
            Debug.LogError("TextMeshProUGUI cho chi phí chưa được gán!");
        }
    }
    public void MysteryBoxUI_Gun()
    {
        contain1.SetActive(false);  // Tắt contain1
        // contain2.transform.position = new Vector3(0, 0, 0);  // Đặt vị trí của contain2 về (0, 0, 0)
        costText2.text = "'B' to take";
    }
}
