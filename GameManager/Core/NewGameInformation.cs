using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NewGameInformation
{
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
        teamMembers = new List<Character>() 
        {
            new Character()
            {
                Name = "Hero",
                ID = 1,
                PrefabDictionaryName = "Hero",
                animator = null,
                BaseDamage = 3,
                HeldWeapon = new Weapon()
                {
                    Name = "Club",
                    Damage = 2,
                },
                Resistances = new Resistances(),
                Debuffs = new List<DebuffEnum>(){ },
                Buffs = new List<BuffEnum>(){ },
                Priorities = null,
                Initiative = 1,
                isDead = false,
                Health = 10,
                MaxHealth = 10,
                Level = 1,
                PlayerControlled = true
            },
        },
        journalContents = "I've finally arrived, but the records don't show from where.",
        inventory = new List<Item>() { },
        gold = 20,
    };
}