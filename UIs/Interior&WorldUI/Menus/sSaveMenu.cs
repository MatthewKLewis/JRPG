using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class sSaveMenu : MonoBehaviour
{
    private GameManager gM;

    [SerializeField] private TextMeshProUGUI[] saveButtonTexts;

    private void Start()
    {
        gM = GameManager.instance;
        for (int i = 0; i < saveButtonTexts.Length; i++)
        {
            if (gM.saveGames[i] != null)
            {
                saveButtonTexts[i].text = gM.saveGames[i].dateCreated;
            }
            else
            {
                Debug.LogError("A save slot was null!");
            }
        }
    }

    public void SaveInSlot(int slot)
    {
        print(slot.ToString());
        string newButtonText = gM.SaveGame(slot);
        saveButtonTexts[slot].text = newButtonText;
    }
}
