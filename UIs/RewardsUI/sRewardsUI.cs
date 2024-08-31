using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class sRewardsUI : MonoBehaviour
{
    private GameManager gM;
    [SerializeField] private TextMeshProUGUI text;

    void Start()
    {
        gM = GameManager.instance;
    }

    public void ProceedButton()
    {
        gM.LoadLastInteriorScene();
    }
}
