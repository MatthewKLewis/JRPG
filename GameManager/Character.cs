using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public string Name;
    public string PrefabDictionaryName;

    //NON-SERIALIZED! Injected upon battle start!
    public sBattleAnimator animator;

    public Weapon HeldWeapon;

    public List<Spell> SpellsKnown = new List<Spell>();

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

        this.SpellsKnown = new List<Spell>() { new Spell("Fireball", true), new Spell("Heal", false) };
    }

    public void RollInitiative()
    {
        Initiative = Random.Range(0, 11);
    }

    public Damage GetAttackDamage(BattleChoice choices)
    {
        return new Damage() { Amount = this.HeldWeapon.Damage, ElementType = ElementTypeEnum.PHYSICAL};
    }

    public Damage GetSpellDamage(BattleChoice choices)
    {
        return new Damage() { Amount = 5, ElementType = ElementTypeEnum.FIRE };
    }

    public Damage GetItemDamage(BattleChoice choices)
    {
        return new Damage() { Amount = 10, ElementType = ElementTypeEnum.FIRE };
    }

    public bool TakeDamageReturnTrueIfDead(Damage damage)
    {
        Health -= damage.Amount;
        isDead = this.Health <= 0;
        return isDead;
    }
}
