using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Net.Security;
using Unity.VisualScripting;

public class GameMaster : MonoBehaviour
{
    [Header("Round Result")]
    public int killCounts;
    public int killCountsCurWave;
    public float scoreSum = 0;

    public GameObject resultPanel;
    public GameObject UIOverlay;
    private static GameMaster gm;
    public int fallBoundary = -60;
    private GameObject playerObj;
    public Transform spawnPoint;
    public Transform playerPrefab;
    public CinemachineVirtualCamera virtualCamera; // Tham chiếu đến Cinemachine Virtual Camera
    public int spawnDelay = 2;
    public GameObject settingPanel;

    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();

        // Gán target ban đầu cho virtual camera
        if (virtualCamera != null && playerObj != null)
        {
            virtualCamera.Follow = playerObj.transform;
        }
    }

    private void Awake()
    {
        UIOverlay.gameObject.SetActive(true);
    }

    private void Update()
    {
        // Kiểm tra nếu người chơi rơi xuống dưới fallBoundary
        if (playerObj != null && playerObj.transform.position.y <= fallBoundary)
        {
            Player player = playerObj.GetComponent<Player>();
            player.DamagePlayer(9999999); // Gây sát thương lớn để giết người chơi
            Destroy(player.gameObject);
            playerObj = null; // Set playerObj to null after destroying it
        }

        ProcessKills();
    }

    public IEnumerator RespawnPlayer()
    {
        Debug.Log("TODO: Add waiting for spawn sound");
        yield return new WaitForSeconds(spawnDelay);

        if (spawnPoint != null && playerPrefab != null)
        {
            // Hồi sinh người chơi mới
            Transform newPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            playerObj = newPlayer.gameObject; // Cập nhật playerObj thành người chơi mới

            // Cập nhật target cho virtual camera
            if (virtualCamera != null)
            {
                virtualCamera.Follow = newPlayer;
            }

            Debug.Log("TODO: Add Spawn Particles");
        }
        else
        {
            Debug.LogError("Spawn point hoặc player prefab chưa được gán!");
        }
    }
    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        if (player.playerStats.lifeCount > 0)
        {
            gm.StartCoroutine(gm.RespawnPlayer());
            player.playerStats.lifeCount--;
        }
        else
        {
            Debug.Log("Player đã hết mạng và không thể hồi sinh!");

            // Tìm đối tượng WaveUI trong scene và dừng timer
            WaveUI waveUI = GameObject.FindObjectOfType<WaveUI>();
            if (waveUI != null)
            {
                waveUI.StopTimer();
                //Cap nhat thong tin len bang RESULT sau khi chet
                // Get stats to update result UI
                int currentWave = waveUI.currentWave; // Assuming spawner keeps track of current wave
                float survivalTime = waveUI.survivalTime;
                Enemy enemy = GameObject.FindObjectOfType<Enemy>();

                int kills = gm.killCounts; // Assuming `kills` is tracked in player stats
                // float headshots = 3; // Assuming `headshots` is tracked in player stats
                float score = gm.scoreSum; // Assuming `score` is tracked in player stats

                gm.resultPanel.SetActive(true);
                UpdateResult resultUI = GameObject.FindObjectOfType<UpdateResult>();
                if (resultUI != null)
                {
                    resultUI.UpdateResults(currentWave, survivalTime, kills, score);
                    gm.settingPanel.SetActive(false);
                }
                else
                {
                    Debug.LogError("UpdateResult UI not found in scene!");
                }

            }
            else
            {
                Debug.LogError("Không tìm thấy WaveUI trong scene!");
            }

        }
    }

    private static Queue<int> killsQueue = new Queue<int>();

    public static void EnqueueKill()
    {
        lock (killsQueue)
        {
            killsQueue.Enqueue(1); // Thêm 1 kill vào hàng đợi
        }
    }

    public static void ProcessKills()
    {
        lock (killsQueue)
        {
            while (killsQueue.Count > 0)
            {
                killsQueue.Dequeue();
                gm.killCounts++;
                gm.killCountsCurWave++;
            }
        }
    }

}
