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
        SetCameraToCenter();
        //cameraMode = MagicCameraMode.IDLE;
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
        //make sure not to pivot too far around
        //if (Mathf.Abs(transform.rotation.eulerAngles.y) > 30)
        //{
        //    ROTATE_SPEED = -ROTATE_SPEED;
        //}
        //transform.Rotate(Vector3.up * ROTATE_SPEED * Time.deltaTime);
    }

    public void FocusCameraOn(Character activeCharacter)
    {
        if (activeCharacter.PlayerControlled)
        {
            //SetCameraToFocus(activeCharacter.animator.transform.position, activeCharacter.animator.transform.rotation);
            SetCameraToCenter();
        }
        else
        {
            SetCameraToCenter();
        }
    }

    private void SetCameraToCenter()
    {
        transform.position = Vector3.zero;
        cameraTransform.localPosition = new Vector3(-9, 5, -9);
        cameraTransform.localRotation = Quaternion.Euler(20, 55, 0);
    }

    private void SetCameraToFocus(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
        cameraTransform.localPosition = new Vector3(0, 6, -6);
        cameraTransform.localRotation = Quaternion.Euler(20, 0, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(transform.position, new Vector3(10, 6, 0).magnitude);
    }
}
