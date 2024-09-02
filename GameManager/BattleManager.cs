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

public enum BattleChoiceEnum
{
    ATTACK = 0,
    SPELL = 1,
    ITEM = 2,
}

public enum BattleResultEnum
{
    WIN = 0,
    LOSS = 1,
    RUN = 2,
}

public class BattleChoice
{
    public Character Target = null;
    public Spell Spell = null;
    public Item Item = null;

    public BattleChoiceEnum Type { 
        get => Spell == null && Item == null ? BattleChoiceEnum.ATTACK : Item == null ? BattleChoiceEnum.SPELL : BattleChoiceEnum.ITEM;
    }
    public Vector3 TargetPosition
    {
        get => this.Target.animator.gameObject.transform.position;
    } 
}

public class BattleResults
{
    public BattleResultEnum result;
    public List<Item> booty;
    public int gold;
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
        new Vector3[]{ new Vector3(2, 0, -5), new Vector3(-2, 0, -5)},
        new Vector3[]{ new Vector3(4, 0, -5), new Vector3(0, 0, -5), new Vector3(-4, 0, -5)},
    }; //1, 2, or 3 spawns

    private GameManager gM;
    private sBattleSounds battleSounds;
    private int turnsTaken = -1;

    public List<Character> combatants = new List<Character>();

    [Space(4)]
    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI currentText;
    [SerializeField] private TextMeshProUGUI activeCharacterText;

    [Space(4)]
    [Header("Camera")]
    [SerializeField] private Transform spotlight;

    [Space(4)]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button magicButton;
    [SerializeField] private Button itemButton;
    [SerializeField] private Button runButton;


    [SerializeField] private Transform originalSelectPanelParent;
    public GameObject genericSelectPanel;

    [Header("Victory Button")]
    [SerializeField] private GameObject victoryPanel;

    [Space(4)]
    [Header("Error Placeholder Prefab")]
    [SerializeField] private GameObject errorCharacterPrefab;

    [Space(4)]
    [Header("UI Elements and Prefabs")]
    [SerializeField] private Transform teamPanel;
    [SerializeField] private GameObject teamMemberRowPrefab;


    //GETTERS and SETTERS
    public Character ActiveCharacter
    {
        get 
        {
            if (turnsTaken < 0) return null;
            return combatants[turnsTaken % combatants.Count];
        }
    }
    public List<Character> EnemyCharacters
    {
        get { return combatants.FindAll(c => !c.PlayerControlled); }
    }
    public List<Character> LivingEnemyCharacters
    {
        get { return combatants.FindAll(c => !c.PlayerControlled && !c.isDead); }
    }
    public List<Character> DeadEnemyCharacters
    {
        get { return combatants.FindAll(c => !c.PlayerControlled && c.isDead); }
    }
    public List<Character> FriendlyCharacters
    {
        get { return combatants.FindAll(c => c.PlayerControlled); }
    }
    public List<Character> LivingFriendlyCharacters
    {
        get { return combatants.FindAll(c => c.PlayerControlled && !c.isDead); }
    }
    public List<Character> DeadFriendlyCharacters
    {
        get { return combatants.FindAll(c => c.PlayerControlled && c.isDead); }
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

        //Order combatants by initiative
        combatants.Sort((x,y) => x.Initiative.CompareTo(y.Initiative));

        //list
        foreach (Character combatant in combatants)
        {
            //print(combatant.Name + ": " + combatant.Initiative);
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
            combatants.Add(teammate);
            teammate.RollInitiative();

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
        List<Character> enemies = ProcGen.RandomEnemies();

        //Prep enemy
        foreach (Character enemy  in enemies)
        {
            enemy.RollInitiative();            
        }

        //sort by init
        enemies.Sort((x, y) => x.Initiative.CompareTo(y.Initiative));

        //add to combatants & load models for them
        for (int i = 0; i < enemies.Count; i++)
        {
            Character enemy = enemies[i];
            combatants.Add(enemy);
            string enemyBattlePrefabName = enemies[i].PrefabDictionaryName;

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
        attackButton.interactable = enabled;
        magicButton.interactable = enabled;
        itemButton.interactable = enabled;
        runButton.interactable = enabled;
        spotlight.position = new Vector3(0, -100, 0);
    }
    public void AttackButton()
    {
        DestroyAllChildrenOf(originalSelectPanelParent);
        Instantiate(genericSelectPanel, originalSelectPanelParent).GetComponent<sGenericSelectPanel>().FillCharInfo(LivingEnemyCharacters, new BattleChoice());
    }
    public void MagicButton()
    {
        DestroyAllChildrenOf(originalSelectPanelParent);
        Instantiate(genericSelectPanel, originalSelectPanelParent).GetComponent<sGenericSelectPanel>().FillSpellInfo(ActiveCharacter.SpellsKnown, new BattleChoice());

    }
    public void ItemButton()
    {
        DestroyAllChildrenOf(originalSelectPanelParent);
        Instantiate(genericSelectPanel, originalSelectPanelParent).GetComponent<sGenericSelectPanel>().FillItemInfo(gM.activeSave.Inventory, new BattleChoice());
    }


    //CHARACTER DECISIONS
    public void FinalizeBattleChoice(BattleChoice choices)
    {
        //Prevent Extra Attacks by Player
        ToggleCombatButtons(false);

        switch (choices.Type)
        {
            case BattleChoiceEnum.ATTACK:
                AddCurrentText(ActiveCharacter.Name + " attack on " + choices.Target.Name);
                break;
            case BattleChoiceEnum.SPELL:
                AddCurrentText(ActiveCharacter.Name + " uses " + choices.Spell.Name + " on " + choices.Target.Name);
                break;
            case BattleChoiceEnum.ITEM:
                AddCurrentText(ActiveCharacter.Name + " casts spell: " + choices.Item.Name + " on " + choices.Target.Name);
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
        gM.LoadRewardsScene(new BattleResults() { result = BattleResultEnum.WIN, gold = 10, booty = new List<Item>() { } });
    }
    public void GameOver()
    {
        DestroyAnimatorsOnTeammates();
        gM.LoadGameOverScene();
    }


    //COMBAT FLOW
    private void AdvanceTurn()
    {
        turnsTaken++;

        if (CheckForWin())
        {
            //battleSounds.PlayBell();
            ToggleCombatButtons(false);
            victoryPanel.SetActive(true);
            return;
        }
        if (CheckForLoss())
        {
            GameOver();
            return;
        }

        //Advance
        print(ActiveCharacter.Name + "'s turn.");

        //Focus Camera
        magicCamera.FocusCameraOn(ActiveCharacter);

        if (ActiveCharacter.isDead)
        {
            print("Would have been " + ActiveCharacter.Name + "'s turn but he's dead.");
            SkipTurn();
            return;
        }

        //Mark buttons, play ready sound.
        if (ActiveCharacter.PlayerControlled)
        {
            battleSounds.PlayBell();
            ToggleCombatButtons(true);
            activeCharacterText.text = ActiveCharacter.Name;

            //await player input to advance again
            return;
        }
        else
        {
            ToggleCombatButtons(false);
            activeCharacterText.text = "";

            //make computer choice - attack random
            FinalizeBattleChoice( new BattleChoice() { Target = GetRandomFriendlyCharacter() });
            return;
        }        
    }
    private bool CheckForWin()
    {
        return LivingEnemyCharacters.Count <= 0;
    }
    private bool CheckForLoss()
    {
        return LivingFriendlyCharacters.Count <= 0;
    }
    private Character GetRandomFriendlyCharacter()
    {
        int randInt = Random.Range(0, LivingFriendlyCharacters.Count);
        return LivingFriendlyCharacters[randInt];
    }


    //COMBAT PAUSE
    private IEnumerator PauseCalcDamageThenAdvanceTurn(BattleChoice choices)
    {
        DestroyAllChildrenOf(originalSelectPanelParent);

        Damage damage = null;
        switch (choices.Type)
        {
            case BattleChoiceEnum.ATTACK:
                damage = ActiveCharacter.GetAttackDamage(choices);
                ActiveCharacter.animator.PlayAttackAnimation(choices);
                break;
            case BattleChoiceEnum.SPELL:
                damage = ActiveCharacter.GetSpellDamage(choices);
                ActiveCharacter.animator.PlaySpellAnimation(choices);
                break;
            case BattleChoiceEnum.ITEM:
                damage = ActiveCharacter.GetItemDamage(choices);
                ActiveCharacter.animator.PlayItemAnimation(choices);
                break;
            default:
                break;
        }

        //Wait until animation is complete
        //Therefore, camera should focus on the action here
        yield return new WaitUntil(() => ActiveCharacter.animator.AnimationFinished);
        DisplayDamageIndicator(choices, damage);

        if (choices.Target.TakeDamageReturnTrueIfDead(damage))
        {
            print(choices.Target.Name + " died.");
            choices.Target.animator.PlayDeathAnimation();
        }
        yield return new WaitForSeconds(1.5f);

        //reset selections and advance
        AdvanceTurn();
    }
    private IEnumerator FlyoverThenAdvanceTurn()
    {
        yield return new WaitForSeconds(3.5f);
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
        foreach (Character character in FriendlyCharacters)
        {
            character.animator = null;
        }
    }
}
