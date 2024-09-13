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
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider mpSlider;
    [SerializeField] private TextMeshProUGUI mpText;

    private void Update()
    {
        //maybe its okay to have this move the slider on update? TODO 
        healthSlider.value = ((float)character.Health / (float)character.MaxHealth);
        mpSlider.value = ((float)character.MP / (float)character.MaxMP);

        healthText.text = character.Health.ToString("000") + " / " + character.MaxHealth.ToString("000");
        mpText.text = character.MP.ToString("000") + " / " + character.MaxMP.ToString("000");
    }

    public void FillInfo(Character characterInfo)
    {
        character = characterInfo;

        nameText.text = character.Name;
        healthSlider.value = ((float)character.Health / (float)character.MaxHealth);
    }
}
