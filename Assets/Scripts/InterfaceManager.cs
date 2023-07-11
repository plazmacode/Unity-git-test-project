using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class InterfaceManager : Singleton<InterfaceManager>
{
    [SerializeField]
    TextMeshProUGUI enemyCountText;
    public int EnemyCount { get; set; }
    private StringBuilder enemyCountBuilder = new StringBuilder();
    private void Update()
    {
        UpdateEnemyCount();
    }

    private void UpdateEnemyCount()
    {
        // StringBuilder implementation for better performance.
        enemyCountBuilder.Length = 0;
        enemyCountBuilder.Append("Enemies: ");
        enemyCountBuilder.Append(EnemyCount.ToString());

        enemyCountText.text = enemyCountBuilder.ToString();

    }
}
