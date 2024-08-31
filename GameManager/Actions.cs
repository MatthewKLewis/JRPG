using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions
{
    //overworld player to UI
    //public static Action<float, float, float> OnOverworldMovement;

    //interior player to UI
    public static Action<bool, string> OnProximityToInteractable;

    public static Action<sNPC> OnConversationStart;
    public static Action OnConversationEnd;

    public static Action<Weapon> OnItemReceived;
}
