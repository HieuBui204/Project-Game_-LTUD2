using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] float explosionDelay = 3f; // Thời gian trước khi nổ
    [SerializeField] float explosionRadius = 50f; // Bán kính vụ nổ
    [SerializeField] LayerMask explosionLayerMask; // Lớp các đối tượng chịu ảnh hưởng vụ nổ
    [SerializeField] float groundCheckRadius = 1f; // Bán kính kiểm tra ground
    [SerializeField] LayerMask groundLayer; // Lớp ground để kiểm tra
    private bool isGrounded = false; // Trạng thái chạm đất
    private Rigidbody2D rb; // Tham chiếu tới RigidBody2D
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(ExplodeAfterDelay());
    }
    private void Update()
    {
        CheckGround(); // Kiểm tra trạng thái chạm đất
        if (isGrounded)
        {
            DisableRigidbody();
        }
    }
    // Coroutine để đợi thời gian trước khi nổ
    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    void Explode()
    {
        // Tìm tất cả các đối tượng trong phạm vi vụ nổ
        Collider2D[] objectsToDamage = Physics2D.OverlapCircleAll(transform.position, explosionRadius, explosionLayerMask);

        Debug.Log("Objects found: " + objectsToDamage.Length);

        foreach (Collider2D obj in objectsToDamage)
        {
                Debug.Log(obj.gameObject.name);
                obj.gameObject.GetComponent<Enemy>().DamageEnemy(100000);
        }
    }


    // Vẽ bán kính vụ nổ trong Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void CheckGround()
    {
        // Kiểm tra nếu lựu đạn chạm đất
        isGrounded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer) != null;
    }

    private void DisableRigidbody()
    {
        rb.simulated = false; // Vô hiệu hóa Rigidbody2D bằng cách đặt simulated = false
    }
}
