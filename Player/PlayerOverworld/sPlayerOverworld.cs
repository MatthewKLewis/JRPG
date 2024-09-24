using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
public class sPlayerOverworld : MonoBehaviour
{
    private GameManager gM;
    private CharacterController cC;
    private float playerSpeed = 4f;
    private float rotateSpeed = 80f;

    //Potential Assets to Act Upon
    private sDoor doorAffordance;

    private Vector3 lastTurnPosition;
    private float distanceTraveled = 0f;
    private float combatDistance = 10f;

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
            if (doorAffordance != null)
            {
                gM.LoadInteriorScene(doorAffordance.interiorSubSceneName, doorAffordance.newScenePosition);
            }
        }

        if (GameConstants.CombatChanceOverworld(distanceTraveled))
        {
            gM.LoadBattleScene(transform.position);
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        lastTurnPosition = transform.position;

        Vector3 inputVector = new Vector3(
            0, 0, //Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0, 0,
            Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0
            ).normalized * playerSpeed * Time.deltaTime;

        Vector3 inputRotate = new Vector3( 0, Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1 : 0, 0) * rotateSpeed * Time.deltaTime;

        if (!cC.isGrounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out worldHit, 100f, worldLayerMask))
            {
                inputVector.y = -1;
            }
        }

        //Move and Rotate
        transform.Rotate(inputRotate);
        cC.Move(transform.TransformDirection(inputVector));

        //Movement change
        distanceTraveled += (transform.position - lastTurnPosition).magnitude;

        //World Wrapping
        if (transform.position.x > 250) { print("teleport E-W"); transform.position = new Vector3(-249.9f, transform.position.y, transform.position.z); }
        if (transform.position.x < -250) { print("teleport W-E"); transform.position = new Vector3(249.9f, transform.position.y, transform.position.z); }

        if (transform.position.z > 250) { print("teleport N-S"); transform.position = new Vector3(transform.position.x, transform.position.y, -249.9f); }
        if (transform.position.z < -250) { print("teleport S-N"); transform.position = new Vector3(transform.position.x, transform.position.y, 249.9f); }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case ("Door"):
                Actions.OnProximityToInteractable(true, "Enter");
                doorAffordance = other.GetComponent<sDoor>();
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
                doorAffordance = null;
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
