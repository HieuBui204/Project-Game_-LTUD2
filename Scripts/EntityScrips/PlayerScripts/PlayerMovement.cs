using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;
    public Animator animator;
    private Transform armContainer;
    private PlayerStatusIndicator playerIndicator;

    [Header("Stamina Settings")]
    private float curStamina;
    private bool isSprinting = false;
    private Coroutine staminaCoroutine;
    private Coroutine recoveryCoroutine;

    [Header("Crouch Settings")]
    public BoxCollider2D CrouchDisableCollider;
    private bool crouch = false;

    [Header("Ground Check")]
    public Transform GroundCheck;
    public Transform CeilingCheck;
    public float CheckRadius = 0.1f;
    public LayerMask groundLayer;
    private bool isGrounded = false;

    private Rigidbody2D rb2D;
    private bool jump = false;
    private float horizontalMove;
    private void Start()
    {
        player = GetComponent<Player>();
        armContainer = transform.Find("Body_Container/Arm_Container");
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.gravityScale = player.playerStats.gravity;

        // Cache PlayerStatusIndicator
        GameObject playerStatsUI = GameObject.FindWithTag("PlayerStatsUI");
        if (playerStatsUI != null)
        {
            playerIndicator = playerStatsUI.GetComponent<PlayerStatusIndicator>();
            if (playerIndicator == null)
            {
                Debug.LogError("Không tìm thấy PlayerStatusIndicator trên PlayerStatsUI!");
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy GameObject với Tag 'PlayerStatsUI'!");
        }

        curStamina = player.playerStats.maxStamina;

        // Kiểm tra GroundCheck và CeilingCheck
        if (CeilingCheck == null || GroundCheck == null)
        {
            Debug.LogWarning("GroundCheck hoặc CeilingCheck chưa được gán trong Inspector.");
        }
    }

    void Update()
    {
        HandleInput();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        CheckIfGrounded();
        Move();
    }
    
    private void HandleInput()
    {
        // Di chuyển ngang
        float moveInput = Input.GetAxisRaw("Horizontal");

        horizontalMove = moveInput * player.playerStats.movementSpeed;

        // Nhảy
        if (Input.GetButtonDown("Jump") && isGrounded && !crouch)
        {
            jump = true;
        }

        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouch = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            crouch = false;
        }

        // Sprinting
        if (Input.GetButton("Sprint") && Mathf.Abs(moveInput) > 0.1f && curStamina > 0)
        {
            if (!isSprinting)
            {
                if (recoveryCoroutine != null)
                {
                    StopCoroutine(recoveryCoroutine);
                }
                staminaCoroutine = StartCoroutine(DrainStamina());
                isSprinting = true;
                SwitchWeaponComponents(false); // Disable shooting
                ArmRotation armRotation = armContainer.GetComponent<ArmRotation>();
                if (armRotation != null)
                {
                    armRotation.ResetArmContainerRotation();
                    armRotation.enabled = false;
                }
            }
            horizontalMove = moveInput * player.playerStats.movementSpeed * 2f * player.playerStats.speedMultiplier;
        }
        else
        {
            if (isSprinting)
            {
                StopCoroutine(staminaCoroutine);
                recoveryCoroutine = StartCoroutine(RecoverStamina());
                isSprinting = false;
                SwitchWeaponComponents(true); // Enable shooting
                ArmRotation armRotation = armContainer.GetComponent<ArmRotation>();
                if (armRotation != null)
                {
                    armRotation.enabled = true;
                }
            }
            horizontalMove = moveInput * player.playerStats.movementSpeed * player.playerStats.speedMultiplier;
        }
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("Player_Speed", Mathf.Abs(horizontalMove));
        animator.SetBool("Player_Crouch", crouch);
    }

    private void Move()
    {
        // Crouch Logic
        bool effectiveCrouch = crouch;

        if (!crouch)
        {
            // Tự crouch nếu có chướng ngại vật phía trên
            if (Physics2D.OverlapCircle(CeilingCheck.position, CheckRadius, groundLayer) && isGrounded)
            {
                effectiveCrouch = true;
            }
        }

        if (effectiveCrouch)
        {
            horizontalMove *= player.playerStats.crouchSpeed;
            if (CrouchDisableCollider != null)
                CrouchDisableCollider.enabled = false;
            animator.SetBool("Player_Crouch", true);
        }
        else
        {
            if (CrouchDisableCollider != null)
                CrouchDisableCollider.enabled = true;
            animator.SetBool("Player_Crouch", false);
        }

        // Giới hạn vận tốc
        float clampedMove = Mathf.Clamp(horizontalMove * 10f, -player.playerStats.maxSpeed, player.playerStats.maxSpeed);
        Vector2 targetVelocity = new Vector2(clampedMove, rb2D.velocity.y);
        rb2D.velocity = targetVelocity;

        // Nhảy
        if (jump && isGrounded && !effectiveCrouch)
        {
            isGrounded = false;
            rb2D.AddForce(new Vector2(0f, player.playerStats.jumpForce), ForceMode2D.Impulse);
            jump = false;
        }
    }

    private IEnumerator DrainStamina()
    {
        while (curStamina > 0)
        {
            curStamina -= 1;
            if (playerIndicator != null)
            {
                playerIndicator.SetStamina(curStamina, player.playerStats.maxStamina);
            }

            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Stamina hết, ngừng chạy");
    }

    private IEnumerator RecoverStamina()
    {
        while (curStamina < player.playerStats.maxStamina)
        {
            curStamina += 2;
            if (playerIndicator != null)
            {
                playerIndicator.SetStamina(curStamina, player.playerStats.maxStamina);
            }

            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Stamina đã đầy");
    }

    private void SwitchWeaponComponents(bool state)
    {
        Transform guns = armContainer.Find("Guns");
        if (guns == null)
        {
            Debug.LogWarning("Không tìm thấy Transform 'Guns' trong Arm_Container!");
            return;
        }

        Weapon[] weaponComponents = guns.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weaponComponents)
        {
            weapon.enabled = state;
        }
    }

    private void CheckIfGrounded()
    {
        if (GroundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(GroundCheck.position, CheckRadius, groundLayer);
            Debug.DrawRay(GroundCheck.position, Vector2.down * CheckRadius, isGrounded ? Color.green : Color.red);
        }
        else
        {
            Debug.LogWarning("GroundCheck chưa được gán. Vui lòng gán GameObject GroundCheck trong Inspector.");
        }
    }

    // Nếu cần thiết, thêm phương thức AdjustCCAndGCPosition() để điều chỉnh vị trí GroundCheck và CeilingCheck dựa trên Collider
}
