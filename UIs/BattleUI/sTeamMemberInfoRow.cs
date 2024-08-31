using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sTeamMemberInfoRow : MonoBehaviour
{
    private Character character;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Slider healthSlider;

    private void Update()
    {
        //maybe its okay to have this move the slider on update? TODO 
        healthSlider.value = ((float)character.Health / (float)character.MaxHealth);
    }

    public void FillInfo(Character characterInfo)
    {
        character = characterInfo;

        nameText.text = character.Name;
        healthSlider.value = ((float)character.Health / (float)character.MaxHealth);
    }
}
