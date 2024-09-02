using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sItemsMenu : MonoBehaviour
{
    [SerializeField] private GameObject itemRowPrefab;

    void Start()
    {
        foreach (Item item in GameManager.instance.activeSave.Inventory)
        {
            Instantiate(itemRowPrefab, this.transform).GetComponent<sItemRow>().FillInfo(item);
        }
    }
}
