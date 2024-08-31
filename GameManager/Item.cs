using System.Collections;
using UnityEngine;

public class Item
{
    public string Name;
    public string PrefabDictionaryName;

    public bool UsedOnEnemies;

    public Item(string name, bool usedOnEnemies)
    {
        this.Name = name;
        this.PrefabDictionaryName = name;
        this.UsedOnEnemies = usedOnEnemies;
    }
}
