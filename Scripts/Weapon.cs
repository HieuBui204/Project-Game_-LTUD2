using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

[RequireComponent(typeof(MuzzleFlashHandler))]
[RequireComponent(typeof(BulletHandler))]
public class Weapon : MonoBehaviour
{
    public string gunId;
    public string gunName;
    public float fireRate;
    public bool autoMode = false;
    public float damage;
    public float pointPerHit;
    public float range;
    public float ammoPerRound;
    public int maxRound = 0;
    private float subAmmo;
    private float curAmmo;
    public bool isRoundEmpty;
    public float reloadTime;
    public bool isReloading = false;
    public float maxRecoil;  // Độ giật súng tối đa
    public float recoilCooldown; // Thời gian hổi tâm
    private float currentRecoil = 0f;  // Độ giật hiện tại
    private float recoilResetTime = 0f;// Thời gian của viên đạn cuối cùng được bắn
    public LayerMask whatToHit;
    private MuzzleFlashHandler muzzleFlashHandler;
    private BulletHandler bulletTrailHandler;
    private EjectCasing2D ejectCasing;
    public enum WeaponType { Pistol, Rifle, Shotgun, Sniper_Rifle, Smg, Mg };
    public WeaponType weaponType;
    public int splitShot = 0;
    public float timeToFire = 0;
    Transform firePoint;
    private Transform bodyPoint;
    private CircleCollider2D bodyPointCollider;
    private GameObject withMagAndAmmoTexture;
    private GameObject outOfAmmoTexture;
    private GameObject magTexture;
    private GameObject singleBulletTexture;
    private WeaponIndicator weaponIndicator;
    public bool shootable = true;

    void Awake()
    {
        muzzleFlashHandler = this.GetComponent<MuzzleFlashHandler>();
        bulletTrailHandler = this.GetComponent<BulletHandler>();
        ejectCasing = this.GetComponent<EjectCasing2D>();

        setUpTextureComponent();

        curAmmo = ammoPerRound;

        subAmmoSetup();

        bodyPoint = transform.parent;
        firePoint = transform.Find("FirePoint");

        if (firePoint == null)
        {
            Debug.LogError("No firePoint? WHAT?!");
        }

        if (muzzleFlashHandler == null)
        {
            Debug.LogError("No MuzzleFlashHandler found.");
        }

        if (bulletTrailHandler == null)
        {
            Debug.LogError("No BulletTrailHandler found.");
        }

        bodyPointCollider = bodyPoint.GetComponent<CircleCollider2D>();
        if (bodyPointCollider == null)
        {
            Debug.LogError("No CircleCollider2D found on bodyPoint.");
        }

        GameObject gunStatsUI = GameObject.FindWithTag("GunStatsUI");
        if (gunStatsUI != null)
        {
            weaponIndicator = gunStatsUI.GetComponent<WeaponIndicator>();
        }
        else
        {
            Debug.LogError("No.");
        }

        updateWeaponUI();
    }

    public void setUpTextureComponent()
    {
        if (weaponType == WeaponType.Pistol)
        {
            withMagAndAmmoTexture = GameObject.Find(gunName + "_0");
            magTexture = GameObject.Find(gunName + "_1");
            outOfAmmoTexture = GameObject.Find(gunName + "_2");
        }
        else
        if (weaponType == WeaponType.Shotgun)
        {
            withMagAndAmmoTexture = GameObject.Find(gunName + "_0");
        }
        else
        {
            withMagAndAmmoTexture = GameObject.Find(gunName + "_0");
            magTexture = GameObject.Find(gunName + "_1");
        }
    }
    public void subAmmoSetup()
    {
        if (maxRound == 0)
        {
            if (weaponType == WeaponType.Rifle)
            {
                subAmmo = ammoPerRound * 10;
            }

            if (weaponType == WeaponType.Sniper_Rifle)
            {
                subAmmo = ammoPerRound * 7;
            }

            if (weaponType == WeaponType.Shotgun)
            {
                subAmmo = ammoPerRound * 7;
            }

            if (weaponType == WeaponType.Mg)
            {
                subAmmo = ammoPerRound * 7;
            }

            if (weaponType == WeaponType.Pistol)
            {
                subAmmo = ammoPerRound * 13;
            }

            if (weaponType == WeaponType.Smg)
            {
                subAmmo = ammoPerRound * 13;
            }
        }
        else
        {
            if (weaponType == WeaponType.Rifle)
            {
                subAmmo = ammoPerRound * maxRound;
            }

            if (weaponType == WeaponType.Sniper_Rifle)
            {
                subAmmo = ammoPerRound * maxRound;
            }

            if (weaponType == WeaponType.Shotgun)
            {
                subAmmo = ammoPerRound * maxRound;
            }

            if (weaponType == WeaponType.Mg)
            {
                subAmmo = ammoPerRound * maxRound;
            }

            if (weaponType == WeaponType.Pistol)
            {
                subAmmo = ammoPerRound * maxRound;
            }

            if (weaponType == WeaponType.Smg)
            {
                subAmmo = ammoPerRound * maxRound;
            }
        }
    }

    public void SwitchGunMode()
    {
        if (weaponType == WeaponType.Sniper_Rifle || weaponType == WeaponType.Shotgun)
        {
            autoMode = true;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                autoMode = !autoMode;
            }
        }
    }

    // Hàm bắn cho shotgun với splitShot
    public void ShootShotgun(Vector3 mousePosition)
    {
        if (shootable == true)
        {
            if (curAmmo != 0)
            {
                if (Time.time > recoilResetTime)
                {
                    ResetRecoil();
                }

                // Bắn nhiều viên đạn dựa trên số lượng splitShot
                for (int i = 0; i < splitShot; i++)
                {
                    Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

                    bool isMouseInCollider = bodyPointCollider.OverlapPoint(mousePosition2D);
                    Vector3 direction;

                    if (isMouseInCollider)
                    {
                        direction = -(mousePosition - firePoint.position).normalized;
                    }
                    else
                    {
                        direction = (mousePosition - firePoint.position).normalized;
                    }

                    // Thêm Muzzle Flash
                    muzzleFlashHandler.ShowMuzzleFlash();

                    // Tính toán điểm cuối mới với recoil
                    Vector3 endPoint = ApplyRecoil(direction, range);

                    // Di chuyển vệt đạn với điểm cuối mới
                    bulletTrailHandler.MoveTrail(this, firePoint.position, endPoint, whatToHit);
                }

                curAmmo -= 1;
                recoilResetTime = Time.time + recoilCooldown;
            }
            else
            {
                Debug.Log("Out of ammo");
                ResetRecoil();
            }
        }
    }

    // Hàm bắn
    public void Shoot(Vector3 mousePosition)
    {
        if (shootable == true)
        {
            if (curAmmo != 0)
            {
                GameObject.Find("Player").GetComponent<Animator>().SetTrigger("isShooting");

                if (Time.time > recoilResetTime)
                {
                    ResetRecoil();
                }

                // Nếu bắn sau khi đã hồi tâm, reset recoil
                if (Time.time > recoilResetTime)
                {
                    currentRecoil = 0f;
                }
                else
                {
                    // Tăng recoil sau mỗi viên bắn nếu giữ chuột
                    currentRecoil = Mathf.Clamp(currentRecoil + maxRecoil / ammoPerRound, 0, maxRecoil); // Giới hạn recoil tối đa
                }
                // Chuyển đổi mousePosition sang không gian 2D
                Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

                // Kiểm tra xem vị trí chuột có nằm trong collider của bodyPoint không
                bool isMouseInCollider = bodyPointCollider.OverlapPoint(mousePosition2D);
                Vector3 direction;

                if (isMouseInCollider)
                {
                    direction = -(mousePosition - firePoint.position).normalized;
                }
                else
                {
                    direction = (mousePosition - firePoint.position).normalized;
                }

                // Thêm Muzzle Flash
                muzzleFlashHandler.ShowMuzzleFlash();

                if (ejectCasing != null)
                {
                    Debug.Log("Calling Eject()");
                    ejectCasing.Eject();
                }
                else
                {
                    Debug.Log("ejectCasing is null!");
                }

                // Tính toán điểm cuối mới với recoil
                Vector3 endPoint = ApplyRecoil(direction, range);

                // Di chuyển vệt đạn với điểm cuối mới
                bulletTrailHandler.MoveTrail(this, firePoint.position, endPoint, whatToHit);

                // Trừ đạn sau mỗi lần bắn
                curAmmo -= 1;
                if (curAmmo == 0)
                {
                    isRoundEmpty = true;
                }

                // Cập nhật thời gian bắn để quản lý hồi tâm
                recoilResetTime = Time.time + recoilCooldown;
            }
            else
            {
                Debug.Log("Out of ammo");
                ResetRecoil();
            }
        }

    }

    // Hàm áp dụng recoil lên điểm bắn (endPoint)
    Vector3 ApplyRecoil(Vector3 direction, float range)
    {
        // Tạo một vector xê dịch ngẫu nhiên để mô phỏng recoil
        float recoilX = Random.Range(-currentRecoil, currentRecoil);
        float recoilY = Random.Range(-currentRecoil, currentRecoil);

        // Cộng vector recoil vào hướng bắn ban đầu
        Vector3 recoilOffset = new Vector3(recoilX, recoilY, 0);

        // Tính toán endPoint mới với recoil
        Vector3 newEndPoint = firePoint.position + (direction * range) + recoilOffset;
        return newEndPoint;
    }

    // Hồi tâm súng
    public void ResetRecoil()
    {
        currentRecoil = 0f;
    }

    public void ResetsubAmmo()
    {
        subAmmoSetup();
    }

    // Coroutine to reload ammo after waiting for reloadTime
    public IEnumerator ReloadAmmo(ArmRotation armRotation, Animator animator)
    {
        if (curAmmo < ammoPerRound && subAmmo != 0)
        {
            isReloading = true;  // Set reloading to true
            // armRotation.ResetArmContainerRotation();
            armRotation.enabled = false;

            if (gunId == "Rifles_05" || gunId == "Rifles_04" || gunId == "Rifles_02" || gunId == "Rifles_01" || gunId == "Rifles_00")
            {
                animator.SetTrigger(gunId + "_reload");
            }

            // Wait for reloadTime seconds
            yield return new WaitForSeconds(reloadTime);

            // After the wait time, reload ammo
            if (subAmmo > ammoPerRound)
            {
                subAmmo += curAmmo;
                curAmmo = ammoPerRound;
                subAmmo -= ammoPerRound;
            }
            else
            {
                curAmmo = subAmmo;
                subAmmo -= subAmmo;
            }

            Debug.Log("Reload complete. Ammo refilled: " + curAmmo + "Remains subAmmo: " + subAmmo);

            isReloading = false;  // Reload complete
            armRotation.enabled = true;
            isRoundEmpty = false;
        }
        else
        {
            Debug.Log("Out of ammo");
        }
    }

    public void ReloadAnimation_00()
    {
        GameObject handMags = GameObject.Find("left_arm_down");
        foreach (Transform item in handMags.transform)
        {
            if (item.name.StartsWith(gunName))
            {
                item.gameObject.SetActive(true);
            }
        }
    }

    public void ReloadAnimation_01()
    {
        GameObject handMags = GameObject.Find("left_arm_down");
        foreach (Transform item in handMags.transform)
        {
            if (item.name.StartsWith(gunName))
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    // Hàm cập nhật bán kính của CircleCollider2D
    public void UpdateBodyPointColliderRadius()
    {
        if (bodyPointCollider != null)
        {
            float distance = Vector2.Distance(bodyPoint.position, firePoint.position);
            bodyPointCollider.radius = distance / 2;
        }
    }

    public void OnDrawGizmos()

    {
        // if (bodyPointCollider != null)
        // {
        //     // Set the color of the gizmo
        //     // Gizmos.color = Color.black;

        //     // Gizmos.DrawWireSphere(bodyPoint.position, bodyPointCollider.radius);
        //     // Gizmos.DrawLine(bodyPoint.position, firePoint.position);
        // }
    }

    public void changePistolTexture_OutOfAmmo()
    {
        if (weaponType == WeaponType.Pistol)
        {
            if (isRoundEmpty)
            {
                withMagAndAmmoTexture.SetActive(false);
                magTexture.SetActive(true);
                outOfAmmoTexture.SetActive(true);

            }
            else
            {
                withMagAndAmmoTexture.SetActive(true);
                magTexture.SetActive(false);
                outOfAmmoTexture.SetActive(false);
            }
        }
    }

    public void updateWeaponUI()
    {
        if (weaponIndicator != null)
        {
            weaponIndicator.SetAmmo(curAmmo, subAmmo);
            weaponIndicator.SetGunName(gunName);
            if (autoMode == false)
            {
                weaponIndicator.SetFireMode("Mode 1");
            }
            else
            {
                weaponIndicator.SetFireMode("Mode 2");
            }
        }
    }

    private void changeShootStatus()
    {
        shootable = !shootable;
    }

}
