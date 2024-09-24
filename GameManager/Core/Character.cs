using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public string Name;
    public int ID;
    public string PrefabDictionaryName;

    //NON-SERIALIZED! Injected upon battle start!
    //MUST be deleted after battle scenes before saving.
    public sBattleAnimator animator;

    public List<Spell> SpellsKnown = new List<Spell>();
    public Resistances Resistances = new Resistances();
    public List<DebuffEnum> Debuffs = new List<DebuffEnum>();
    public List<BuffEnum> Buffs = new List<BuffEnum>();

    public int Initiative;
    public bool isDead;
    public int Health;
    public int MaxHealth;
    public int MP;
    public int MaxMP;

    public void SetInitiative(int init)
    {
        Initiative = init;
    }

    public void UseMP(int usedMP)
    {
        this.MP -= usedMP;
        //Debug.Log("Consumed " + usedMP);

        if (this.MP < 0)
        {
            Debug.LogError("Used more MP than had!");
        }
    }

    public Damage GetAttackDamage(BattleChoice choices)
    {
        Debug.LogWarning("USING ANCESTOR CLASS DAMAGE CALC!");
        return new Damage() { Amount = 1, ElementType = ElementTypeEnum.PHYSICAL};
    }

    public Damage GetSpellDamage(BattleChoice choices)
    {
        Debug.LogWarning("USING ANCESTOR CLASS DAMAGE CALC!");
        return new Damage() { Amount = choices.Spell.Damage, ElementType = ElementTypeEnum.FIRE };
    }

    public Damage GetItemDamage(BattleChoice choices)
    {
        Debug.LogWarning("USING ANCESTOR CLASS DAMAGE CALC!");
        return new Damage() { Amount = choices.Item.Damage, ElementType = ElementTypeEnum.FIRE };
    }

    public bool TakeDamageReturnTrueIfDead(Damage damage)
    {
        Health -= damage.Amount;
        isDead = this.Health <= 0;
        return isDead;
    }

    public bool ConditionObtains(Condition condition)
    {
        switch (condition.status)
        {
            case CharacterStatusEnum.ANY:
                return true;
            case CharacterStatusEnum.ISDEAD:
                return isDead;
            case CharacterStatusEnum.ISLOWERTHAN50:
                //Debug.Log((float)Health / (float)MaxHealth);
                return (Health / MaxHealth) < 0.50f;
            case CharacterStatusEnum.ISLOWERTHAN25:
                //Debug.Log((float)Health / (float)MaxHealth);
                return ((float)Health / (float)MaxHealth) < 0.25f;
            case CharacterStatusEnum.ISPOISONED:
                return Debuffs.Contains(DebuffEnum.POISONED);
            default:
                return false; //true?
        }
    }

}

public class PlayerCharacter : Character
{
    public int Level;
    public int Experience;
    public Weapon HeldWeapon;

    public bool EarnXPReturnTrueIfLevelUp(int xp)
    {
        Experience += xp;

        while (Experience > GameConstants.LevelUpThresholds[Level])
        {
            Level++;
            Debug.Log(Name + " got a level up");
        }

        return false;
    }

    public new Damage GetAttackDamage(BattleChoice choices)
    {
        return new Damage() { Amount = HeldWeapon.Damage + Level, ElementType = ElementTypeEnum.PHYSICAL };
    }
}

public class AICharacter : Character
{
    public int BaseDamage;
    public List<Priority> Priorities = new List<Priority>();

    public AICharacter ShallowCopy()
    {
        return (AICharacter)MemberwiseClone();
    }

    public new Damage GetAttackDamage(BattleChoice choices)
    {
        return new Damage() { Amount = BaseDamage, ElementType = ElementTypeEnum.PHYSICAL };
    }
}

