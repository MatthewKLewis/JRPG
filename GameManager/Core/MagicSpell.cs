using System.Collections;
using UnityEngine;

public class Spell
{
    public string Name;
    public string PrefabDictionaryName;

    public int MPCost;

    public int Damage;

    public bool UsedOnEnemies; 
    public bool UsedOnAll;
    public ElementTypeEnum ElementType;
        
    public Spell(string name, bool usedOnEnemies, int damage, ElementTypeEnum element)
    {
        this.Name = name;
        this.Damage = damage;
        this.PrefabDictionaryName = name;
        this.UsedOnEnemies = usedOnEnemies;
        this.ElementType = element;
    }
}