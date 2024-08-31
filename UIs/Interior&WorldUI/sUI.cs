using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//public enum InteriorUIMode
//{
//    CLOSED = 0,
//    PARTY = 1,
//    EQUIPMENT = 2,
//    ITEMS = 3,
//    JOURNAL = 4,
//    SAVE = 5,
//}

public class sInteriorUI : MonoBehaviour
{
    private GameManager gM;

    //private InteriorUIMode interiorUIMode;
    private bool isOpen = false;

    [SerializeField] private Transform interactIndicator;
    [SerializeField] private TextMeshProUGUI interactText;

    [Space(4)]
    [SerializeField] private Transform menuPanel;
    [SerializeField] private Transform menuContentParent;

    [Space(4)]
    [SerializeField] private Transform conversationPanel;
    [SerializeField] private TextMeshProUGUI conversationText; //get rid of later and add convo script

    [Space(4)]
    [SerializeField] private Transform notifierPanel;
    [SerializeField] private Transform notifierIBT;
    [SerializeField] private Transform notifierOBT;
    [SerializeField] private TextMeshProUGUI notifierText; //get rid of later and add convo script

    [Space(4)]
    [SerializeField] private GameObject placeholderPanelPrefab;

    [Space(4)]
    [SerializeField] private GameObject partyMenuPrefab;
    [SerializeField] private GameObject itemsMenuPrefab;


    private void Awake()
    {
        //interiorUIMode = InteriorUIMode.CLOSED;

        Actions.OnProximityToInteractable += ToggleInteractIndicator;

        Actions.OnConversationStart += HandleConversationStart;
        Actions.OnConversationEnd += HandleConversationEnd;

        Actions.OnItemReceived += HandleItemReceived;

        interactIndicator.localScale = Vector3.zero;
        menuPanel.localScale = Vector3.zero;
        conversationPanel.localScale = Vector3.zero;
    }

    private void OnDestroy()
    {
        Actions.OnProximityToInteractable -= ToggleInteractIndicator;

        Actions.OnConversationStart -= HandleConversationStart;
        Actions.OnConversationEnd -= HandleConversationEnd;

        Actions.OnItemReceived -= HandleItemReceived;
    }

    private void Start()
    {
        gM = GameManager.instance;
        isOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isOpen) { OpenMenu();}
            else { CloseMenuButton();}
        }
    }

    public void SaveGameButton()
    {
        gM.SaveGame();
    }
    
    public void QuitGameButton()
    {
        gM.QuitToDesktop();
    }

    private void ToggleInteractIndicator(bool show, string action)
    {
        interactText.text = "Press E to " + action;
        interactIndicator.localScale = show ? Vector3.one : Vector3.zero;
    }

    private void OpenMenu()
    {
        menuPanel.localScale = Vector3.one;
        //interiorUIMode = InteriorUIMode.PARTY;
        isOpen = true;
        FillPartyInfo();
    }

    private void HandleConversationStart(sNPC npc)
    {
        conversationPanel.localScale = Vector3.one;
        conversationText.text = npc.utterance;
        print(npc.utterance);
    }

    private void HandleConversationEnd()
    {
        conversationPanel.localScale = Vector3.zero;
    }

    private void HandleItemReceived(Weapon wep)
    {
        print(wep.Name);
        ShowNotifier(wep.Name + " added to inventory.");
    }

    private void ShowNotifier(string text)
    {
        notifierPanel.transform.position = notifierIBT.position;
        notifierText.text = text;
        StartCoroutine(HideNotifierAfter(1));
    }
    private IEnumerator HideNotifierAfter(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);
        notifierText.text = "";
        notifierPanel.transform.position = notifierOBT.position;
    }

    //BUTTONS
    public void CloseMenuButton()
    {
        ClearMenuContentParent();
        menuPanel.localScale = Vector3.zero;
        //interiorUIMode = InteriorUIMode.CLOSED;
        isOpen = false;
    }
    public void PartyButton()
    {
        ClearMenuContentParent();
        FillPartyInfo();
    }
    public void EquipmentButton()
    {
        ClearMenuContentParent();
        FillEquipmentInfo();
    }
    public void ItemsButton()
    {
        ClearMenuContentParent();
        FillItemsInfo();
    }

    private void ClearMenuContentParent()
    {
        foreach (Transform child in menuContentParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void FillPartyInfo()
    {
        ClearMenuContentParent();
        Instantiate(partyMenuPrefab, menuContentParent);
    }
    private void FillEquipmentInfo()
    {
        ClearMenuContentParent();
        Instantiate(placeholderPanelPrefab, menuContentParent);
    }
    private void FillItemsInfo()
    {
        ClearMenuContentParent();
        Instantiate(itemsMenuPrefab, menuContentParent);
    }
}
