using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame
{
    public string dateCreated;
    public string subSceneName;
    public bool onOverworldMap;
    public float x;
    public float y;
    public float z;

    public string journalContents;

    public List<Character> teamMembers;

    public int gold;

    public List<Item> inventory = new List<Item>();
    public List<Weapon> armaments = new List<Weapon>();

    public override string ToString()
    {
        return dateCreated + ", " + subSceneName;
    }

    public void AddArmament(Weapon newWep)
    {
        armaments.Add(newWep);
    }

    public void AddItem(Item newItem)
    {
        foreach (Item item in inventory)
        {
            if (item.Name == newItem.Name)
            {
                Debug.Log("incrementing: " + item.Name);
                item.Quantity++;
                return;
            }            
        }
        
        //If no match was found...
        inventory.Add(newItem);
        Debug.Log("added new stack of " + newItem.Name);
    }

    public void UseItem(Item usedItem)
    {
        Item itemToDelete = null;
        foreach (Item item in inventory)
        {
            if (usedItem.Name == item.Name)
            {
                //decrement
                if (item.Quantity > 1)
                {
                    Debug.Log("decrementing: " + item.Name);
                    item.Quantity--;
                    return;
                }
                else
                {
                    itemToDelete = item;
                }
            }
        }
        if (itemToDelete != null)
        {
            Debug.Log("removing stack of: " + itemToDelete.Name);
            inventory.Remove(itemToDelete);
        }
        else
        {
            Debug.LogError("tried to remove non-existing item: " + itemToDelete.Name);
        }
    }
}
