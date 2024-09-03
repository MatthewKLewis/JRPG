using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MagicCameraMode
{
    IDLE = 0,
    FOCUS = 1,
    VICTORY = 2,
    INTRO = 3,
}

public class sMagicCamera : MonoBehaviour
{
    //private MagicCameraMode cameraMode;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform cameraTransform;

    private float ROTATE_SPEED = 1f;

    void Start()
    {
        //cameraMode = MagicCameraMode.IDLE;
        transform.localScale = Vector3.one * 3f;
    }

    void Update()
    {
        IdleCameraUpdate();

        //switch (cameraMode)
        //{
        //    case MagicCameraMode.IDLE:
        //        break;
        //    case MagicCameraMode.FOCUS:
        //        break;
        //    case MagicCameraMode.VICTORY:
        //        break;
        //    case MagicCameraMode.INTRO:
        //        break;
        //    default:
        //        break;
        //}
    }

    private void IdleCameraUpdate()
    {
        transform.Rotate(Vector3.up * ROTATE_SPEED * Time.deltaTime);
    }

    public void FocusCameraOn(Character activeCharacter)
    {
        if (activeCharacter.PlayerControlled)
        {
            ROTATE_SPEED = -ROTATE_SPEED;
            transform.position = activeCharacter.animator.transform.position;
            transform.rotation = activeCharacter.animator.transform.rotation;
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one * 3f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(transform.position, new Vector3(10, 6, 0).magnitude);
    }
}
