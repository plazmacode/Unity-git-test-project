using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject towerPrefab;

    [SerializeField]
    private Sprite sprite;

    public GameObject TowerPrefab { get => towerPrefab; private set => towerPrefab = value; }
    public Sprite Sprite { get => sprite; private set => sprite = value; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Maybe add extra check when some menus are opened?
        gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
    }
}
