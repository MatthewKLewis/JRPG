using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sContainer : MonoBehaviour
{
    public Weapon weaponInContainer = ProcGen.RandomWeapon();
    public Item itemInContainer = ProcGen.RandomItem();
}
