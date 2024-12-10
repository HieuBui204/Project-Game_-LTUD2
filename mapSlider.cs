using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mapSlider : MonoBehaviour
{
    public Image mapImage; // Image hiển thị map
    public Button leftButton; // Nút trái
    public Button rightButton; // Nút phải
    public Sprite[] maps; // Mảng chứa các hình ảnh map

    static public int currentMapIndex = 0; // Vị trí hiện tại trong mảng maps

    void Start()
    {
        // Gắn sự kiện cho các nút
        leftButton.onClick.AddListener(PreviousMap);
        rightButton.onClick.AddListener(NextMap);

        // Hiển thị map đầu tiên
        UpdateMap();
    }

    void PreviousMap()
    {
        if (currentMapIndex > 0)
        {
            currentMapIndex--;
            UpdateMap();
        }
    }

    void NextMap()
    {
        if (currentMapIndex < maps.Length - 1)
        {
            currentMapIndex++;
            UpdateMap();
        }
    }

    void UpdateMap()
    {
        mapImage.sprite = maps[currentMapIndex];
    }
}
