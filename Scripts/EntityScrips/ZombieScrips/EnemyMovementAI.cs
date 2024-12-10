using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyMovementAI : MonoBehaviour
{
    public Pathfinder pathfinder; // Tham chiếu đến Pathfinder để tìm đường đi
    public GameObject target;      // Mục tiêu cần di chuyển đến
    public LayerMask groundLayer; // Layer để kiểm tra đối tượng đang trên mặt đất
    private float gravity = 3f;  // Trọng lực của đối tượng
    private List<Vector3> path = new List<Vector3>(); // Đường đi
    private int currentPathIndex = 0; // Chỉ số node hiện tại trong đường đi
    private Vector3 prevNode;  // Node mục tiêu hiện tại
    private Vector3 targetNode;  // Node mục tiêu hiện tại
    private Rigidbody2D rb2D;    // Rigidbody 2D của đối tượng
    public bool isGrounded = false; // Kiểm tra xem đối tượng có đang trên mặt đất không
    private float padding = 0.6f; // Độ sai số để kiểm tra đến gần node
    private bool isPathUpdated = false; // Đánh dấu khi path được cập nhật
    private Vector2 smoothVelocity = Vector2.zero;
    public Transform GroundCheck; // Tham chiếu đến GroundCheck
    private const float groundCheckRadius = 0.1f; // Bán kính để kiểm tra mặt đất
    public bool isFacingRight = false;
    private float timeToReachNode = 0f; // Thời gian để di chuyển đến node hiện tại
    private float maxTimeToReachNode = 1f; // Giới hạn thời gian tối đa để di chuyển đến một node
    private float checkPathTimer = 0f; // Biến theo dõi thời gian đã trôi qua
    private const float checkPathInterval = 0.2f; // Thời gian giữa các lần kiểm tra
    private Enemy enemy;
    private Animator animator;
    private bool wasGrounded = false; // Trạng thái trước đó của GroundCheck

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();
        setupAnimationParameter_zombieClass();

        // Tìm Pathfinder nếu chưa gán
        if (pathfinder == null)
        {
            pathfinder = FindObjectOfType<Pathfinder>();
        }

        // Thiết lập trọng lực của đối tượng
        rb2D.gravityScale = gravity;

        // Kiểm tra nếu không tìm thấy GroundCheck thì in ra cảnh báo
        if (GroundCheck == null)
        {
            Transform groundCheck = transform.Find("GroundCheck");
            if (groundCheck == null)
            {
                Debug.LogWarning("GameObject GroundCheck chưa được tạo.");
            }
            else
            {
                GroundCheck = groundCheck;
                // Thiết lập vị trí mặc định của BodyPoint so với đáy của Collider2D
                AdjustBPAndGCPosition();
            }
        }
    }

    GameObject FindTarget()
    {
        // Tìm đối tượng có tag "Player"
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            return player; // Trả về Transform của đối tượng "Player"
        }
        else
        {
            return null; // Không tìm thấy đối tượng
        }
    }

    private void Update()
    {
        if (target == null)
        {
            HandleNoTarget();
        }
        else
        {
            HandleHasTarget();
        }

        LandEventCheck(); // Kiểm tra sự kiện hạ cánh
    }

    // Xử lý khi không có mục tiêu
    private void HandleNoTarget()
    {
        animator.SetBool("isTargetExist", false);
        animator.SetFloat("Zombie_Speed", 0);
        rb2D.velocity = Vector2.zero;
        path.Clear();
        target = FindTarget();
    }

    // Xử lý khi đã có mục tiêu
    private void HandleHasTarget()
    {
        animator.SetBool("isTargetExist", true);

        if (!enemy.isUsingAbility)
        {
            animator.SetFloat("Zombie_Speed", enemy.OriginalSpeed);

            // Kiểm tra và cập nhật đường đi khi cần thiết
            if (!isPathUpdated)
            {
                FindPathToTarget();
                isPathUpdated = true;
            }

            if (path != null)
            {
                HandleMovement();
            }
            else
            {
                // Đếm thời gian kiểm tra lại đường đi
                checkPathTimer += Time.deltaTime;
                if (checkPathTimer >= checkPathInterval)
                {
                    FindPathToTarget();
                    checkPathTimer = 0f;
                }
            }
        }
        else
        {
            if (enemy.enemyStats.zombieClass == Enemy.EnemyStats.ZombieClass.Ranger)
            {
                StopAndResetPath();
            }
        }
    }

    // Xử lý di chuyển theo đường đi
    private void HandleMovement()
    {
        if ((enemy.DetectPlayer() || enemy.DetectBreakableDoor()) && isGrounded)
        {
            animator.SetBool("isNearTarget", true);
            StopAndResetPath();
        }
        else
        {
            MoveAlongPath();
        }
    }

    // Dừng và đặt lại trạng thái đường đi
    private void StopAndResetPath()
    {
        rb2D.velocity = Vector2.zero;
        path.Clear();
        isPathUpdated = false;
    }

    // Kiểm tra sự kiện hạ cánh
    private void LandEventCheck()
    {
        if (!wasGrounded && isGrounded)
        {
            OnLandEvent();
        }
        wasGrounded = isGrounded;
    }

    // Sự kiện khi hạ cánh
    private void OnLandEvent()
    {
        if (path != null)
        {
            path.Clear();
        }
        FindPathToTarget();
    }

    // Hàm điều chỉnh vị trí BodyPoint so với đáy của Collider2D
    private void AdjustBPAndGCPosition()
    {
        // Lấy Collider2D hiện tại của đối tượng
        Collider2D col = GetComponent<Collider2D>();

        if (col != null && GroundCheck != null)
        {
            // Tính vị trí trung tâm theo chiều X và đáy theo chiều Y của Collider2D
            Vector3 colliderBottomCenter = new Vector3(col.bounds.center.x, col.bounds.min.y, 0);

            // Thiết lập vị trí của GroundCheck dựa trên trung tâm theo chiều ngang và cao hơn đáy một khoảng
            GroundCheck.position = new Vector3(colliderBottomCenter.x, colliderBottomCenter.y - 0.01f, 0);
        }
        else
        {
            Debug.LogWarning("Collider hoặc BodyPoint chưa được thiết lập.");
        }
    }

    // Tìm đường đi tới mục tiêu
    private void FindPathToTarget()
    {
        if (pathfinder != null && target != null)
        {
            // Tìm đường đi từ vị trí hiện tại của đối tượng đến mục tiêu
            path = pathfinder.FindPath(GetComponent<Enemy>().GroundPosition, target.GetComponent<Player>().GroundPosition);
            if (path != null)
            {
                currentPathIndex = 0; // Đặt lại chỉ số đường đi về 0
                if (path.Count >= 2)
                {
                    prevNode = path[currentPathIndex];
                    targetNode = path[currentPathIndex + 1]; // Đặt node đầu tiên làm mục tiêu
                }
                else
                {
                    animator.SetFloat("Zombie_Speed", 0);
                }
            }
        }
    }

    // Di chuyển dọc theo đường đi
    private void MoveAlongPath()
    {
        animator.SetBool("isNearTarget", false);
        // Nếu đường đi còn tồn tại
        if (currentPathIndex < path.Count)
        {
            float distanceToTargetNode = Vector3.Distance(GetComponent<Enemy>().GroundPosition, targetNode);

            // Kiểm tra xem có đang gần node mục tiêu hay không
            if (distanceToTargetNode < padding)
            {
                timeToReachNode = 0f;
                path.Clear(); // Xóa đường đi
                FindPathToTarget(); // Tìm lại đường đi                 
            }
            else
            {
                //  Tính toán hướng di chuyển tới node tiếp theo
                Vector3 direction = (targetNode - GroundCheck.position).normalized;

                // Cập nhật trạng thái hướng mặt của nhân vật theo hướng di chuyển
                if ((direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight))
                {
                    Flip();
                }

                // Tính toán độ nghiêng của dốc
                float Angle = Vector2.Angle(Vector2.up, targetNode - prevNode);

                if (Angle == 90) // Không có dốc
                {
                    float adjustMoventSpeed = enemy.enemyStats.movementSpeed;
                    if (isGrounded == false)
                    {
                        adjustMoventSpeed = 10;
                    }
                    // Di chuyển ngang bình thường
                    Vector2 targetVelocity = new Vector2(direction.x * adjustMoventSpeed, rb2D.velocity.y);
                    rb2D.velocity = Vector2.SmoothDamp(rb2D.velocity, targetVelocity, ref smoothVelocity, 0.1f);
                }
                else if (Angle < 90) // Có dốc
                {
                    float adjustMoventSpeed = enemy.enemyStats.movementSpeed;
                    if (isGrounded == false)
                    {
                        adjustMoventSpeed = 10;
                    }
                    // Tính vận tốc theo hướng dốc
                    Vector2 targetVelocity = new Vector2(direction.x * adjustMoventSpeed, rb2D.velocity.y);
                    rb2D.velocity = Vector2.SmoothDamp(rb2D.velocity, targetVelocity, ref smoothVelocity, 0f);
                }

                // Tính toán lực nhảy dựa trên sự khác biệt về độ cao giữa prevNode và targetNode
                float heightDifference = targetNode.y - prevNode.y;
                if (heightDifference > 1.1f && isGrounded)
                {
                    DynamicJump(heightDifference, direction);
                }

                // Tăng thời gian di chuyển đến node
                timeToReachNode += Time.deltaTime;

                // Nếu thời gian di chuyển vượt quá 1 giây, cập nhật lại đường đi
                if (timeToReachNode >= maxTimeToReachNode)
                {
                    path.Clear();
                    FindPathToTarget();
                    timeToReachNode = 0f; // Đặt lại bộ đếm thời gian
                }

            }
        }
    }
    // Hàm nhảy có lực động dựa trên độ cao giữa các node
    private void DynamicJump(float heightDifference, Vector3 direction)
    {
        // Điều chỉnh lực nhảy dựa trên trọng lực và độ cao. 58.86 = 9.81 x 3 x 2, + lên 62 để nhảy cao hơn 1 xíu chứa không quá vừa đủ
        float calculatedJumpForce = (float)Math.Sqrt(62f * heightDifference);
        Debug.Log(calculatedJumpForce);
        // Áp dụng lực nhảy
        rb2D.velocity = new Vector2(rb2D.velocity.x, calculatedJumpForce);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight; // Đảo ngược trạng thái isFacingRight
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Đảo ngược hướng X của đối tượng
        transform.localScale = localScale;
    }

    // Kiểm tra xem đối tượng có đang trên mặt đất không
    private void CheckIfGrounded()
    {
        // Nếu đã gán groundCheck, sử dụng vị trí của nó để kiểm tra mặt đất
        if (GroundCheck != null)
        {
            // Kiểm tra nếu có đối tượng thuộc layer ground trong bán kính groundCheckRadius
            isGrounded = Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, groundLayer);
        }
    }

    private void stopMoving()
    {
        Vector2 targetVelocity = new Vector2(0f, rb2D.velocity.y);
        rb2D.velocity = Vector2.SmoothDamp(rb2D.velocity, targetVelocity, ref smoothVelocity, 0f);
    }
    void FixedUpdate()
    {
        CheckIfGrounded();
    }

    void OnDrawGizmos()
    {
        // Vẽ đường đi nếu có

        if (path != null)
        {
            // for (int i = 0; i < path.Count - 1; i++)
            // {
            //     Gizmos.DrawLine(path[i], path[i + 1]);
            // }

            // Vẽ điểm bắt đầu và điểm kết thúc
            // Gizmos.color = Color.blue; // Màu của điểm bắt đầu
            // Gizmos.DrawSphere(BodyPoint.position, 0.1f); // Vẽ điểm bắt đầu

            // Gizmos.color = Color.red; // Màu của điểm kết thúc
            // Gizmos.DrawSphere(target.position, 0.1f); // Vẽ điểm kết thúc
        }
        // Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(targetNode, 0.15f);
        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(prevNode, 0.15f);
    }

    // Hàm để thiết lập mục tiêu di chuyển
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        isPathUpdated = false; // Khi mục tiêu thay đổi, cho phép cập nhật lại path
        FindPathToTarget();
    }

    public void AnimationAction()
    {
        animator.SetFloat("Zombie_Speed", enemy.enemyStats.movementSpeed);

    }

    private void setupAnimationParameter_zombieClass()
    {
        if (enemy.enemyStats.zombieClass == Enemy.EnemyStats.ZombieClass.Walker)
        {
            animator.SetBool("isWalker", true);

        }
        if (enemy.enemyStats.zombieClass == Enemy.EnemyStats.ZombieClass.Runner)
        {
            animator.SetBool("isRunner", true);

        }
        if (enemy.enemyStats.zombieClass == Enemy.EnemyStats.ZombieClass.Tanker)
        {
            animator.SetBool("isTanker", true);

        }
        if (enemy.enemyStats.zombieClass == Enemy.EnemyStats.ZombieClass.Juggernaut)
        {
            animator.SetBool("isJuggernaut", true);

        }
        if (enemy.enemyStats.zombieClass == Enemy.EnemyStats.ZombieClass.Ranger)
        {
            animator.SetBool("isRanger", true);

        }
    }
}
