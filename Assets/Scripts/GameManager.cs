using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public ObjectPool Pool { get; set; }

    public TowerButton ClickedButton { get; set; }

    public List<Tower> Towers { get; set; } = new List<Tower>();

    [SerializeField]
    private float money = 0;

    public float Money
    {
        get
        {
            return money;
        }
        set
        {
            money = value;
            moneyText.text = "Money: " + money.ToString();
        }
    }
    
    /// <summary>
    /// Adds pre-placed towers to game. 
    /// Helps updating path finding nodes and Towers list. <br></br>
    /// Must be called after Tilemap is made.
    /// </summary>
    public void SetupTowers()
    {
        for (int i = 0; i < Towers.Count; i++)
        {
            Vector2Int cellPosition = (Vector2Int)TileManager.Instance.TileMap.WorldToCell(Towers[i].gameObject.transform.position);
            TileValue tileValue = TileManager.Instance.GetTile(cellPosition);
            tileValue.HasTower = true;
            tileValue.TileNode.SetWalkable(false);
        }
    }

    [SerializeField]
    private TextMeshProUGUI moneyText;
    public TextMeshProUGUI MoneyText { get => moneyText; set => moneyText = value; }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();

        // Updates money text incase start money is not 0.
        moneyText.text = "Money: " + money.ToString();
    }
    private void Update()
    {
        HandleEscape();
    }

    public void StartWave()
    {
        LevelManager.Instance.GeneratePath(false);
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        int enemyIndex = Random.Range(0, 4);

        string type = string.Empty;

        switch (enemyIndex)
        {
            case 0:
                type = "WhiteChicken";
                break;
            case 1:
                type = "BlueChicken";
                break;
            case 2:
                type = "RedChicken";
                break;
            case 3:
                type = "BlackChicken";
                break;
            default:
                break;
        }

        Enemy enemy = Pool.GetObject(type).GetComponent<Enemy>();
        enemy.Spawn();
        yield return new WaitForSeconds(2.5f);
    }

    public void PickTower(TowerButton towerButton)
    {
        ClickedButton = towerButton;
        Hover.Instance.Activate(towerButton.Sprite);
    }

    public void BuyTower()
    {
        Hover.Instance.Deactivate();
    }

    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hover.Instance.Deactivate();
        }
    }
}
