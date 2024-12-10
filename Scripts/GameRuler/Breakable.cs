using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Breakable : MonoBehaviour
{
    public enum BreakableType
    {
        Door,
        Removeable
    }

    [System.Serializable]
    public class BreakableStats
    {
        [HideInInspector]
        public float cost;
        public float maxHealth;
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

    public BreakableStats breakableStats = new BreakableStats();

    [Header("Breakable Object Type")]
    public BreakableType breakableType;
    private float pastHealth;

    // private bool isBroken = false;
    public bool isInteracable = true;
    private const float detectRadius = 5f;
    private Transform detectPoint;
    private void Awake()
    {
        breakableStats.Init();
        pastHealth = breakableStats.curHealth;
        detectPoint = transform.Find("DetectPoint");
        if (detectPoint == null)
        {
            Debug.Log("Khong tim thay detectPoint");
        }
    }

    private void Update()
    {
        DetectEnities();
        if (breakableStats.curHealth <= 0)
        {
            colliderToTrigger();
        }

        OnLostHealthEvent();
        OnFixHealthEvent();
    }

    public void DetectEnities()
    {
        Collider2D obj = Physics2D.OverlapCircle(detectPoint.position, detectRadius, LayerMask.GetMask("Enemy"));
        if (obj != null)
        {
            isInteracable = false;
        }
        else
        {
            isInteracable = true;
        }
    }

    public void BuyRemoveable(Player player)
    {
        if (player.playerStats.point >= breakableStats.cost)
        {
            player.playerStats.point -= breakableStats.cost;
            HandleRemoveable();
        }
    }

    public void DamageBreakable()
    {
        if (breakableStats.curHealth == 0)
        {
            return;
        }
        breakableStats.curHealth -= 1;

    }

    public void Fix_Door()
    {
        if (!isInteracable)
        {
            Debug.Log("Can't fix the door right now. It's being damaged!");
            return;
        }
        if (breakableStats.curHealth == breakableStats.maxHealth)
        {
            return;
        }
        if (breakableType == BreakableType.Door)
        {
            breakableStats.curHealth += 1;
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.isTrigger = false;
            }
        }
        Debug.Log("Door health: " + breakableStats.curHealth);
    }

    private void HandleRemoveable()
    {
        colliderToTrigger();
        activeRelateMobSpawns();
        Debug.Log("The way has been opened!");
    }
    private void colliderToTrigger()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void activeRelateMobSpawns()
    {
        // Lấy đối tượng cha của gameObject hiện tại
        Transform area = transform.parent;
        Transform enemySpawnPoint = area.Find("EnemySpawnPoints");
        GameObject gameMaster = GameObject.Find("_GM");
        foreach (Transform mobSpawnPoint in enemySpawnPoint)
        {
            mobSpawnPoint.gameObject.SetActive(true);
            gameMaster.GetComponent<WaveSpawner>().AddSpawnPoint(mobSpawnPoint);
        }
    }
    private void OnLostHealthEvent()
    {
        if (pastHealth > breakableStats.curHealth)
        {
            DoorTextureUpdate("lost");
            pastHealth = breakableStats.curHealth;
        }

    }

    private void OnFixHealthEvent()
    {
        if (breakableStats.curHealth > pastHealth)
        {
            DoorTextureUpdate("recover");
            pastHealth = breakableStats.curHealth;
        }

    }
    private void DoorTextureUpdate(string message)
    {
        Transform door = transform.Find("Door");

        if (door == null)
        {
            Debug.LogWarning("Door not found!");
            return;
        }

        if (message == "lost" && breakableStats.curHealth % 2 == 0)
        {
            foreach (Transform woodenBlank in door)
            {
                if (woodenBlank.gameObject.activeSelf && woodenBlank.name.StartsWith("Wooden"))
                {
                    woodenBlank.gameObject.SetActive(false);
                    break;
                }
            }
        }
        else if (message == "recover" && breakableStats.curHealth % 2 == 0)
        {
            for (int i = door.childCount - 1; i >= 0; i--)
            {
                Transform woodenBlank = door.GetChild(i);
                if (!woodenBlank.gameObject.activeSelf && woodenBlank.name.StartsWith("Wooden"))
                {
                    woodenBlank.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(Breakable))]
    public class BreakableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Breakable breakable = (Breakable)target;

            // Draw the default Inspector
            DrawDefaultInspector();

            // Conditionally show 'cost' field based on the selected BreakableType
            if (breakable.breakableType == Breakable.BreakableType.Removeable)
            {
                SerializedProperty costProperty = serializedObject.FindProperty("breakableStats.cost");
                EditorGUILayout.PropertyField(costProperty, new GUIContent("Cost"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
