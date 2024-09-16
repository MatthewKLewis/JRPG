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
        teamMembers = new List<PlayerCharacter>()
        {
            new PlayerCharacter()
            {
                Name = "Hero",
                ID = 1,
                PrefabDictionaryName = "Hero",
                animator = null,
                HeldWeapon = new Weapon()
                {
                    Name = "Club",
                    Damage = 2,
                },
                SpellsKnown = new List<Spell>()
                {
                    new Spell()
                    {
                        Name="Fireball",
                        PrefabDictionaryName="Fireball",
                        MPCost=5,
                        Damage=5,
                        UsedOnEnemies=true,
                        UsedOnAll=false,
                        ElementType=ElementTypeEnum.FIRE,
                    }
                },
                Resistances = new Resistances(),
                Debuffs = new List<DebuffEnum>(){ },
                Buffs = new List<BuffEnum>(){ },
                Initiative = 1,
                isDead = false,
                MP = 10,
                MaxMP = 10,
                Health = 24,
                MaxHealth = 24,
                Level = 1,
                Experience = 0,
            },
        },
        journalContents = "I've finally arrived, but the records don't show from where.",
        inventory = new List<Item>() { },
        gold = 20,
    };

    public static int[] LevelUpThresholds = new int[] { 1000, 2000, 4000, 8000, 16000, 32000, 64000, 128000, 256000, 512000, 1024000, 2048000, 4096000 };
}