using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
public class sPlayerOverworld : MonoBehaviour
{
    private GameManager gM;
    private CharacterController cC;
    private float playerSpeed = 5f;

    private string potentialNextSubSceneName = "";

    //private float distanceTraveled = 0f;

    [SerializeField] private LayerMask worldLayerMask;
    private RaycastHit worldHit;

    void Start()
    {
        gM = GameManager.instance;
        cC = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (potentialNextSubSceneName != "")
            {
                gM.LoadInteriorScene(potentialNextSubSceneName);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 inputVector = new Vector3(
            Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0, 0,
            Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0
            ).normalized * playerSpeed * Time.deltaTime;

        if (!cC.isGrounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out worldHit, 100f, worldLayerMask))
            {
                //print(worldHit.point);
                inputVector.y = -1;
            }
        }

        //set velocity
        cC.Move(inputVector);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case ("Door"):
                Actions.OnProximityToInteractable(true, "Enter");
                potentialNextSubSceneName = other.GetComponent<sDoor>().interiorSubSceneName;
                break;
            case ("NPC"):
                //who?
                break;
            default:
                print("trigger error");
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case ("Door"):
                Actions.OnProximityToInteractable(false, "");
                potentialNextSubSceneName = "";
                break;
            case ("NPC"):
                //who?
                break;
            default:
                print("trigger error");
                break;
        }
    }
}
