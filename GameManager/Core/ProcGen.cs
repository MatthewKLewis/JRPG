using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProcGen
{
    public static Character RandomEnemy()
    {
        return new Character("Rando", 5, new Weapon("Club", 1), false);
    }

    public static List<Character> RandomEnemies()
    {
        int randInt = Random.Range(1, 4);
        return new List<Character>()
        {
            new Character("Hardsuit", 12, new Weapon("Club", 1), false),
        };
    }

    public static Weapon RandomWeapon()
    {
        return new Weapon("9mm Handgun", 5);
    }
    public static Item RandomItem()
    {
        return new Item("Molotov", 5, true);
    }
}
