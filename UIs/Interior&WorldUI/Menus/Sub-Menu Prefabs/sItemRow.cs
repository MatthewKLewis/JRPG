using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class sItemRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI quantityText;

    public void FillInfo(Item item)
    {
        nameText.text = item.Name;
        quantityText.text = item.Quantity.ToString();
    } 
}
