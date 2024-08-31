using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame
{
    //
    public string dateCreated;
    public string subSceneName;
    public bool onOverworldMap;
    public float x;
    public float y;
    public float z;

    //
    public List<Character> teamMembers;

    //
    public int gold;
    public List<Item> inventory;
    public List<Weapon> armaments;

    public override string ToString()
    {
        return dateCreated + ", " + subSceneName;
    }
}
