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

    public Weapon HeldWeapon;
    public List<Spell> SpellsKnown = new List<Spell>();

    public Resistances Resistances = new Resistances();
    public List<DebuffEnum> Debuffs = new List<DebuffEnum>();
    public List<BuffEnum> Buffs = new List<BuffEnum>();

    public List<Priority> Priorities = new List<Priority>();

    public int Initiative;
    public int BaseDamage;
    public bool isDead;
    public int Health;
    public int MaxHealth;
    public int MP;
    public int MaxMP;
    public int Level;
    public bool PlayerControlled;

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
        int damage = this.BaseDamage;

        //Weapon Damage
        if (HeldWeapon != null) { damage += this.HeldWeapon.Damage; }

        //Random Damage
        //

        return new Damage() { Amount = damage, ElementType = ElementTypeEnum.PHYSICAL};
    }

    public Damage GetSpellDamage(BattleChoice choices)
    {
        int damage = 0;
        damage += choices.Spell.Damage;
        damage += this.Level;
        return new Damage() { Amount = damage, ElementType = ElementTypeEnum.FIRE };
    }

    public Damage GetItemDamage(BattleChoice choices)
    {
        int damage = 0;
        damage += choices.Item.Damage;
        damage += this.Level;
        return new Damage() { Amount = damage, ElementType = ElementTypeEnum.FIRE };
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

    public Character ShallowCopy()
    {
        return (Character)MemberwiseClone();
    }
}
