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
            new Character("Rando (1)", 4, new Weapon("Club", 1), false),
            new Character("Rando (2)", 4, new Weapon("Club", 1), false),
            new Character("Rando (3)", 4, new Weapon("Club", 1), false),
        };
    }

    public static Weapon RandomWeapon()
    {
        return new Weapon("Bad Gun", 5);
    }
}
