using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusIndicator : MonoBehaviour
{
    [SerializeField]
    private RectTransform healthBarRect;
    [SerializeField]
    private RectTransform staminaBarRect;
    public bool maintainHorizontalAlignment = true;
    [SerializeField]
    // private Text playerPointsText;
    private TextMeshProUGUI playerPointsText;
    // Start is called before the first frame update
    void Start()
    {

        if (healthBarRect == null)
        {
            Debug.LogError("STATUS INDICATOR: No health bar object referenced!");
        }
        if (playerPointsText == null)
        {
            Debug.LogError("STATUS INDICATOR: No health text object referenced!");
        }
    }


    public void SetHealth(float _cur, float _max)
    {
        float _value = (float)_cur / _max;

        healthBarRect.localScale = new Vector3(_value, healthBarRect.localScale.y, healthBarRect.localScale.z);

        if (maintainHorizontalAlignment)
        {
            // Always face the camera
            healthBarRect.rotation = Quaternion.Euler(0f, 0f, Camera.main.transform.eulerAngles.z);
        }
        else
        {
            // Follow the enemy's rotation
            healthBarRect.rotation = transform.rotation;
        }
    }

    public void SetStamina(float _cur, float _max)
    {
        float _value = (float)_cur / _max;

        staminaBarRect.localScale = new Vector3(_value, staminaBarRect.localScale.y, staminaBarRect.localScale.z);

        if (maintainHorizontalAlignment)
        {
            // Always face the camera
            staminaBarRect.rotation = Quaternion.Euler(0f, 0f, Camera.main.transform.eulerAngles.z);
        }
        else
        {
            // Follow the enemy's rotation
            staminaBarRect.rotation = transform.rotation;
        }

    }

    public void SetPoints(float _points)
    {
        playerPointsText.text = _points.ToString();

    }
}
