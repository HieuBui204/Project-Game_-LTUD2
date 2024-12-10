using UnityEngine;

public class ArmRotation : MonoBehaviour
{
    private Transform bodyTransform;
    public bool facingRight = true;

    private void Start()
    {
        bodyTransform = transform.parent;
    }

    private void LateUpdate()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        Vector3 aimDirection = mousePosition - bodyTransform.position;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        // Vẽ đường thẳng từ vị trí của transform đến vị trí chuột
        // Debug.DrawLine(BodyPosition.position, mousePosition, Color.white);

        // Toggle hiển thị con trỏ chuột khi nhấn phím F
        if (Input.GetKeyDown(KeyCode.F))
        {
            Cursor.visible = !Cursor.visible;
        }

        FlipController(mousePosition);
    }

    private void FlipController(Vector3 mousePosition)
    {
        // Flip đối tượng con nếu chuột ở bên trái
        if (mousePosition.x < bodyTransform.position.x && facingRight)
        {
            FlipChild();
        }
        else if (mousePosition.x >= bodyTransform.position.x && !facingRight)
        {
            FlipChild();
        }
    }

    // Hàm flip đối tượng con bằng cách thay đổi localScale.x của nó
    private void FlipChild()
    {
        facingRight = !facingRight;

        if (facingRight)
        {
            bodyTransform.eulerAngles = new Vector3(0f, 0f, 0f);
            transform.localScale = new Vector3(1f, 1f, 1f);

        }
        else
        {
            bodyTransform.eulerAngles = new Vector3(0f, -180f, 0f);
            transform.localScale = new Vector3(1f, -1f, 1f);
        }

    }

    // Hàm để lấy vị trí chuột trong World Space
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition; // Lấy vị trí chuột trong Screen Space
        mousePosition.z = Camera.main.nearClipPlane; // Đặt Z để lấy đúng vị trí trong World Space

        // Chuyển vị trí chuột từ Screen Space sang World Space
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f; // Đảm bảo Z là 0 trong game 2D

        return worldPosition;
    }

    public void ResetArmContainerRotation()
    {
        if (facingRight)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {
            transform.localScale = new Vector3(1f, -1f, 1f);
            transform.rotation = Quaternion.Euler(new Vector3(-180, -180, 0));
        }

    }
}
