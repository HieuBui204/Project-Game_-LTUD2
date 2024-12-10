using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{
    public GameObject messageTextObject; // Thông báo mật khẩu sai (chứa TextMeshPro)
    public TMP_Text messageText;         // TextMeshPro để hiển thị thông báo
    public TMP_InputField usernameInputField;  // Trường nhập tên người dùng
    public TMP_InputField passwordInputField;  // Trường nhập mật khẩu
    public Toggle rememberMeToggle;
    public GameObject RegPanel;
    public GameObject LogPanel;
    static public string userID;
    CodeScripts codeScripts;

    void Start()
    {
        codeScripts = new CodeScripts();
        messageTextObject.SetActive(false);  // Đảm bảo ban đầu không hiển thị thông báo

        // Kiểm tra nếu đã lưu thông tin đăng nhập trước đó
        if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
        {
            usernameInputField.text = PlayerPrefs.GetString("Username");
            passwordInputField.text = PlayerPrefs.GetString("Password");
            rememberMeToggle.isOn = true; // Nếu có thông tin lưu trữ thì bật Toggle
        }
    }



    // Hàm đăng nhập khi nhấn nút
    public void btnLogClick()
    {
        string username = usernameInputField.text;  // Lấy tên người dùng từ InputField
        string password = passwordInputField.text;  // Lấy mật khẩu từ InputField

        var user = codeScripts.GetUserByUserName(username);  // Kiểm tra người dùng có tồn tại không
        Debug.Log(user);
        if (user != null)
        {
            userID = user.ID;
            Debug.Log(userID);
            if (user.Password == password)  // Kiểm tra mật khẩu
            {
                // Đăng nhập thành công
                messageText.text = "Login Successful!";
                messageTextObject.SetActive(true);

                // Nếu "Remember me" được chọn, lưu trữ thông tin người dùng
                if (rememberMeToggle.isOn)
                {
                    PlayerPrefs.SetString("Username", username);
                    PlayerPrefs.SetString("Password", password);
                }
                else
                {
                    // Nếu "Remember me" không được chọn, xóa thông tin người dùng khỏi PlayerPrefs
                    PlayerPrefs.DeleteKey("Username");
                    PlayerPrefs.DeleteKey("Password");
                }

                PlayerPrefs.Save(); // Lưu thông tin
                // Có thể chuyển đến scene khác nếu cần
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                // Mật khẩu sai
                messageText.text = "Incorrect password!";
                messageTextObject.SetActive(true);
                StartCoroutine(HideMessageAfterDelay(3f)); // Ẩn thông báo sau 3 giây
            }
        }
        else
        {
            // Người dùng không tồn tại
            messageText.text = "Username not found!";
            messageTextObject.SetActive(true);
            StartCoroutine(HideMessageAfterDelay(3f)); // Ẩn thông báo sau 3 giây
        }
    }

    // Coroutine để ẩn thông báo sau một khoảng thời gian
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Chờ trong khoảng thời gian delay
        messageTextObject.SetActive(false);      // Tắt thông báo
    }
    public void OnSelectUser()
    {
        usernameInputField.text = "";
    }
    public void OnSelectPass()
    {
        passwordInputField.text = "";
    }

    public void swapToReg()
    {
        RegPanel.SetActive(true);
        LogPanel.SetActive(false);
    }
}
