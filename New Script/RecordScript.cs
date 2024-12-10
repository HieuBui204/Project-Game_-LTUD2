using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordScript : MonoBehaviour
{
    public GameObject itemPrefab;
    public GameObject parent;
    CodeScripts codeScripts;

    void Start()
    {
        codeScripts = new CodeScripts();
        string dd = LoginScript.userID;
        // string dd = "USER001";
        Debug.Log(dd);
        var profile = codeScripts.GetProfileByUserID(dd);
        if (profile != null)
        {
            loadRecordsToScrollView(profile.ID);
        }
        else
        {
            Debug.Log("Khong thay profile");
        }
    }
    public void loadRecordsToScrollView(string proID)
    {
        // Xóa các đối tượng cũ nếu có
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }

        // Tải dữ liệu từ database và gán vào ScrollView
        foreach (var item in codeScripts.GetAllRecordByProfileID(proID))
        {
            Debug.Log(item);
            // Tạo một prefab và gán dữ liệu
            GameObject newRecord = Instantiate(itemPrefab, parent.transform);
            var recordScript = newRecord.GetComponent<recordPre>();
            if (recordScript != null)
            {
                recordScript.SetData(item.ID, item.ID_Profile, item.Time, item.Kill, item.Score, item.Save_at);
            }
        }
    }

}
