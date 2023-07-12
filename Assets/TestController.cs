using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : Singleton<TestController>
{
    [Header("General Settings")]
    [SerializeField]
    private float timeScale = 1.0f;

    [SerializeField]
    private int money = 100000;

    [SerializeField]
    private int health = 10;

    [Header("Tower Settings")]
    [SerializeField]
    private int towerMoveCooldown = 3;

    [SerializeField]
    private TowerButton[] towerButtons;

    [SerializeField]
    private int[] towerMoveDistance;

    [SerializeField]
    private int[] towerCooldown;

    [Header("Wave Settings")]
    [SerializeField]
    private int enemiesPerWave = 25;

    [SerializeField]
    private int currentWave = 0;

    [SerializeField]
    private int wavesBetweenSetupPhases = 5;

    [Header("Waypoint Settings")]
    [SerializeField]
    private int waypointCount = 2;

    [SerializeField]
    private bool sequentialMode = true;

    [SerializeField]
    private bool minWaypointMode = true;

    [SerializeField]
    private int minWaypointDistance = 5; //

    public float TimeScale {
        get
        {
            return timeScale;
        }
        set
        {
            timeScale = value;
            UpdateTimeScale();
        }
    }

    public int TowerMoveCooldown { get => towerMoveCooldown; set => towerMoveCooldown = value; }
    public bool SequentialMode { get => sequentialMode; set => sequentialMode = value; }
    public bool MinWaypointMode { get => minWaypointMode; set => minWaypointMode = value; }
    public int MinWaypointDistance { get => minWaypointDistance; set => minWaypointDistance = value; }

    private void Start()
    {
        Time.timeScale = TimeScale;
        GameManager.Instance.Money = money;
        GameManager.Instance.Health = health;

        // Update preplaced towers
        for (int i = 0; i < GameManager.Instance.Towers.Count; i++)
        {
            GameManager.Instance.Towers[i].TowerMoveCooldown = TowerMoveCooldown;
        }

        // Update tower prefabs
        for (int i = 0; i < towerButtons.Length; i++)
        {
            towerButtons[i].TowerPrefab.GetComponent<Tower>().MoveDistance = towerMoveDistance[i];
            int cooldown = TowerMoveCooldown + towerCooldown[i];
            towerButtons[i].TowerPrefab.GetComponent<Tower>().TowerMoveCooldown = cooldown;
        }

        GameManager.Instance.EnemyPerWave = enemiesPerWave;

        GameManager.Instance.Waves = currentWave;

        GameManager.Instance.WavesBetweenSetupPhases = wavesBetweenSetupPhases;

    }

    private void Awake()
    {
        LevelManager.Instance.WaypointCount = waypointCount;        
    }

    private void Update()
    {
        
    }

    private void UpdateTimeScale()
    {
        Time.timeScale = TimeScale;
    }
}
