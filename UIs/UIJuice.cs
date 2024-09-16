using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIJuice
{
    private static AnimationCurve aC = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 2, 2), new Keyframe(1, 1, -1.6f, -1.6f), });

    public static IEnumerator SquashAndStretchRoutine(Transform t)
    {
        float TIME_RANGE = 0.25f;

        float time = 0f;
        while (time < TIME_RANGE)
        {
            time += Time.deltaTime;
            t.localScale = Vector3.one * aC.Evaluate(time / TIME_RANGE);
            yield return null;
        }
    }
}
