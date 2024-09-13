using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class sRewardsUI : MonoBehaviour
{
    private GameManager gM;

    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI xpText;

    void Start()
    {
        gM = GameManager.instance;
    }

    public void ProceedButton()
    {
        gM.LoadScenePriorToBattle();
    }

    public void DisplayBattleResults(BattleResults battleResults)
    {
        switch (battleResults.result)
        {
            case BattleResultEnum.WIN:
                resultText.text = "Victory";
                foreach (PlayerCharacter teammate in gM.activeSave.teamMembers)
                {
                    if (!teammate.isDead)
                    {
                        teammate.EarnXPReturnTrueIfLevelUp(battleResults.experience);
                    }
                }
                break;
            case BattleResultEnum.RUN:
                resultText.text = "Ran Away!";
                break;
            default:
                resultText.text = "ERROR!";
                break;
        }
        goldText.text = battleResults.gold.ToString() + " chatls gained";
        xpText.text = battleResults.experience.ToString() + " xp gained";
    }
}
