using UnityEngine;

public class MuzzleFlashHandler : MonoBehaviour
{
    private Transform MuzzleFlashPrefab;
    private Transform firePoint;
    private float effectSpawnRate = 10f;
    private float timeToSpawnEffect = 0f;

    // Method to show Muzzle Flash
    private void Awake()
    {
        MuzzleFlashPrefab = Resources.Load<Transform>("BulletEffects/MuzzleFlash");
        firePoint = transform.Find("FirePoint");
    }

    public void ShowMuzzleFlash()
    {
        if (MuzzleFlashPrefab != null)
        {
            if (Time.time >= timeToSpawnEffect)
            {
                // Create Muzzle Flash at the firePoint position
                Transform muzzleFlash = Instantiate(MuzzleFlashPrefab, firePoint.position, firePoint.rotation);

                // Randomize size for a more dynamic effect
                float size = Random.Range(0.6f, 0.9f);
                muzzleFlash.localScale = new Vector3(size, size, size);

                // Destroy Muzzle Flash after a short duration
                Destroy(muzzleFlash.gameObject, 0.05f);

                timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
            }
        }
        else
        {
            Debug.LogWarning("MuzzleFlashPrefab is not assigned!");
        }
    }
}
