using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#region Battle Classes and Enums

public enum ElementTypeEnum
{
    PHYSICAL = 0,
    FIRE = 1,
    ICE = 2,
}

public enum BattleChoiceTypeEnum
{
    ATTACK = 0,
    SPELL = 1,
    ITEM = 2,
}

public enum BattleResultEnum
{
    WIN,
    RUN,
}

public enum DebuffEnum
{
    SILENCED = 0,
    POISONED = 1,
    PARALYZED = 2,
}

public enum BuffEnum
{
    HASTED = 0,
    SHIELDED = 1,
}

public class BattleChoice
{
    public Character Target = null;
    public Spell Spell = null;
    public Item Item = null;

    public BattleChoiceTypeEnum Type { 
        get => Spell == null && Item == null ? BattleChoiceTypeEnum.ATTACK : Item == null ? BattleChoiceTypeEnum.SPELL : BattleChoiceTypeEnum.ITEM;
    }
    public Vector3 TargetPosition
    {
        get => this.Target.animator.gameObject.transform.position;
    }

    public override string ToString()
    {
        switch (Type) 
        {
            case BattleChoiceTypeEnum.ATTACK:
                return "Chose basic attack on " + Target.Name;
            case BattleChoiceTypeEnum.SPELL:
                return "Chose " + Spell.Name + " on " + Target.Name;
            case BattleChoiceTypeEnum.ITEM:
                return "Chose " + Item.Name + " on " + Target.Name;                
            default:
                return "CHOICE ERROR!";
        }
    }
}

public class BattleResults
{
    public BattleResultEnum result;
    public List<Item> booty;
    public int gold;
    public int experience;
}

public class Damage
{
    public int Amount;
    public ElementTypeEnum ElementType;
}

#endregion

[RequireComponent(typeof(sBattleSounds))]
public class BattleManager : MonoBehaviour, IPointerClickHandler
{
    public static BattleManager instance;

    [Space(4)]
    [Header("Magic Camera")]
    [SerializeField] private sMagicCamera magicCamera;

    private Vector3[][] spawnPositions = new Vector3[][] {
        new Vector3[]{ new Vector3(0, 0, -5)},
        new Vector3[]{ new Vector3(-2, 0, -5), new Vector3(2, 0, -5)},
        new Vector3[]{ new Vector3(-4, 0, -5), new Vector3(0, 0, -5), new Vector3(4, 0, -5)},
    }; //1, 2, or 3 spawns

    private GameManager gM;
    private sBattleSounds battleSounds;

    private int inititiveOrder = 10; //1 more than possible by roll
    private List<int> initiativeTickets = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    public List<Character> combatants = new List<Character>();
    public List<Character> combatantsNonInitOrder = new List<Character>(); //?

    [Space(4)]
    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI currentText;

    [Space(4)]
    [Header("Camera")]
    [SerializeField] private Transform spotlight;

    [Space(4)]
    [Header("BattleUI & Buttons")]
    [SerializeField] private Transform actionPanel;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button magicButton;
    [SerializeField] private Button itemButton;
    [SerializeField] private Button runButton;

    [Space(4)]
    [Header("Menu Parents and Prefabs")]
    [SerializeField] private Transform originalSelectPanelParent;
    public GameObject genericSelectPanel;

    [Space(4)]
    [Header("Team Elements and Prefabs")]
    [SerializeField] private Transform teamPanel;
    [SerializeField] private GameObject teamMemberRowPrefab;

    [Header("Victory & Defeat")]
    [SerializeField] private GameObject victoryPanel;
    private BattleResults battleResults = new BattleResults();

    [Space(4)]
    [Header("Error Placeholder Prefab")]
    [SerializeField] private GameObject errorCharacterPrefab;

    //GETTERS and SETTERS
    public Character ActiveCharacter
    {
        get //Change this to reflect a descending wrapping inititive order.
        {
            return combatants.Find(character => character.Initiative == inititiveOrder);
        }
    }
    public List<Character> AICharacters
    {
        get { return combatants.FindAll(c => c is AICharacter); }
    }
    public List<Character> LivingAICharacters
    {
        get { return combatants.FindAll(c => c is AICharacter && !c.isDead); }
    }
    public List<Character> DeadAICharacters
    {
        get { return combatants.FindAll(c => c is AICharacter && c.isDead); }
    }
    public List<Character> PlayerCharacters
    {
        get { return combatants.FindAll(c => c is PlayerCharacter); }
    }
    public List<Character> LivingPlayerCharacters
    {
        get { return combatants.FindAll(c => c is PlayerCharacter && !c.isDead); }
    }
    public List<Character> DeadPlayerCharacters
    {
        get { return combatants.FindAll(c => c is PlayerCharacter && c.isDead); }
    }


    //MONOBEHAVIOR
    private void Awake()
    {
        //SINGLETON, NOT CARRIED FROM SCENE TO SCENE THOUGH.
        if (instance != null) { Destroy(gameObject); }
        else { instance = this; }
    }
    void Start()
    {
        gM = GameManager.instance;
        battleSounds = GetComponent<sBattleSounds>();
        LoadTeam();
        LoadEnemies();        

        //while two combatants have the same inititive, one must go higher.

        //list
        foreach (Character combatant in combatants)
        {
            print(combatant.Name + ": " + combatant.Initiative);
        }

        ToggleCombatButtons(false);
        StartCoroutine(FlyoverThenAdvanceTurn());
    }
    

    //SET UP
    private void LoadTeam()
    {
        for (int i = 0; i < gM.activeSave.teamMembers.Count; i++)
        {
            //Prep Teammate
            Character teammate = gM.activeSave.teamMembers[i];
            string teammateBattlePrefabName = gM.activeSave.teamMembers[i].PrefabDictionaryName;

            int initTicketIndex = Random.Range(0, initiativeTickets.Count);
            int initTicket = initiativeTickets[initTicketIndex];
            initiativeTickets.RemoveAt(initTicketIndex);            
            teammate.SetInitiative(initTicket);

            combatants.Add(teammate);

            //make ui rows for team members
            Instantiate(teamMemberRowPrefab, teamPanel).GetComponent<sTeamMemberInfoRow>().FillInfo(teammate);

            if (gM.gameObjectDictionary.TryGetValue(teammateBattlePrefabName, out GameObject gO))
            {
                GameObject tempTeamGo = Instantiate(gO, 
                    spawnPositions[gM.activeSave.teamMembers.Count-1][i], 
                    Quaternion.identity);
                teammate.animator = tempTeamGo.GetComponent<sBattleAnimator>();
            }
            else
            {
                GameObject tempTeamGo = Instantiate(errorCharacterPrefab, 
                    spawnPositions[gM.activeSave.teamMembers.Count - 1][i], 
                    Quaternion.identity);
                teammate.animator = tempTeamGo.GetComponent<sBattleAnimator>();
            }

        }
    }
    private void LoadEnemies()
    {
        List<Character> enemies = new List<Character>() { gM.GetRandomEnemy() };
        for (int i = 0; i < enemies.Count; i++)
        {
            Character enemy = enemies[i];
            string enemyBattlePrefabName = enemies[i].PrefabDictionaryName;

            int initTicketIndex = Random.Range(0, initiativeTickets.Count);
            int initTicket = initiativeTickets[initTicketIndex];
            initiativeTickets.RemoveAt(initTicketIndex);
            enemy.SetInitiative(initTicket);

            combatants.Add(enemy);

            if (gM.gameObjectDictionary.TryGetValue(enemyBattlePrefabName, out GameObject gO))
            {
                GameObject tempEnemyGo = Instantiate(gO,
                    spawnPositions[enemies.Count - 1][i] * -1,
                    Quaternion.Euler(new Vector3(0f, 180f, 0)));
                enemy.animator = tempEnemyGo.GetComponent<sBattleAnimator>();
            }
            else
            {
                GameObject tempEnemyGo = Instantiate(errorCharacterPrefab,
                    spawnPositions[enemies.Count - 1][i] * -1,
                    Quaternion.Euler(new Vector3(0f, 180f, 0)));
                enemy.animator = tempEnemyGo.GetComponent<sBattleAnimator>();
            }
        }
    }


    //BUTTONS AND TURN STATE SELECTORS
    private void ToggleCombatButtons(bool enabled)
    {
        actionPanel.localScale =  enabled ? Vector3.one : Vector3.zero;
        attackButton.interactable = enabled;
        magicButton.interactable = enabled;
        itemButton.interactable = enabled;
        runButton.interactable = enabled;
        spotlight.position = new Vector3(0, -100, 0);
    }
    public void AttackButton()
    {
        DestroyAllChildrenOf(originalSelectPanelParent);
        Instantiate(genericSelectPanel, originalSelectPanelParent).GetComponent<sGenericSelectPanel>().FillCharInfo(LivingAICharacters, new BattleChoice());
    }
    public void MagicButton()
    {
        DestroyAllChildrenOf(originalSelectPanelParent);
        Instantiate(genericSelectPanel, originalSelectPanelParent).GetComponent<sGenericSelectPanel>().FillSpellInfo(ActiveCharacter.SpellsKnown, new BattleChoice());

    }
    public void ItemButton()
    {
        DestroyAllChildrenOf(originalSelectPanelParent);
        Instantiate(genericSelectPanel, originalSelectPanelParent).GetComponent<sGenericSelectPanel>().FillItemInfo(gM.activeSave.inventory, new BattleChoice());
    }


    //CHARACTER DECISIONS
    public void FinalizeBattleChoice(BattleChoice choices)
    {
        //Prevent Extra Attacks by Player
        ToggleCombatButtons(false);

        switch (choices.Type)
        {
            case BattleChoiceTypeEnum.ATTACK:
                AddCurrentText(ActiveCharacter.Name + " attack on " + choices.Target.Name);
                break;
            case BattleChoiceTypeEnum.SPELL:
                ActiveCharacter.UseMP(choices.Spell.MPCost);
                AddCurrentText(ActiveCharacter.Name + " casts spell " + choices.Spell.Name + " on " + choices.Target.Name);
                break;
            case BattleChoiceTypeEnum.ITEM:
                gM.activeSave.UseItem(choices.Item);
                AddCurrentText(ActiveCharacter.Name + " uses item: " + choices.Item.Name + " on " + choices.Target.Name);
                break;
            default:
                Debug.LogError("Invalid spell, item, or attack choice on enemy selection");
                break;
        }

        //finally
        StartCoroutine(PauseCalcDamageThenAdvanceTurn(choices));
    }
    public void SkipTurn()
    {
        //battleSounds.PlayHmm();
        AdvanceTurn(); //No need for pause.
    }


    //COMBAT ENDERS
    public void RunButton()
    {
        DestroyAllChildrenOf(originalSelectPanelParent);
        DestroyAnimatorsOnTeammates();        
        gM.LoadRewardsScene(new BattleResults() {result = BattleResultEnum.RUN });
    }
    public void VictoryButton()
    {
        DestroyAnimatorsOnTeammates();
        gM.LoadRewardsScene(new BattleResults() { 
            booty = new List<Item>() {gM.GetRandomItem()},
            gold = gM.GetRandomGoldAmount(),
            result = BattleResultEnum.WIN,
            experience = 6000,
        });
    }
    public void GameOver()
    {
        DestroyAnimatorsOnTeammates();
        gM.LoadGameOverScene();
    }


    //COMBAT FLOW
    private void AdvanceTurn()
    {
        inititiveOrder--;
        while (ActiveCharacter == null)
        {
            inititiveOrder--;
            if (inititiveOrder < 0)
            {
                inititiveOrder = 10;
            }
        }
        //print("init order: " + inititiveOrder.ToString());

        //Advance
        //print(ActiveCharacter.Name + "'s turn.");

        if (CheckForWin())
        {
            print("win!");
            ToggleCombatButtons(false);
            victoryPanel.SetActive(true);
            return;
        }
        if (CheckForLoss())
        {
            print("loss!");
            GameOver();
            return;
        }

        if (ActiveCharacter.isDead)
        {
            print("Would have been " + ActiveCharacter.Name + "'s turn but he's dead.");
            SkipTurn();
            return;
        }

        //Focus Camera
        magicCamera.FocusCameraOn(ActiveCharacter);

        //Mark buttons, play ready sound.
        if (ActiveCharacter is PlayerCharacter)
        {
            battleSounds.PlayBell();
            ToggleCombatButtons(true);

            //await player input to advance again
            return;
        }
        else
        {
            ToggleCombatButtons(false);

            //make computer choice
            FinalizeBattleChoice( DetermineAIChoice() );
            return;
        }        
    }
    private bool CheckForWin()
    {
        //print("Remaining AI Chars: " + LivingAICharacters.Count.ToString());
        return LivingAICharacters.Count <= 0;
    }
    private bool CheckForLoss()
    {
        return LivingPlayerCharacters.Count <= 0;
    }
    private Character GetRandomCharacterFromList(List<Character> list)
    {
        int randInt = Random.Range(0, list.Count);
        return list[randInt];
    }


    //AI
    private BattleChoice DetermineAIChoice()
    {
        //Instantiate final choice as a basic attack
        BattleChoice finalChoice = new BattleChoice() {
            Item = null,
            Spell = null,
            Target = GetRandomCharacterFromList(LivingPlayerCharacters),
        };

        if (ActiveCharacter is PlayerCharacter)
        {
            Debug.LogError("Trying to determine an AI choice for a player controlled character!");
        }

        //Random enemy flubs go here? Ignore good stratagem 20% of the time and just attack?
        //never on bosses ofc

        //Cast Character as AICharacter?
        if (ActiveCharacter is AICharacter)
        {
            AICharacter tempAIChar = ActiveCharacter as AICharacter;
            foreach (Priority priority in tempAIChar.Priorities)
            {
                switch (priority.condition.relevance)
                {
                    case RelevanceEnum.TOSELF:
                        if (ActiveCharacter.ConditionObtains(priority.condition) && ChoiceIsPossible(priority.choice))
                        {
                            finalChoice = priority.choice;
                            finalChoice.Target = ActiveCharacter;
                            print("AI chose itself due to " + priority.condition.status.ToString());
                            return finalChoice;
                        }
                        break;
                    case RelevanceEnum.TOFRIENDLIES:
                        if (ChoiceIsPossible(priority.choice))
                        {
                            List<Character> aiTeamMatesWhoMayBeChosen = new List<Character>();
                            foreach (Character aiChar in AICharacters)
                            {
                                if (aiChar.ConditionObtains(priority.condition))
                                {
                                    aiTeamMatesWhoMayBeChosen.Add(aiChar);
                                }
                            }
                            if (aiTeamMatesWhoMayBeChosen.Count > 0)
                            {
                                finalChoice = priority.choice;
                                finalChoice.Target = GetRandomCharacterFromList(aiTeamMatesWhoMayBeChosen);
                            }
                        }
                        break;
                    case RelevanceEnum.TOENEMIES:
                        if (ChoiceIsPossible(priority.choice))
                        {
                            List<Character> playerCharsWhoMayBeChosen = new List<Character>();
                            foreach (Character playerChar in PlayerCharacters)
                            {
                                if (playerChar.ConditionObtains(priority.condition))
                                {
                                    playerCharsWhoMayBeChosen.Add(playerChar);
                                }
                            }
                            if (playerCharsWhoMayBeChosen.Count > 0)
                            {
                                finalChoice = priority.choice;
                                finalChoice.Target = GetRandomCharacterFromList(playerCharsWhoMayBeChosen);
                            }
                        }
                        break;
                    default:
                        Debug.LogError("No conditions apply to AI priorities. Basic Attack, chosen randomly holds as default");
                        break;
                }
            }
        }

        print(finalChoice.ToString());
        return finalChoice;
    }
    private bool ChoiceIsPossible(BattleChoice choice)
    {
        if (ActiveCharacter is PlayerCharacter)
        {
            Debug.LogError("Trying to determine AI choice validity for a player character!");
            return false;
        }
        if (choice.Spell == null && choice.Item == null)
        {
            return true;
        }
        else if (choice.Spell != null)
        {
            if (choice.Spell.MPCost > ActiveCharacter.MP)
            {
                return false;
            }
            Debug.Log("Choice was a spell with MP cost: " + choice.Spell.MPCost);
            return true;
        }
        else if (choice.Item != null)
        {
            Debug.LogError("AI character trying to use an item!");
            return false;
        }
        return true;
    }


    //COMBAT PAUSE
    private IEnumerator PauseCalcDamageThenAdvanceTurn(BattleChoice choices)
    {
        DestroyAllChildrenOf(originalSelectPanelParent);

        Damage damage = null;

        if (ActiveCharacter is PlayerCharacter)
        {
            PlayerCharacter pC = ActiveCharacter as PlayerCharacter;
            switch (choices.Type)
            {
                case BattleChoiceTypeEnum.ATTACK:
                    damage = pC.GetAttackDamage(choices);
                    ActiveCharacter.animator.PlayAttackAnimation(choices);
                    break;
                case BattleChoiceTypeEnum.SPELL:
                    damage = pC.GetSpellDamage(choices);
                    ActiveCharacter.animator.PlaySpellAnimation(choices);
                    break;
                case BattleChoiceTypeEnum.ITEM:
                    damage = pC.GetItemDamage(choices);
                    ActiveCharacter.animator.PlayItemAnimation(choices);
                    break;
                default:
                    break;
            }
        }
        else if (ActiveCharacter is AICharacter)
        {
            AICharacter aiC = ActiveCharacter as AICharacter;
            switch (choices.Type)
            {
                case BattleChoiceTypeEnum.ATTACK:
                    damage = aiC.GetAttackDamage(choices);
                    ActiveCharacter.animator.PlayAttackAnimation(choices);
                    break;
                case BattleChoiceTypeEnum.SPELL:
                    damage = aiC.GetSpellDamage(choices);
                    ActiveCharacter.animator.PlaySpellAnimation(choices);
                    break;
                case BattleChoiceTypeEnum.ITEM:
                    damage = aiC.GetItemDamage(choices);
                    ActiveCharacter.animator.PlayItemAnimation(choices);
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.LogError("CHARACTER NOT AI OR PLAYER?!");
        }

        //Wait until animation is complete
        //Therefore, camera should focus on the action here
        yield return new WaitUntil(() => ActiveCharacter.animator.AnimationFinished);
        DisplayDamageIndicator(choices, damage);

        if (choices.Target.TakeDamageReturnTrueIfDead(damage))
        {
            //print(choices.Target.Name + " died.");
            choices.Target.animator.PlayDeathAnimation();
        }
        yield return new WaitForSeconds(1.5f);

        //reset selections and advance
        AdvanceTurn();
    }
    private IEnumerator FlyoverThenAdvanceTurn()
    {
        yield return new WaitForSeconds(4f);
        AdvanceTurn();
    }


    //VIEW
    private void AddCurrentText(string text)
    {
        currentText.text = text;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //print(eventData.pointerPressRaycast.gameObject.name);
        string panelClicked = eventData.pointerPressRaycast.gameObject.name;
        if (panelClicked == "GUTTER")
        {
            //print("Close all optional windows");
            DestroyAllChildrenOf(originalSelectPanelParent);
        }
    }
    private void DisplayDamageIndicator(BattleChoice choices, Damage damage)
    {
        //print("did " + damage.Amount + " damage");
        Instantiate(GameManager.instance.gameObjectDictionary.GetValueOrDefault("DAMAGE_POPUP"),
            choices.TargetPosition + Vector3.up * choices.Target.animator.MeshSize.y + Vector3.up,
            Quaternion.identity,
            null
        ).GetComponent<sDamagePopup>().FillInfo(damage);
    }
    public void Spotlight(Vector3 newPos)
    {
        spotlight.position = newPos + Vector3.up * 3;
    }


    //DEBUG
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + Vector3.back * 5, Vector3.one);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.forward * 5, Vector3.one);
    }
    private void DEBUG_DECREMENT_ALL_HEALTH()
    {
        foreach (Character combatant in combatants)
        {
            combatant.Health--;            
        }
    }


    //UTILITY
    private void DestroyAllChildrenOf(Transform t)
    {
        foreach (Transform child in t.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void DestroyAnimatorsOnTeammates()
    {
        foreach (Character character in PlayerCharacters)
        {
            character.animator = null;
        }
    }
}
