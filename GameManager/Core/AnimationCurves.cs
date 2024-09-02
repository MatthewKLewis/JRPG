using System.Collections;
using UnityEngine;

public static class AnimationCurves
{
    public static AnimationCurve ATTACK_CURVE = new AnimationCurve(
        new Keyframe(0.0f, 0.0f),
        new Keyframe(0.2f, 1.0f),
        new Keyframe(0.8f, 1.0f),
        new Keyframe(1.0f, 0.0f)
    );

    //public static AnimationCurve JUMP_CURVE = new AnimationCurve(
    //    new Keyframe(0.0f, 0.0f),
    //    new Keyframe(0.25f, 1.0f),
    //    new Keyframe(0.5f, 0.0f),
    //    new Keyframe(0.75f, 1.0f),
    //    new Keyframe(0.0f, 0.0f)
    //);

}
