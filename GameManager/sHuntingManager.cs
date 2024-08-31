using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sHuntingManager : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public Transform rifleTransform;

    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask mask;
    [SerializeField] private RaycastHit hit;

    void Start()
    {
        //Change cursor to 
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.farClipPlane;
        Vector3 aimPoint = cam.ScreenToWorldPoint(mousePos);
        rifleTransform.LookAt(aimPoint);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(cam.transform.position, (aimPoint - cam.transform.position), out hit, 100f, mask)) 
            {
                print(hit.point + hit.collider.gameObject.name);
            }
        }
    }

    public void ExitButton()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.ForceSoftware);
        print("Exiting");
    }
}
