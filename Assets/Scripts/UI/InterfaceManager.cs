using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class InterfaceManager : Singleton<InterfaceManager>
{
    [SerializeField]
    TextMeshProUGUI enemyCountText;

    [SerializeField]
    TextMeshProUGUI waveCounterText;

    [SerializeField]
    TextMeshProUGUI setupPhaseText;

    [SerializeField]
    TextMeshProUGUI healthText;

    public int EnemyCount { get; set; }
    public TextMeshProUGUI SetupPhaseText { get => setupPhaseText; set => setupPhaseText = value; }

    private StringBuilder enemyCountBuilder = new StringBuilder();
    private StringBuilder waveCounterBuilder = new StringBuilder();
    private StringBuilder healthBuilder = new StringBuilder();
    private void Update()
    {
        UpdateInterface();
        InputHandler();
    }

    private void InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LineController.Instance.ShowLineRenderer();
        }
    }

    private void UpdateInterface()
    {
        waveCounterBuilder.Length = 0;
        waveCounterBuilder.Append("Wave: ");
        waveCounterBuilder.Append(GameManager.Instance.Waves.ToString());

        waveCounterText.text = waveCounterBuilder.ToString();

        enemyCountBuilder.Length = 0;
        enemyCountBuilder.Append("Enemies: ");
        enemyCountBuilder.Append(EnemyCount.ToString());

        enemyCountText.text = enemyCountBuilder.ToString();

        healthBuilder.Length = 0;
        healthBuilder.Append("Health: ");
        healthBuilder.Append(GameManager.Instance.Health.ToString());

        healthText.text = healthBuilder.ToString();
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
