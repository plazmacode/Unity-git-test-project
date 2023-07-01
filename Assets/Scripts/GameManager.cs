using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public ObjectPool Pool { get; set; }

    public TowerButton ClickedButton { get; set; }

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


    [SerializeField]
    private TextMeshProUGUI moneyText;
    public TextMeshProUGUI MoneyText { get => moneyText; set => moneyText = value; }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }
    private void Update()
    {
        HandleEscape();
    }

    public void StartWave()
    {
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
