using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SelectPanelType
{
    ENEMY = 0,
    MAGIC = 1,
    ITEM = 2,
}

public class sGenericSelectPanel : MonoBehaviour
{
    private SelectPanelType panelType;
    [SerializeField] private GameObject selectButton;
    [SerializeField] private Transform verticalLayoutParent;
    [SerializeField] private Transform additionalPanelParent;

    void Start()
    {
        //
    }

    // ALWAYS LAST STEP!
    public void FillCharInfo(List<Character> characters, BattleChoice prevChoices) //could be friendly or enemy
    {
        //print("Panel Type is ENEMY");
        panelType = SelectPanelType.ENEMY;
        foreach (Character character in characters)
        {
            GameObject gO = Instantiate(selectButton, verticalLayoutParent); // a button that refers to BattleManager, attacking an enemy.
            gO.GetComponent<sGenericSelectButton>().FillLastStep(character, additionalPanelParent, prevChoices);
        }
    }

    public void FillSpellInfo(List<Spell> spells, BattleChoice prevChoices)
    {
        //print("Panel Type is MAGIC");
        panelType = SelectPanelType.MAGIC;
        foreach (Spell spell in spells)
        {
            GameObject gO = Instantiate(selectButton, verticalLayoutParent); // a button that refers to BattleManager, attacking an enemy.
            gO.GetComponent<sGenericSelectButton>().FillButtonSpellInfo(spell, additionalPanelParent, prevChoices);
        }
    }

    public void FillItemInfo(List<Item> items, BattleChoice prevChoices)
    {
        //print("Panel Type is ITEM");
        panelType = SelectPanelType.ITEM;
        foreach (Item item in items)
        {
            GameObject gO = Instantiate(selectButton, verticalLayoutParent); // a button that refers to BattleManager, attacking an enemy.
            gO.GetComponent<sGenericSelectButton>().FillButtonItemInfo(item, additionalPanelParent, prevChoices);
        }
    }
}
