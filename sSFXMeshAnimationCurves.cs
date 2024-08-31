using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSFXMeshAnimationCurves : MonoBehaviour
{
    private MeshRenderer mR;
    [SerializeField] private AnimationCurve opacityCurve;
    [SerializeField] private AnimationCurve scaleCurve;
    private float time = 0f;

    private void Start()
    {
        mR = GetComponent<MeshRenderer>();
        
        mR.material.color = Color.clear;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        mR.material.color = new Color(1, 1, 1, opacityCurve.Evaluate(time));
        transform.localScale = Vector3.one * scaleCurve.Evaluate(time);
        time += Time.deltaTime;
    }
}