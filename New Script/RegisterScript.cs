using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegisterScript : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_InputField emailInputField;
    public TMP_InputField passInputField;
    public TMP_InputField rePassInputField;

    public GameObject RegPanel;
    public GameObject LogPanel;
    public GameObject messageTextObject; // Thông báo lỗi
    public TMP_Text messageText;         // Thông báo lỗi (TextMeshPro)
    CodeScripts codeScripts;

    void Start()
    {
        codeScripts = new CodeScripts();
        messageTextObject.SetActive(false);  // Đảm bảo ban đầu không hiển thị thông báo
    }
    public void btnRegClick()
    {
        string username = usernameInputField.text;
        string email = emailInputField.text;
        string pass = passInputField.text;
        string rePass = rePassInputField.text;

        // Kiểm tra xem có trường nào trống không
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(rePass))
        {
            messageText.text = "Please fill in all fields!";
            messageTextObject.SetActive(true);
            StartCoroutine(HideMessageAfterDelay(3f)); // Ẩn thông báo sau 3 giây
            return; // Dừng hàm nếu có trường nào trống
        }

        // Kiểm tra mật khẩu và xác nhận mật khẩu có khớp không
        if (pass == rePass)
        {
            // Kiểm tra nếu tài khoản đã tồn tại
            var existingUser = codeScripts.GetUserByUserName(username);
            if (existingUser != null)
            {
                // Người dùng đã tồn tại
                messageText.text = "Username already exists!";
                messageTextObject.SetActive(true);
                StartCoroutine(HideMessageAfterDelay(3f)); // Ẩn thông báo sau 3 giây
                return;
            }

            // Tạo tài khoản mới
            var newUser = codeScripts.CreateNewAccount(username, email, pass, System.DateTime.Now);

            if (newUser != null)
            {
                messageText.text = "Account created successfully!";
                messageTextObject.SetActive(true);
                StartCoroutine(HideMessageAfterDelay(3f)); // Ẩn thông báo sau 3 giây
                swapToLog(); // Chuyển về màn hình đăng nhập
            }
            else
            {
                messageText.text = "Error creating account!";
                messageTextObject.SetActive(true);
                StartCoroutine(HideMessageAfterDelay(3f)); // Ẩn thông báo sau 3 giây
            }
        }
        else
        {
            // Mật khẩu và xác nhận mật khẩu không khớp
            messageText.text = "Passwords do not match!";
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
    public void swapToLog()
    {
        LogPanel.SetActive(true);
        RegPanel.SetActive(false);
    }

}
