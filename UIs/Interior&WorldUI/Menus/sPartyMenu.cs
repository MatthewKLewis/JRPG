using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPartyMenu : MonoBehaviour
{
    [SerializeField] private GameObject partyMemberRowPrefab;

    void Start()
    {
        //Instantiate Rows
        foreach (PlayerCharacter teamMember in GameManager.instance.activeSave.teamMembers)
        {
            Instantiate(partyMemberRowPrefab, this.transform).GetComponent<sPartyMemberRow>().FillInfo(teamMember);
        }
    }
}
