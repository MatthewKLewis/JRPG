using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NewGameInformation
{
    public static string JOURNAL_START = "So I've finally arrived, but the records don't show from where...";

    public static List<Character> STARTING_CHARACTERS = new List<Character>()
    {
                new Character("Uef", 10, new Weapon("Axe", 3), true),
                new Character("Bi", 10, new Weapon("Spear", 3), true),
                new Character("Skripach", 10, new Weapon("Spear", 3), true),
    };
}