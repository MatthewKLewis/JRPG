using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sNPC : MonoBehaviour
{
    public string utterance = "";
    public Vector3 spotOverHead;

    private void Start()
    {
        spotOverHead = transform.position + (Vector3.one * 3) ;//GetComponentInChildren<CapsuleCollider>().bounds.size;
    }
}
