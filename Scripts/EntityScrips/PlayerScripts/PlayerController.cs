using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float detectRadius = 1.4f;
    [SerializeField]
    private LayerMask breakableLayer;
    [SerializeField]
    private GameObject detectedBreakableDoor;
    public GameObject detectedRemoveableDoor;
    public LayerMask detectionLayer;
    public GameObject detectedObject;
    private Player player;
    private ArmRotation armRotation;
    private Animator animator;
    public Weapon weapon;

    void Awake()
    {
        player = GetComponent<Player>();
        Transform armContainer = GameObject.Find("Arm_Container").transform;
        armRotation = armContainer.GetComponent<ArmRotation>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        getWeapon();
        DetectObject();
        DetectBreakable();
        Interact();
        HandleBreakable();
        WeaponHandler();
        KnifeAtttack();
        HandleThrow();
    }
    private void getWeapon()
    {
        GameObject guns = GameObject.Find("Guns");
        foreach (Transform item in guns.transform)
        {
            if (item.GetComponent<Weapon>().gunId == player.playerStats.isHolding)
            {
                weapon = item.GetComponent<Weapon>();
                break;
            }
        }
    }
    private void HandleBreakable()
    {
        if (Input.GetKeyDown(KeyCode.F) && DetectBreakable())
        {
            // Fix cửa
            if (detectedBreakableDoor != null)
            {
                Breakable breakable = detectedBreakableDoor.GetComponent<Breakable>();
                breakable.Fix_Door();
            }

            // Open new way
            if (detectedRemoveableDoor != null)
            {
                Breakable breakable = detectedRemoveableDoor.GetComponent<Breakable>();
                breakable.BuyRemoveable(player);
            }
        }
    }

    private void Interact()
    {
        // Tương tác với Item hoặc Perk
        if (Input.GetKeyDown(KeyCode.B) && DetectObject())
        {
            if (detectedObject != null)
            {
                Item itemComponent = detectedObject.GetComponent<Item>();
                PerkController perkController = detectedObject.GetComponent<PerkController>();
                MysteryBox mysterybox = detectedObject.GetComponent<MysteryBox>();

                if (itemComponent != null)
                {
                    itemComponent.Interact(gameObject);
                }
                else if (perkController != null)
                {
                    perkController.Interact(gameObject);
                }
                else if (mysterybox != null)
                {
                    mysterybox.Interact(gameObject);
                }
            }
        }
    }

    bool DetectObject()
    {
        Collider2D obj = Physics2D.OverlapCircle(transform.position, detectRadius, detectionLayer);
        if (obj == null)
        {
            detectedObject = null;
            return false;
        }
        else
        {
            detectedObject = obj.gameObject;
            return true;
        }
    }

    public bool DetectBreakable()
    {
        Collider2D obj = Physics2D.OverlapCircle(transform.position, detectRadius, breakableLayer);
        if (obj != null && obj.GetComponent<Breakable>().breakableType == Breakable.BreakableType.Door)
        {
            detectedBreakableDoor = obj.gameObject;
            return true;
        }
        else
        {
            detectedBreakableDoor = null;
        }

        if (obj != null && obj.GetComponent<Breakable>().breakableType == Breakable.BreakableType.Removeable)
        {
            detectedRemoveableDoor = obj.gameObject;
            return true;
        }
        else
        {
            detectedRemoveableDoor = null;
        }

        return false;
    }

    private void WeaponHandler()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        weapon.UpdateBodyPointColliderRadius();
        weapon.SwitchGunMode();
        if (weapon.weaponType == Weapon.WeaponType.Shotgun)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                weapon.ShootShotgun(mousePosition);
            }
        }
        else
            if (!weapon.autoMode)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                weapon.Shoot(mousePosition);
            }
        }
        else if (weapon.autoMode)
        {
            if (Input.GetButton("Fire1") && Time.time > weapon.timeToFire)
            {
                weapon.timeToFire = Time.time + 1 / weapon.fireRate;
                weapon.Shoot(mousePosition);

            }
        }

        // Call the reloadAmmo method in Update so that it listens for the reload keypress
        if (Input.GetKeyDown(KeyCode.R) && !weapon.isReloading)
        {
            StartCoroutine(weapon.ReloadAmmo(armRotation, animator));
        }

        weapon.changePistolTexture_OutOfAmmo();
        weapon.updateWeaponUI();
    }

    private void KnifeAtttack()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            weapon.shootable = false;
            armRotation.enabled = false;
            GameObject guns = GameObject.Find("Guns");
            foreach (Transform gun in guns.transform)
            {
                if (gun.GetComponent<Weapon>().gunId == weapon.gunId)
                {
                    gun.gameObject.SetActive(false);
                    break;
                }
            }
            animator.SetTrigger("Player_KnifeAttack");
        }
    }

    public void KnifeAttackAnimation_01()
    {
        GameObject knifeArm = GameObject.Find("right_arm_down");

        foreach (Transform knife in knifeArm.transform)
        {
            if (knife != null)
            {
                knife.gameObject.SetActive(true);
            }
        }

    }

    public void KnifeAttackAnimation_02()
    {
        GameObject knifeArm = GameObject.Find("right_arm_down");

        foreach (Transform knife in knifeArm.transform)
        {
            knife.gameObject.SetActive(false);
        }

        GetComponent<GunPose>().ChangePlayerArmsPose(weapon.gunId);

        weapon.shootable = true;
        armRotation.enabled = true;
    }

    public void KnifeDamaging()
    {
        GameObject knifeArm = GameObject.Find("right_arm_down");
        foreach (Transform knife in knifeArm.transform)
        {
            // Lấy CircleCollider2D từ knife
            CircleCollider2D knifeCollider = knife.GetComponent<CircleCollider2D>();
            if (knifeCollider == null)
            {
                Debug.LogError("Knife does not have a CircleCollider2D!");
                return;
            }

            // Lấy vị trí tâm và bán kính trực tiếp từ collider
            Vector2 circlePosition = knifeCollider.bounds.center;
            float circleRadius = knifeCollider.radius;

            // Lọc theo layer "enemy"
            LayerMask enemyLayer = LayerMask.GetMask("Enemy");

            // Kiểm tra collider trong vùng OverlapCircle
            Collider2D hittedTarget = Physics2D.OverlapCircle(circlePosition, circleRadius, enemyLayer);

            if (hittedTarget != null)
            {
                hittedTarget.GetComponent<Enemy>().DamageEnemy(50);
            }
        }
    }

    public void ReloadAnimation_00()
    {
        weapon.ReloadAnimation_00();
    }

    public void ReloadAnimation_01()
    {
        weapon.ReloadAnimation_01();
    }

    private void HandleThrow()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            this.GetComponent<GunPose>().DeActiveGun();
            weapon.shootable = false;
            GetComponent<Animator>().SetTrigger("isPrepareThrow");
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            this.GetComponent<GunPose>().DeActiveGun();
            weapon.shootable = false;
            GetComponent<Animator>().SetTrigger("isThrowing");
        }
    }

    public void EndPrepareThrowAnimation()
    {
        GetComponent<Animator>().SetBool("inPrepareThrowingPose", true);
    }

    public void EndThrowingAnimation()
    {
        GetComponent<Launcher>().LaunchGrenade();
        GetComponent<Animator>().SetBool("inPrepareThrowingPose", false);
        this.GetComponent<GunPose>().ChangePlayerArmsPose(player.playerStats.isHolding);
        weapon.shootable = true;
    }
}
