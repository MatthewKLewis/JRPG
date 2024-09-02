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

    public string journalContents;

    public List<Character> teamMembers;

    public int gold;

    private List<Item> _inventory = new List<Item>();
    private List<Weapon> _armaments = new List<Weapon>();
    public List<Item> Inventory { get => _inventory; }
    public List<Weapon> Armaments { get => _armaments; }

    public override string ToString()
    {
        return dateCreated + ", " + subSceneName;
    }

    public void AddArmament(Weapon newWep)
    {
        _armaments.Add(newWep);
    }

    public void AddItem(Item newItem)
    {
        foreach (Item item in _inventory)
        {
            if (item.Name == newItem.Name)
            {
                Debug.Log("incremented quantity of " + item.Name);
                item.Quantity++;
            }
            else
            {
                Debug.Log("added new stack of " + item.Name);
                _inventory.Add(newItem);
            }
        }
    }
}
