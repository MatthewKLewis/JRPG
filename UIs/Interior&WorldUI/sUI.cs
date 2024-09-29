using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    [SerializeField] private GameObject partyMenuPrefab;
    [SerializeField] private GameObject itemsMenuPrefab;
    [SerializeField] private GameObject equipmentMenuPrefab;
    [SerializeField] private GameObject journalMenuPrefab;
    [SerializeField] private GameObject saveMenuPrefab;


    //ACTION REGISTRY
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


    //MONOBEHAVIOR
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
    private void OpenMenu()
    {
        menuPanel.localScale = Vector3.one;
        //interiorUIMode = InteriorUIMode.PARTY;
        isOpen = true;
        FillPartyInfo();
    }


    //HANDLERS
    private void HandleConversationStart(sNPC npc)
    {
        Camera activeCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        Vector3 screenPos = activeCamera.WorldToScreenPoint(npc.spotOverHead);

        print(screenPos);
        conversationPanel.position = new Vector3(screenPos.x, screenPos.y, 0f);
        conversationPanel.localScale = Vector3.one;
        conversationText.text = npc.utterance;
    }
    private void HandleConversationEnd()
    {
        conversationPanel.localScale = Vector3.zero;
    }
    private void HandleItemReceived(Item item)
    {
        ShowNotifier(item.Name + " added to inventory.");
    }    
    private void ToggleInteractIndicator(bool show, string action)
    {
        interactText.text = "Press E to " + action;
        interactIndicator.localScale = show ? Vector3.one : Vector3.zero;
    }


    //NOTIFIER
    private void ShowNotifier(string text)
    {
        notifierPanel.transform.position = notifierIBT.position;
        notifierText.text = text;
        StartCoroutine(HideNotifierAfter(1.5f));
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
    public void EquipmentButton()
    {
        FillEquipmentInfo();
    }
    public void ItemsButton()
    {
        FillItemsInfo();
    }
    public void JournalButton()
    {
        FillJournalInfo();
    }
    public void PartyButton()
    {
        FillPartyInfo();
    }
    public void QuitGameButton()
    {
        gM.QuitToDesktop();
    }
    public void SaveGameButton()
    {
        FillSaveInfo();
    }


    //MENU CONTENT FILLING
    private void FillPartyInfo()
    {
        ClearMenuContentParent();
        Instantiate(partyMenuPrefab, menuContentParent);
    }
    private void FillEquipmentInfo()
    {
        ClearMenuContentParent();
        Instantiate(equipmentMenuPrefab, menuContentParent);
    }
    private void FillItemsInfo()
    {
        ClearMenuContentParent();
        Instantiate(itemsMenuPrefab, menuContentParent);
    }
    private void FillJournalInfo()
    {
        ClearMenuContentParent();
        Instantiate(journalMenuPrefab, menuContentParent);
    }
    private void FillSaveInfo()
    {
        ClearMenuContentParent();
        Instantiate(saveMenuPrefab, menuContentParent);
    }


    //UTILITY
    private void ClearMenuContentParent()
    {
        foreach (Transform child in menuContentParent)
        {
            Destroy(child.gameObject);
        }
    }
}
