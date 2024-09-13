using System.Collections;
using UnityEngine;

public class Spell
{
    public string Name;
    public string PrefabDictionaryName;

    public string Description;

    public int MPCost;
    public int Damage;
    public bool UsedOnEnemies; 
    public bool UsedOnAll;
    public ElementTypeEnum ElementType;        
}