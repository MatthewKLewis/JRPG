using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sPartyMemberRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider mpSlider;

    public void FillInfo(PlayerCharacter characterInfo)
    {
        nameText.text = characterInfo.Name;
        levelText.text = "LVL " + characterInfo.Level.ToString();
        healthText.text = characterInfo.Health + " / " + characterInfo.MaxHealth;
        mpText.text = characterInfo.MP + " / " + characterInfo.MaxMP;
        healthSlider.value = ((float)characterInfo.Health / (float)characterInfo.MaxHealth);
        mpSlider.value = ((float)characterInfo.MP / (float)characterInfo.MaxMP);
    }
}
