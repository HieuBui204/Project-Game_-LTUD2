using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraMode : MonoBehaviour
{
    private GameObject mainCharacter; 
    private GameObject mouse; 
    private GameObject crosshair; 
    private float cameraMaxDistance = 10f; 


    private void Start()
    {
        mainCharacter = GameObject.Find("Player");
        mouse = GameObject.Find("Mouse");
        crosshair = GameObject.Find("Crosshair");

        if (mouse == null)
        {
            Debug.LogError("Mouse object not found!");
        }
    }

    private void Update()
    {
        if (mainCharacter == null || mouse == null) return;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; 

        Vector3 mainCharacterPosition = mainCharacter.transform.position;

        float distance = Vector3.Distance(mainCharacterPosition, mouseWorldPosition);
        Vector3 direction = (mouseWorldPosition - mainCharacterPosition).normalized;

        if (distance > cameraMaxDistance)
        {
            mouse.transform.position = mainCharacterPosition + direction * cameraMaxDistance;
        }
        else
        {
            mouse.transform.position = mainCharacterPosition;
        }

        crosshair.transform.position = mainCharacterPosition + direction * distance;
    }

    private void OnDrawGizmos()
    {
        if (mainCharacter == null || mouse == null) return;

        Gizmos.color = Color.red;

        Vector3 mainCharacterPosition = mainCharacter.transform.position;
        Gizmos.DrawLine(mainCharacterPosition, mouse.transform.position);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(mainCharacterPosition, 0.5f); 
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(mouse.transform.position, 0.5f);
    }
}
