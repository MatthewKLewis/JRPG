using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class sDamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private AnimationCurve opacityCurve;
    [SerializeField] private AnimationCurve fontSizeCurve;
    private float time = 0f;

    private void Start()
    {
        
    }

    private void Update()
    {
        damageText.color = new Color(1, 1, 1, opacityCurve.Evaluate(time));
        damageText.fontSize = fontSizeCurve.Evaluate(time);
        time += Time.deltaTime;
    }

    public void FillInfo(Damage damage)
    {
        switch (damage.ElementType)
        {
            case (ElementTypeEnum.PHYSICAL):
                damageText.color = Color.white;
                break;
            case (ElementTypeEnum.FIRE):
                damageText.color = Color.red;
                break;
            case (ElementTypeEnum.ICE):
                damageText.color = Color.blue;
                break;
            default:
                break;
        }
        damageText.text = damage.Amount.ToString();
    }

}
