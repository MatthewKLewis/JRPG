using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sOrthographicTrackingCamera : MonoBehaviour
{
    private GameObject playerGameObject;

    void Start()
    {
        playerGameObject = GameObject.Find("Player_Interior(Clone)");
    }

    void Update()
    {
        if (playerGameObject)
        {
            transform.position = playerGameObject.transform.position;
        }
        else
        {
            Debug.LogWarning("NEVER FOUND PLAYER GO");
            playerGameObject = GameObject.Find("Player_Interior(Clone)");
        }
    }
}
