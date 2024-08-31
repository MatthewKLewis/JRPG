using System.Collections;
using UnityEngine;

public class Spell
{
    public string Name;
    public string PrefabDictionaryName;

    public bool UsedOnEnemies;
    public ElementTypeEnum ElementType;
        
    public Spell(string name, bool usedOnEnemies)
    {
        this.Name = name;
        this.PrefabDictionaryName = name;
        this.UsedOnEnemies = usedOnEnemies;
    }
}