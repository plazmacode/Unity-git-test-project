using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public ObjectPool Pool { get; set; }

    public TowerButton ClickedButton { get; set; }

    public List<Tower> Towers { get; set; } = new List<Tower>();

    private Tower selectedTower;

    public Tower MovedTower { get; set; }

    [SerializeField]
    private float money = 0;

    [SerializeField]
    private TextMeshProUGUI moneyText;
    public TextMeshProUGUI MoneyText { get => moneyText; set => moneyText = value; }
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

    public Tower SelectedTower { get => selectedTower; set => selectedTower = value; }

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
            tileValue.MyTower = Towers[i];
            tileValue.TileNode.SetWalkable(false);
        }
    }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();

        // Updates money text incase start money is not 0.
        moneyText.text = "Money: " + money.ToString();
    }
    private void Update()
    {
        HandleEscape();
        HandleRightClick();
    }

    public void StartWave()
    {
        //LevelManager.Instance.GeneratePath(false);
        if (LevelManager.Instance.Path == null)
        {
            CreateWaypoints();
        }
        StartCoroutine(SpawnWave());
    }

    public void CreateWaypoints()
    {
        LevelManager.Instance.CalculateWaypoints(2);
    }

    public void SelectTower(Tower tower)
    {
        if (SelectedTower != null)
        {
            SelectedTower.Select();
        }
        SelectedTower = tower;
        SelectedTower.Select();
    }

    public void DeselectTower()
    {
        if (SelectedTower != null)
        {
            SelectedTower.Select();
        }

        SelectedTower = null;
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < 25; i++)
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
            yield return new WaitForSeconds(0.2f);
        }
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

    private void HandleRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Hover.Instance.Deactivate();
            if (SelectedTower != null)
            {
                DeselectTower();
            }
            TileManager.Instance.ClearDebugCanvas();
        }
    }

    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hover.Instance.Deactivate();
            if (SelectedTower != null)
            {
                DeselectTower();
            }
            TileManager.Instance.ClearDebugCanvas();
        }
    }
}
