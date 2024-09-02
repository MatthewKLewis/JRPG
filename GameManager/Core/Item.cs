﻿using System.Collections;
using UnityEngine;

public class Item
{
    public string Name;
    public string PrefabDictionaryName;
    public int Quantity;

    public int Damage;

    public bool UsedOnEnemies;
    public bool UsedOnAll;
    public ElementTypeEnum ElementType;


    public Item(string name, int damage, bool usedOnEnemies)
    {
        this.Name = name;
        this.Damage = damage;
        this.PrefabDictionaryName = name;
        this.UsedOnEnemies = usedOnEnemies;
    }
}
