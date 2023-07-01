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

    [SerializeField]
    private TextMeshProUGUI buttonText;

    [SerializeField]
    private float price;

    public float BasePrice { get; set; }

    [SerializeField]
    private string towerName = "";

    private bool showName = false;

    public GameObject TowerPrefab { get => towerPrefab; private set => towerPrefab = value; }
    public Sprite Sprite { get => sprite; private set => sprite = value; }
    public float Price
    {
        get
        {
            return price;
        }
        set
        {
            price = value;
            UpdatePriceText();
        }
    }

    private void Start()
    {
        BasePrice = price;
        UpdatePriceText();
    }

    private void UpdatePriceText()
    {
        if (showName)
        {
            buttonText.text = $"{towerName}\n{Price}";
        } else
        {
            buttonText.text = $"\n{Price}";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        showName = true;
        UpdatePriceText();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        showName = false;
        UpdatePriceText();
    }
}
