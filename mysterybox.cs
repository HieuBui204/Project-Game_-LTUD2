using System.Collections;
using UnityEngine;

public class MysteryBox : MonoBehaviour
{
    public GameObject[] weapons; // Danh sách các vũ khí hoặc item
    public Transform spawnPoint; // Điểm xuất hiện vũ khí
    public float rollSpeed = 0.1f;  // Thời gian chuyển đổi giữa các vũ khí trong roll
    public int rollCount = 20;     // Số lần đổi vũ khí trong roll
    public float finalDisplayTime = 10f; // Thời gian hiển thị vũ khí cuối cùng
    private bool isActive = true; // Mystery box có hoạt động không?
    private Player player;

    [Header("Stats")]
    public float Cost;

    [Header("Teddy Bear")]
    public GameObject teddyBear; // Prefab của Teddy Bear

    [Header("Effect Roll")]
    public Transform containRoll; // Trỏ đến ContainRoll trong spawnPoint
    public Transform mysterybox;

    [Header("Sounds")]
    public AudioClip sound;

    [Header("Mystery Box Locations")]
    public Transform[] possibleLocations; // Danh sách các vị trí

    private GameObject currentWeapon; // Vũ khí đang hiển thị

    public void Interact(GameObject playerObj)
    {
        player = playerObj.GetComponent<Player>(); // Lấy script Player
        int layer = gameObject.layer;

        // Kiểm tra layer của đối tượng
        if (layer == LayerMask.NameToLayer("MysteryBox") && isActive)
        {
            if (player.playerStats.point >= Cost)
            {
                player.playerStats.point -= Cost;
                PlaySound();
                StartCoroutine(ActivateMysteryBox());
            }
            else
            {
                Debug.Log("Không đủ tiền!");
            }
        }
    }

    private IEnumerator ActivateMysteryBox()
    {
        isActive = false; // Khóa Mystery Box khi đang hoạt động
        // Lưu lại Layer gốc
        int originalLayer = gameObject.layer;

        // Đặt Layer về Default khi bắt đầu roll
        gameObject.layer = LayerMask.NameToLayer("Default");
        // Kích hoạt ContainRoll và bắt đầu animation
        containRoll.gameObject.SetActive(true);
        Animator animator = containRoll.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Roll"); // Giả sử bạn có trigger "Roll" trong Animator
        }

        // Roll các vũ khí
        for (int i = 0; i < rollCount; i++)
        {
            GameObject selectedWeapon = weapons[Random.Range(0, weapons.Length)];

            // Nếu đã có vũ khí đang hiển thị, xóa nó
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }

            // Tạo vũ khí mới tại spawnPoint
            currentWeapon = Instantiate(selectedWeapon, spawnPoint.position, Quaternion.identity);
            currentWeapon.transform.SetParent(containRoll, true);
            // Đặt Tag và Layer của vũ khí thành "Untagged" và "Default"
            currentWeapon.tag = "Untagged";
            currentWeapon.layer = LayerMask.NameToLayer("Default");



            yield return new WaitForSeconds(rollSpeed);
        }

        // Kết quả cuối cùng
        GameObject finalWeapon = weapons[Random.Range(0, weapons.Length)];
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        currentWeapon = Instantiate(finalWeapon, spawnPoint.position, Quaternion.identity);
        currentWeapon.transform.SetParent(containRoll, true);

        // Đặt Tag và Layer của vũ khí thành "Untagged" và "Default"
        currentWeapon.tag = "GunFrame";
        currentWeapon.layer = LayerMask.NameToLayer("Item");

        // Kiểm tra xem currentWeapon có chứa component Item không
        Item itemComponent = currentWeapon.GetComponent<Item>();
        if (itemComponent != null)
        {
            // Nếu có, đặt thuộc tính fromMysteryBox = true
            itemComponent.fromMysteryBox = true;
        }
        else
        {
            Debug.Log("GGG");
        }

        Animator animator_M = mysterybox.GetComponent<Animator>();

        // Nếu kết quả là Teddy Bear
        if (finalWeapon == teddyBear)
        {
            Debug.Log("Kết quả là Teddy Bear!");
            if (animator_M != null)
            {
                finalDisplayTime = 2f; // Thời gian hiển thị vũ khí cuối cùng
                StartCoroutine(TriggerDisappearAnimation(animator_M));

            }
        }

        // Hiển thị vũ khí cuối cùng trong một khoảng thời gian
        yield return new WaitForSeconds(finalDisplayTime);
        // Khôi phục Layer ban đầu sau khi roll kết thúc
        gameObject.layer = originalLayer;
        // Xóa vũ khí cuối cùng
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        // Tắt ContainRoll sau khi kết thúc
        containRoll.gameObject.SetActive(false);

        isActive = true; // Mystery Box sẵn sàng cho lần tiếp theo
    }


    // Phát âm thanh khi kích hoạt mystery box
    void PlaySound()
    {
        if (sound != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(sound);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy AudioClip hoặc AudioManager chưa được thiết lập!");
        }
    }

    private IEnumerator TriggerDisappearAnimation(Animator animator)
    {
        yield return new WaitForSeconds(2f); // Chờ trước khi animation bắt đầu
        animator.SetTrigger("disappear"); // Kích hoạt animation

        yield return new WaitForSeconds(2f); // Chờ animation hoàn thành

        // Ẩn Mystery Box
        gameObject.SetActive(false);
        Debug.Log("An thanh cong");

        // Chọn vị trí mới
        Transform newLocation = possibleLocations[Random.Range(0, possibleLocations.Length)];

        // Đảm bảo vị trí mới không trùng với vị trí hiện tại
        while (newLocation.position == transform.position)
        {
            newLocation = possibleLocations[Random.Range(0, possibleLocations.Length)];
        }

        // Di chuyển Mystery Box đến vị trí mới
        transform.position = newLocation.position;

        gameObject.SetActive(true);

        // Kích hoạt animation xuất hiện
        if (animator != null)
        {
            animator.SetTrigger("appear"); // Kích hoạt animation xuất hiện
        }

        Debug.Log("Mystery Box đã di chuyển đến vị trí mới!");
    }


}
