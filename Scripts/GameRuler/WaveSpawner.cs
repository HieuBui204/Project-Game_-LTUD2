using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    [SerializeField]
    public class EnemyTypeConfig
    {
        public string enemyName;
        public Transform enemyPrefab;
        public int startWave;
        public int initialCount;
        public int countIncrement;
    }

    [System.Serializable]
    public class BossConfig
    {
        public string bossName;
        public Transform bossPrefab;
        public List<int> bossWaves;
    }

    [SerializeField]
    public List<EnemyTypeConfig> enemyTypes = new List<EnemyTypeConfig>();
    public List<BossConfig> bosses = new List<BossConfig>(); // Danh sách BOSS
    public Transform[] spawnPoints;
    public float timeBetweenWaves;
    private float waveCountdown;

    [Header("Enemy Count UI")]
    public TextMeshProUGUI enemyCount;

    private SpawnState state = SpawnState.COUNTING;
    public SpawnState State
    {
        get { return state; }
    }
    private int waveNumber = 1;

    public int NextWave
    {
        get { return waveNumber; }
    }

    public float WaveCountdown
    {
        get { return waveCountdown; }
    }

    void Start()
    {
        EntryZoneSetup();
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points referenced.");
        }
        waveCountdown = timeBetweenWaves;
    }

    void Update()
    {
        if (state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
            }
            else
            {
                return;
            }
        }

        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayRoundStartSound();
                StartCoroutine(SpawnWave());
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
        UpdateEnemyCountText();
    }

    public void EntryZoneSetup()
    {
        GameObject entryArea = GameObject.Find("EntryArea");
        Transform enemySpawnPoint = entryArea.transform.Find("EnemySpawnPoints");
        foreach (Transform mobSpawnPoint in enemySpawnPoint)
        {
            mobSpawnPoint.gameObject.SetActive(true);
            this.AddSpawnPoint(mobSpawnPoint);
        }
    }
    public void AddSpawnPoint(Transform newSpawnPoint)
    {
        List<Transform> spawnPointList = new List<Transform>(spawnPoints);
        spawnPointList.Add(newSpawnPoint);
        spawnPoints = spawnPointList.ToArray();
    }

    void WaveCompleted()
    {
        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        waveNumber++;
    }

    bool EnemyIsAlive()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length > 0;
    }

    void UpdateEnemyCountText()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int enemyCounts = enemies.Length;
        enemyCount.SetText(enemyCounts.ToString());
    }

    IEnumerator SpawnWave()
    {
        state = SpawnState.SPAWNING;

        // Lấy danh sách các loại zombie được phép sinh trong wave hiện tại
        List<EnemyTypeConfig> activeEnemyTypes = GetActiveEnemyTypes(waveNumber);

        // Tính tổng số quái cần spawn cho wave này
        int totalEnemyCount = 0;
        foreach (var enemyType in activeEnemyTypes)
        {
            totalEnemyCount += enemyType.initialCount + (waveNumber - enemyType.startWave) * enemyType.countIncrement;
        }

        // Đặt lại killCountsCurWave cho wave mới
        GameMaster gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        gm.killCountsCurWave = 0;
        bool bossSpawned = false; // Cờ kiểm tra BOSS đã spawn chưa

        foreach (var enemyType in activeEnemyTypes)
        {
            int enemyCount = enemyType.initialCount + (waveNumber - enemyType.startWave) * enemyType.countIncrement;

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy(enemyType.enemyPrefab);
                yield return new WaitForSeconds(5f / GetSpawnRate(enemyType));
            }
        }

        // Kiểm tra và spawn Boss sau khi spawn hết zombie nhỏ
        while (!bossSpawned)
        {
            if (gm.killCountsCurWave >= totalEnemyCount / 2)
            {
                SpawnBossIfApplicable();
                bossSpawned = true;
            }
            yield return null;
        }


        // Chờ tất cả quái trong wave này bị tiêu diệt để kết thúc wave
        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return null;
        }

        state = SpawnState.WAITING;
        yield break;
    }

    void SpawnBossIfApplicable()
    {
        foreach (var boss in bosses)
        {
            if (boss.bossWaves.Contains(waveNumber))
            {
                SpawnEnemy(boss.bossPrefab);
                Debug.Log("BOSS HAS SPAWNED!");
                break; // Chỉ spawn BOSS một lần trong mỗi wave
            }
        }
    }

    void SpawnEnemy(Transform enemyPrefab)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points available.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Transform enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        StartCoroutine(WaitForPlayerRespawnAndUpdateTarget(enemyInstance));
    }

    List<EnemyTypeConfig> GetActiveEnemyTypes(int currentWave)
    {
        List<EnemyTypeConfig> activeTypes = new List<EnemyTypeConfig>();

        foreach (var enemyType in enemyTypes)
        {
            if (currentWave >= enemyType.startWave)
            {
                activeTypes.Add(enemyType);
            }
        }

        return activeTypes;
    }

    float GetSpawnRate(EnemyTypeConfig enemyType)
    {
        return 1f;
    }

    IEnumerator WaitForPlayerRespawnAndUpdateTarget(Transform enemyInstance)
    {
        yield return new WaitForSeconds(1f);

        if (enemyInstance == null)
        {
            Debug.LogWarning("Instance kẻ thù đã bị hủy trước khi cập nhật mục tiêu.");
            yield break;
        }
    }
}
