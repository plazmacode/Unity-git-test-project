using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterfaceManager : Singleton<InterfaceManager>
{
    [SerializeField]
    TextMeshProUGUI enemyCountText;
    public int EnemyCount { get; set; }

    private void Update()
    {
        enemyCountText.text = "Enemies: " + EnemyCount.ToString();
    }
}
