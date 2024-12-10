using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatusIndicator : MonoBehaviour
{
    [SerializeField]
    private RectTransform healthBarRect;
    public bool maintainHorizontalAlignment = true;
    [SerializeField]
    private Text healthText;
    // Start is called before the first frame update
    void Start()
    {

        if (healthBarRect == null)
        {
            Debug.LogError("STATUS INDICATOR: No health bar object referenced!");
        }
        if (healthText == null)
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

        healthText.text = _cur + "/" + _max + " HP";
    }
}
