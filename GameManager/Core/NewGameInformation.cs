using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NewGameInformation
{
    public static string JOURNAL_START = "So I've finally arrived, but the records don't show from where...";

    public static List<Character> STARTING_CHARACTERS = new List<Character>()
    {
        new Character("Uef", 10, new Weapon("Axe", 3), true){ ID = 1 },
        new Character("Bi", 10, new Weapon("Spear", 3), true){ ID = 2 },
        new Character("Skripach", 10, new Weapon("Spear", 3), true){ ID = 3 },
    };

    public static List<Item> STARTING_ITEMS = new List<Item>()
    {
        new Item("Potion", -5, false),
        new Item("Molotov", 5, true)
    };

    public static SaveGame STARTING_SAVE_FILE = new SaveGame()
    {
        dateCreated = DateTime.Now.ToString(),

        //LOCATIONS
        subSceneName = "HouseSubScene",
        onOverworldMap = false,
        x = 0,
        y = 0,
        z = 0,

        //LISTS
        teamMembers = STARTING_CHARACTERS,
        journalContents = JOURNAL_START,
        inventory = STARTING_ITEMS,
        gold = 20,
    };
}