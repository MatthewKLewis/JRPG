using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
public class sPlayerInterior : MonoBehaviour
{
    private GameManager gM;
    private CharacterController cC;
    private float playerSpeed = 7.5f;

    [SerializeField] private Transform meshTransform;

    //Potential Assets to Act Upon
    private sDoor doorAffordance;
    private sGate gateAffordance;
    private sNPC npcAffordance;
    private sButton buttonAffordance;
    private sContainer containerAffordance;

    void Start()
    {
        gM = GameManager.instance;
        cC = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (containerAffordance != null) {
                Actions.OnItemReceived(containerAffordance.weaponInContainer);
                gM.activeSave.armaments.Add(containerAffordance.weaponInContainer);
            }
            else if (npcAffordance != null) {
                Actions.OnConversationStart(npcAffordance);
            }
            else if (buttonAffordance != null) {
                print("do button");
            }
            else if (gateAffordance != null) {
                gM.LoadOverworldScene(gateAffordance.overworldPosition);
            }
            else if (doorAffordance != null) {
                gM.LoadInteriorScene(doorAffordance.interiorSubSceneName);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 inputVector = new Vector3(
            Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0, 0, 
            Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0
            ).normalized * playerSpeed * Time.deltaTime;

        //set velocity
        cC.Move(inputVector);
        meshTransform.LookAt(transform.position + inputVector);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case ("Door"):
                Actions.OnProximityToInteractable(true, "Enter");
                doorAffordance = other.GetComponent<sDoor>();
                break;
            case ("Gate"):
                Actions.OnProximityToInteractable(true, "Leave");
                gateAffordance = other.GetComponent<sGate>();
                break;
            case ("Container"):
                Actions.OnProximityToInteractable(true, "Open");
                containerAffordance = other.GetComponent<sContainer>();
                break;
            case ("NPC"):
                Actions.OnProximityToInteractable(true, "Speak");
                npcAffordance = other.GetComponent<sNPC>();
                break;
            case ("Monster"): //SPECIAL CASE!!!
                gM.LoadBattleScene();
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
                doorAffordance = null;
                break;
            case ("Gate"):
                Actions.OnProximityToInteractable(false, "");
                gateAffordance = null;
                break;
            case ("Container"):
                Actions.OnProximityToInteractable(false, "");
                containerAffordance = null;
                break;
            case ("NPC"):
                Actions.OnProximityToInteractable(false, "");
                Actions.OnConversationEnd(); //walking away ends conversations
                npcAffordance = null;
                break;
            default:
                Actions.OnProximityToInteractable(false, "");
                print("trigger error");
                break;
        }
    }
}
