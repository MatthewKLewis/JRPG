using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sPartyMemberRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Slider healthSlider;

    public void FillInfo(Character characterInfo)
    {
        nameText.text = characterInfo.Name;
        healthSlider.value = ((float)characterInfo.Health / (float)characterInfo.MaxHealth);
    }
}
