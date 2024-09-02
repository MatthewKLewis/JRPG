using System.Collections;
using UnityEngine;

public class Resistances
{
    private int fireResist = 0;
    private int iceResist = 0;
    private int physicalResist = 0;
    private int lightningResist = 0;

    public int GetResistanceTo(ElementTypeEnum element)
    {
        switch (element)
        {
            case ElementTypeEnum.PHYSICAL:      return this.physicalResist;
            case ElementTypeEnum.FIRE:          return this.fireResist;
            case ElementTypeEnum.ICE:           return this.iceResist;
            default:                            return 0;
        }
    }

}
