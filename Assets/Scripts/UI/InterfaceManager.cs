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
        InputHandler();
        
    }

    private void InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LineController.Instance.ShowLineRenderer();
        }
    }

    private void UpdateEnemyCount()
    {
        // StringBuilder implementation for better performance.
        enemyCountBuilder.Length = 0;
        enemyCountBuilder.Append("Enemies: ");
        enemyCountBuilder.Append(EnemyCount.ToString());

        enemyCountText.text = enemyCountBuilder.ToString();
    }

    /// <summary>
    /// LineRenderer path visualization
    /// </summary>
    /// <param name="path">Path of AStar Algorithm</param>
    public void UpdateLineRendererPath(Stack<Node> path)
    {
        List<Vector3> positions = new List<Vector3>();

        Node[] nodeArray = path.ToArray();

        for (int i = 0; i < nodeArray.Length; i++)
        {
            positions.Add(nodeArray[i].Tile.WorldPosition);
        }

        LineController.Instance.SetupLine(positions);
    }
}
