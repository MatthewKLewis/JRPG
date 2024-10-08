using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class sGenericSelectButton: MonoBehaviour, IPointerEnterHandler
{
    private BattleManager bM;

    [SerializeField] private Button button;
    [SerializeField] private TMPro.TextMeshProUGUI text;

    private SelectPanelType selectType;
    private Character charThisButtonRepresents;
    private Spell spellThisButtonRepresents;
    private Item itemThisButtonRepresents;

    private Transform parentAdditionalPanelParent;

    private BattleChoice choices;

    private void Start()
    {
        bM = BattleManager.instance;
    }

    public void FillLastStep(Character character, Transform parentPanelParent, BattleChoice prevChoices)
    {
        //State
        charThisButtonRepresents = character;
        selectType = SelectPanelType.ENEMY;

        choices = prevChoices; //INTAKE LAST CHOICES

        //Display
        parentAdditionalPanelParent = parentPanelParent;
        text.text = charThisButtonRepresents.Name;
    }

    public void FillButtonSpellInfo(Spell spell, Transform parentPanelParent, BattleChoice prevChoices)
    {
        //State
        spellThisButtonRepresents = spell;
        selectType = SelectPanelType.MAGIC;

        choices = prevChoices; //INTAKE LAST CHOICES

        if (choices.Chooser.MP < spellThisButtonRepresents.MPCost)
        {
            button.interactable = false;
            text.color = Color.red;
        }

        //Display
        parentAdditionalPanelParent = parentPanelParent;
        text.text = spellThisButtonRepresents.Name + " " + spellThisButtonRepresents.MPCost;
    }

    public void FillButtonItemInfo(Item item, Transform parentPanelParent, BattleChoice prevChoices)
    {
        //State
        itemThisButtonRepresents = item;
        selectType = SelectPanelType.ITEM;

        choices = prevChoices; //INTAKE LAST CHOICES

        //Display
        parentAdditionalPanelParent = parentPanelParent;
        text.text = itemThisButtonRepresents.Name;
    }

    public void Click()
    {
        switch (selectType)
        {
            case SelectPanelType.ENEMY: //LEAF!
                choices.Target = charThisButtonRepresents;
                bM.FinalizeBattleChoice(choices);              
                break;

            case SelectPanelType.MAGIC: //branch
                choices.Spell = spellThisButtonRepresents;
                if (choices.Spell.UsedOnEnemies)
                {
                    Instantiate(bM.genericSelectPanel, parentAdditionalPanelParent).GetComponent<sGenericSelectPanel>().FillCharInfo(bM.LivingAICharacters, choices);                
                }
                else
                {
                    Instantiate(bM.genericSelectPanel, parentAdditionalPanelParent).GetComponent<sGenericSelectPanel>().FillCharInfo(bM.LivingPlayerCharacters, choices);                
                }
                break;

            case SelectPanelType.ITEM: //branch
                choices.Item = itemThisButtonRepresents;
                if (choices.Item.UsedOnEnemies)
                { 
                    Instantiate(bM.genericSelectPanel, parentAdditionalPanelParent).GetComponent<sGenericSelectPanel>().FillCharInfo(bM.LivingAICharacters, choices);
                }
                else
                {
                    Instantiate(bM.genericSelectPanel, parentAdditionalPanelParent).GetComponent<sGenericSelectPanel>().FillCharInfo(bM.LivingPlayerCharacters, choices);
                }
                break;

            default:
                Debug.LogError("Button did not receive parent's panel type!");
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectType == SelectPanelType.ENEMY && charThisButtonRepresents != null)
        {
            bM.Spotlight(charThisButtonRepresents.animator.gameObject.transform.position);
        }
    }
}
