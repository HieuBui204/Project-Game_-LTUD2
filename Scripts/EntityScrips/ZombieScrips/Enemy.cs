using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [System.Serializable]
    public class EnemyStats
    {
        public float maxHealth = 100;
        public float damage = 10; // Giá trị sát thương
        public float abilityDamage = 20; // Giá trị sát thương
        public float attackCooldown = 1; // Thời gian cooldown giữa các lần tấn công
        public float lastAttackTime = 0; // Thời điểm lần tấn công cuối
        public float movementSpeed = 8f; // Thời điểm lần tấn công cuối
        public float abilityCD;
        public float abilityRange;
        public enum ZombieClass
        {
            Walker,
            Runner,
            Tanker,
            Juggernaut,
            Ranger
        }
        public ZombieClass zombieClass;
        private float _curHealth;
        public float curHealth
        {
            get { return _curHealth; }
            set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        public void Init()
        {
            curHealth = maxHealth;
        }
    }

    public EnemyStats enemyStats = new EnemyStats();

    [Header("Optional: ")]
    [SerializeField]
    private StatusIndicator statusIndicator;

    [Header("PowerUpsDrop")]
    [SerializeField]
    private GameObject[] powerUpPrefabs; // Mảng prefab cho PowerUps
    [SerializeField]
    private float dropChance = 0.3f; // Tỷ lệ drop cho PowerUp (30%)

    [Header("PlayerDetect")]
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private LayerMask breakableLayer;
    [SerializeField]
    private float detectRadius;
    private GameObject detectedPlayer;
    private GameObject detectedBreakableDoor;
    private bool isAttacking = false;
    private bool isSlowed = false;
    private Coroutine slowCoroutine;
    private Coroutine abilityCoroutine;
    private float originalSpeed; // Lưu tốc độ ban đầu để tránh giảm dồn
    public float OriginalSpeed
    {
        get { return originalSpeed; }
        set { originalSpeed = value; }
    }
    [SerializeField]
    public bool isUsingAbility = false;
    private float maxAbilityDuration = 0.7f;
    private float lastTimeAbilityUsed;
    public bool isDead = false;
    private Vector3 _groundPosition;
    public Vector3 GroundPosition
    {
        get { return _groundPosition; }
        set { _groundPosition = value; }
    }

    [SerializeField]
    private GameObject projectilePrefab; // Prefab for the thrown object
    // [SerializeField]
    // private float throwForce = 100f; // Force applied to the projectile

    [Header("Blood Effect")]
    [SerializeField]
    private GameObject bloodParticlePrefab; // Prefab của hiệu ứng máu
    public GameObject floatingDamage; // Prefab hiệu ứng số mau
    private void Start()
    {
        enemyStats.Init();

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(enemyStats.curHealth, enemyStats.maxHealth);
        }

        originalSpeed = this.enemyStats.movementSpeed;
        lastTimeAbilityUsed = 0;
    }

    private void SetGroundPosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1000f, LayerMask.GetMask("Platform"));
        _groundPosition = hit.point;
    }

    private void Update()
    {
        SetGroundPosition();
        // Chỉ bắt đầu tấn công nếu không đang tấn công
        if (!isAttacking)
        {
            if ((DetectPlayer() || DetectBreakableDoor()) && !isUsingAbility)
            {
                AttackTarget();
            }
        }
        if (DetectPlayer() && isUsingAbility)
        {
            InstantTargetDamged();
        }

        if (enemyStats.zombieClass == EnemyStats.ZombieClass.Juggernaut)
        {
            if (Time.time - lastTimeAbilityUsed >= enemyStats.abilityCD)
            {
                if (DetectConditionUseAbility())
                {
                    abilityCoroutine = StartCoroutine(Ability_Juggernaut_Charggggge());
                    enemyStats.abilityCD = 30;
                    lastTimeAbilityUsed = Time.time;
                }
            }
        }

        if (enemyStats.zombieClass == EnemyStats.ZombieClass.Ranger)
        {
            if (Time.time - lastTimeAbilityUsed >= enemyStats.abilityCD)
            {
                if (DetectConditionUseAbility())
                {
                    isUsingAbility = true;
                    GetComponent<Animator>().SetBool("isTargetInRange", true);
                    GetComponent<Animator>().SetTrigger("isUseAbility");
                    enemyStats.abilityCD = 2;
                    lastTimeAbilityUsed = Time.time;
                }
                else
                {
                    isUsingAbility = false;
                    GetComponent<Animator>().SetBool("isTargetInRange", false);
                    enemyStats.movementSpeed = originalSpeed;
                }
            }
        }
    }

    public void AttackTarget()
    {
        if (Time.time >= enemyStats.lastAttackTime + enemyStats.attackCooldown)
        {
            GetComponent<Animator>().SetTrigger("Attack");
            enemyStats.lastAttackTime = Time.time;
        }
    }

    public void InstantTargetDamged()
    {
        Player player = detectedPlayer.GetComponent<Player>();
        player.DamagePlayer(enemyStats.damage);
    }

    // Phương thức này được gọi khi animation tấn công kết thúc
    public void EndAttackAnimation()
    {
        // Kiểm tra xem target có còn trong tầm đánh hay không
        if (DetectPlayer())
        {
            Player player = detectedPlayer.GetComponent<Player>();
            player.DamagePlayer(enemyStats.damage);
        }
        else if (DetectBreakableDoor())
        {
            Breakable door = detectedBreakableDoor.GetComponent<Breakable>();
            door.DamageBreakable();
            // Debug.Log("current door's health: " + door.breakableStats.curHealth);
        }
        isAttacking = false;
    }

    public void DamageEnemy(float inputdamage)
    {
        enemyStats.curHealth -= inputdamage;
        // // Gọi hiệu ứng máu
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, -10f);
        SpawnBloodEffect(spawnPosition);
        ShowFloatingDamage(inputdamage, new Vector3(transform.position.x, transform.position.y, -transform.position.z));
        if (enemyStats.curHealth <= 0)
        {
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            if (rb2D != null)
            {
                rb2D.constraints = RigidbodyConstraints2D.FreezePositionX;
            }
            isDead = true;
            GetComponent<Animator>().SetBool("isDead", isDead);
            int number = Random.Range(0, 3);
            GetComponent<Animator>().SetTrigger("Death_" + number);
        }
        else
        {
            if (!isSlowed)
            {
                slowCoroutine = StartCoroutine(ApplyTemporarySlow());
            }
            else
            {
                // Làm mới thời gian slow nếu bị gây sát thương thêm
                StopCoroutine(slowCoroutine);
                slowCoroutine = StartCoroutine(ApplyTemporarySlow());
            }
        }

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(enemyStats.curHealth, enemyStats.maxHealth);
        }
    }

    public void afterDeadEvent()
    {
        GameMaster.EnqueueKill();
        Destroy(this.gameObject);
        DropPowerUp();
    }

    // Coroutine để giảm tốc độ di chuyển tạm thời
    private IEnumerator ApplyTemporarySlow()
    {
        isSlowed = true;
        enemyStats.movementSpeed = originalSpeed * 0.7f; // Giảm 30%

        yield return new WaitForSeconds(2); // Giữ giảm tốc độ trong 2 giây

        enemyStats.movementSpeed = originalSpeed; // Khôi phục tốc độ ban đầu
        isSlowed = false;
    }

    public bool DetectPlayer()
    {
        Collider2D obj = Physics2D.OverlapCircle(transform.position, detectRadius, playerLayer);
        if (obj != null)
        {
            detectedPlayer = obj.gameObject;
            return true;

        }
        detectedPlayer = null;
        return false;
    }

    public bool DetectConditionUseAbility()
    {
        bool isFacingRight = GetComponent<EnemyMovementAI>().isFacingRight;

        // Determine the direction of the raycast based on isFacingRight
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;

        // Fire a raycast from the center of the enemy in the specified direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, enemyStats.abilityRange, playerLayer);

        // Check if the raycast hit the player
        if (hit.collider != null)
        {
            Debug.Log("Player detected within ability range!");
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        // Draw a sphere at groundPosition if it has been set (not zero)
        if ((Vector2)_groundPosition != Vector2.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_groundPosition, 0.2f); // Adjust size as needed
        }
    }

    private IEnumerator Ability_Juggernaut_Charggggge()
    {
        Animator animator = GetComponent<Animator>();
        isUsingAbility = true;
        animator.SetBool("isUsingAbility", isUsingAbility);
        enemyStats.movementSpeed = 0;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        bool isFacingRight = GetComponent<EnemyMovementAI>().isFacingRight;
        float chargeForce = 20f; // Adjust this value for how strong the charge should be

        // Determine the direction of the charge
        Vector2 chargeDirection = isFacingRight ? Vector2.right : Vector2.left;

        // Apply force to the Rigidbody2D in the charge direction
        yield return new WaitForSeconds(0.5f);
        rb.AddForce(chargeDirection * chargeForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(maxAbilityDuration);
        // Khôi phục tốc độ ban đầu
        isUsingAbility = false;
        animator.SetBool("isUsingAbility", isUsingAbility);
        enemyStats.movementSpeed = originalSpeed;
    }

    private void EndAbility_Ranger_ThrowAnimation()
    {
        // if (projectilePrefab == null) yield break;
        if (projectilePrefab != null)
        {
            // Capture the player's position at the time of throwing
            Vector2 targetPosition = (Vector2)GetComponent<EnemyMovementAI>().target.transform.position;
            Vector2 throwPosition = transform.position;
            float gravity = Mathf.Abs(Physics2D.gravity.y);

            // Calculate the horizontal distance and height difference to the target
            float distance = Vector2.Distance(throwPosition, targetPosition);
            float heightDifference = targetPosition.y - throwPosition.y;

            // Set an angle (in degrees) for the throw, e.g., 45 degrees for a nice arc
            float throwAngle = 45f;
            float angleInRadians = throwAngle * Mathf.Deg2Rad;

            // Calculate the initial speed required to reach the target
            float speed = Mathf.Sqrt((distance * gravity) / (Mathf.Sin(2 * angleInRadians) + (2 * heightDifference * Mathf.Cos(angleInRadians) * Mathf.Sin(angleInRadians)) / distance));

            // Determine the direction of the throw based on isFacingRight
            bool isFacingRight = GetComponent<EnemyMovementAI>().isFacingRight;
            Vector2 direction = (isFacingRight ? Vector2.right : Vector2.left);

            // Calculate the initial velocity components
            Vector2 initialVelocity = new Vector2(
                direction.x * speed * Mathf.Cos(angleInRadians),
                speed * Mathf.Sin(angleInRadians)
            );

            // Instantiate the projectile
            GameObject projectile = Instantiate(projectilePrefab, throwPosition, Quaternion.identity);

            // Get the Rigidbody2D component of the projectile
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.gravityScale = 1;

            // Apply the calculated initial velocity to the projectile
            rb.velocity = initialVelocity;

            // yield return new WaitForSeconds(maxAbilityDuration);
        }
    }

    public bool DetectBreakableDoor()
    {
        Collider2D obj = Physics2D.OverlapCircle(transform.position, detectRadius, breakableLayer);
        if (obj != null)
        {
            if (obj.GetComponent<Breakable>().breakableType == Breakable.BreakableType.Door && obj.GetComponent<Breakable>().breakableStats.curHealth > 0)
            {
                detectedBreakableDoor = obj.gameObject;
                return true;
            }
        }
        detectedBreakableDoor = null;
        return false;
    }

    // Vẽ Gizmo để hiển thị bán kính phát hiện
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Màu của Gizmo
        Gizmos.DrawWireSphere(transform.position, detectRadius); // Vẽ vòng tròn xung quanh enemy
    }

    private void DropPowerUp()
    {
        // Kiểm tra xem có PowerUps không
        if (powerUpPrefabs.Length > 0)
        {
            // Tạo một giá trị ngẫu nhiên từ 0 đến 1
            float randomValue = Random.Range(0f, 1f);

            // Kiểm tra xác suất drop
            if (randomValue <= dropChance) // Nếu giá trị ngẫu nhiên nhỏ hơn hoặc bằng tỷ lệ drop
            {
                // Chọn ngẫu nhiên một PowerUp từ mảng
                int randomIndex = Random.Range(0, powerUpPrefabs.Length);
                GameObject powerUpToDrop = powerUpPrefabs[randomIndex];

                // Tạo một vị trí drop ngẫu nhiên xung quanh enemy
                Vector3 dropPosition = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                Instantiate(powerUpToDrop, dropPosition, Quaternion.identity); // Khởi tạo PowerUp tại vị trí drop
            }
        }
    }

    public void SpawnBloodEffect(Vector3 position)
    {
        if (bloodParticlePrefab != null)
        {
            Instantiate(bloodParticlePrefab, position, Quaternion.identity);
        }
    }

    private void ShowFloatingDamage(float damage, Vector3 spawnPosition)
    {
        if (floatingDamage != null)
        {
            // Instantiate đối tượng nhưng không có parent
            GameObject floatingNumber = Instantiate(floatingDamage, spawnPosition, Quaternion.identity);

            // Lấy script điều khiển hiệu ứng số
            FloatingNumber floatingNumberScript = floatingNumber.GetComponent<FloatingNumber>();
            if (floatingNumberScript != null)
            {
                floatingNumberScript.SetText("- " + damage.ToString());
            }
        }
        else
        {
            Debug.LogWarning("FloatingNumberPrefab chưa được thiết lập!");
        }
    }

}
