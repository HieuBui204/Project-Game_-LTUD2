using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    private Transform BulletTrailPrefab;
    private float bulletSpeed = 100f;
    private GameObject playerObj;
    public GameObject floatingNumberPrefab; // Prefab hiệu ứng số điểm
    public Transform scorePanel;           // Vùng cố định trên GUI (ScorePanel)

    private void Awake()
    {
        BulletTrailPrefab = Resources.Load<Transform>("BulletEffects/BulletTrail");
        playerObj = transform.root.gameObject;
    }
    // Hàm di chuyển vệt đạn đến đúng khoảng cách trúng hoặc theo hướng bắn
    public void MoveTrail(Weapon weapon, Vector2 startPoint, Vector2 endPoint, LayerMask whatToHit)
    {
        Vector2 direction = (endPoint - startPoint).normalized;
        Transform bulletTrail = Instantiate(BulletTrailPrefab, startPoint, Quaternion.identity);
        StartCoroutine(MoveBulletTrail(weapon, bulletTrail, direction, weapon.range, whatToHit));
    }

    // Coroutine để di chuyển vệt đạn có giới hạn khoảng cách
    IEnumerator MoveBulletTrail(Weapon weapon, Transform trail, Vector2 direction, float maxDistance, LayerMask whatToHit)
    {
        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayGunShotSound();
        float traveledDistance = 0f;
        while (traveledDistance < maxDistance)
        {
            Collider2D obj = Physics2D.OverlapCircle(trail.position, 0.5f, whatToHit);
            if (obj != null)
            {
                // Nếu trúng Enemy
                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy != null && enemy.isDead == false)
                {
                    Player player = playerObj.GetComponent<Player>();
                    float inputdamage = weapon.damage * player.playerStats.damageMultiplier;
                    enemy.DamageEnemy(inputdamage);

                    // Cập nhật điểm
                    float pointsGained = weapon.pointPerHit * player.playerStats.pointMultiplier;
                    player.playerStats.point += pointsGained;

                    GameMaster gm = GameObject.FindObjectOfType<GameMaster>();
                    if (gm != null) gm.scoreSum += pointsGained;

                    // Hiển thị Floating Number
                    ShowFloatingNumber(pointsGained);

                    GameObject playerStatsUI = GameObject.FindWithTag("PlayerStatsUI");
                    PlayerStatusIndicator playerIndicator = playerStatsUI.GetComponent<PlayerStatusIndicator>();
                    playerIndicator.SetPoints(player.playerStats.point);

                    Vector3 bulletPositionBeforeDestroy = new Vector3(trail.position.x, trail.position.y, -10f);
                    enemy.SpawnBloodEffect(bulletPositionBeforeDestroy);

                    // Hủy vệt đạn sau khi trúng Enemy
                    Destroy(trail.gameObject);
                    yield break;
                }

                // Nếu trúng Platform (hoặc bất kỳ thứ gì không phải Enemy)
                if (obj.CompareTag("Platform")) // Giả sử Platform có tag là "Platform"
                {
                    // Hủy vệt đạn sau khi trúng Platform
                    Destroy(trail.gameObject);
                    yield break;
                }
            }
            float moveStep = bulletSpeed * Time.deltaTime;
            trail.Translate(direction * moveStep, Space.World);
            traveledDistance += moveStep;
            yield return null; // Wait for the next frame
        }

        Destroy(trail.gameObject);
    }
    private void ShowFloatingNumber(float points)
    {
        if (scorePanel != null && floatingNumberPrefab != null)
        {
            GameObject floatingNumber = Instantiate(floatingNumberPrefab, scorePanel.transform);
            FloatingNumber floatingNumberScript = floatingNumber.GetComponent<FloatingNumber>();
            if (floatingNumberScript != null)
            {
                floatingNumberScript.SetText("+" + points.ToString());
            }
        }
        else
        {
            Debug.LogWarning("ScorePanel hoặc FloatingNumberPrefab chưa được thiết lập!");
        }
    }
}
