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
    public bool isDead;
    public int Health;
    public int MaxHealth;
    public int Level;
    public bool PlayerControlled;

    public Character(string name, int health, Weapon heldWeapon, bool playerControlled)
    {
        this.Name = name;
        this.PrefabDictionaryName = name;

        this.Health = health;
        this.MaxHealth = health;
        this.isDead = health <= 0;

        this.HeldWeapon = heldWeapon;
        this.PlayerControlled = playerControlled;

        this.Initiative = -1;
        this.Level = 1;

        this.SpellsKnown = new List<Spell>() { 
            new Spell("Fireball", true, 3, ElementTypeEnum.FIRE), 
            new Spell("Heal", false, -5, ElementTypeEnum.ICE) 
        };
    }

    public void RollInitiative()
    {
        Initiative = Random.Range(0, 11);
    }

    public Damage GetAttackDamage(BattleChoice choices)
    {
        int damage = 0;
        damage += this.HeldWeapon.Damage;
        damage += this.Level;
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
            case ConditionStatusEnum.ISDEAD:
                return isDead;
            case ConditionStatusEnum.ISLOWERTHAN50:
                return (Health / MaxHealth) < 0.50f;
            case ConditionStatusEnum.ISLOWERTHAN25:
                return (Health / MaxHealth) < 0.25f;
            case ConditionStatusEnum.ISPOISONED:
                return Debuffs.Contains(DebuffEnum.POISONED);
            default:
                return false; //true?
        }
    }
}
