using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sTitleScreen : MonoBehaviour
{
    private GameManager gM;

    [SerializeField] private Transform loadGamePanel;
    [SerializeField] private TextMeshProUGUI[] loadButtonTexts;

    void Start()
    {
        loadGamePanel.localScale = Vector3.zero;

        gM = GameManager.instance;
        for (int i = 0; i < loadButtonTexts.Length; i++)
        {
            if (gM.saveGames[i] != null)
            {
                loadButtonTexts[i].text = gM.saveGames[i].dateCreated;
            }
            else
            {
                Debug.LogError("A save slot was null!");
            }
        }
    }

    public void NewGameButton()
    {
        gM.StartNewGame();
    }

    public void LoadGamePanelOpen()
    {
        loadGamePanel.localScale = Vector3.one;
    }

    public void LoadGamePanelClose()
    {
        loadGamePanel.localScale = Vector3.zero;
    }

    public void LoadGameButton(int slot)
    {
        print("slot clicked:" + slot);
        gM.LoadSavedGame(slot);
    }

    public void QuitGameButton()
    {
        gM.QuitToDesktop();
    }
}
