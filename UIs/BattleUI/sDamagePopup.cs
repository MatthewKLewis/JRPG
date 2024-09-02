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
    private float r = 1f;
    private float g = 1f;
    private float b = 1f;

    private void Start()
    {
        
    }

    private void Update()
    {
        damageText.color = new Color(r, g, b, opacityCurve.Evaluate(time));
        damageText.fontSize = fontSizeCurve.Evaluate(time);
        time += Time.deltaTime;
    }

    public void FillInfo(Damage damage)
    {
        switch (damage.ElementType)
        {
            case (ElementTypeEnum.PHYSICAL):
                r = 1f;
                g = 1f;
                b = 1f;
                break;
            case (ElementTypeEnum.FIRE):
                r = 1f;
                g = 0f;
                b = 0f;
                break;
            case (ElementTypeEnum.ICE):
                r = 0f;
                g = 0f;
                b = 1f;
                break;
            default:
                break;
        }
        damageText.text = damage.Amount.ToString();
    }

}
