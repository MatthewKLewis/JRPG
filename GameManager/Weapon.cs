using System.Collections;
using UnityEngine;

public class Weapon
{
    public string Name;
    public int Damage;

    public Weapon(string name, int damage)
    {
        this.Name = name;
        this.Damage = damage;
    }
}
