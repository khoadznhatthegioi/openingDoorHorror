using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class BasicDoorRaycast : MonoBehaviour
{
    [Header("Raycast Parameters")]
    //[SerializeField] GiayTestRaycast giayTestRaycast;
    [SerializeField] private int rayLength = 5;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private string exludeLayerName = null;
    [SerializeField] public FirstPersonController player;
    
    [SerializeField] Camera mainCamera;

    [SerializeField] public BasicDoorController raycasted_obj;

    [Header("UI Parameters")]
    [SerializeField] private Image crosshair = null;
    public bool isCrosshairActive;
    public bool doOnce;
   // public bool openedDoor, openedDoor1, openedDoor2;


    private const string interactableTag = "InteractiveObject";
 

    [SerializeField] CollideDoorFrame cdf;
 

    private void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        int mask = 1 << LayerMask.NameToLayer(exludeLayerName) | layerMaskInteract.value;



        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
        {

           

            if (hit.collider.CompareTag("DoorHinge"))
            {
                if (!doOnce)
                {
                    raycasted_obj = hit.collider.gameObject.GetComponent<BasicDoorController>();
                    CrosshairChange(true);
                }

                isCrosshairActive = true;
                doOnce = true;



                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    raycasted_obj.GetComponent<Rigidbody>().isKinematic = false;
                    raycasted_obj.openedDoor = true;
                    raycasted_obj.openedDoor1 = true;
                    raycasted_obj.openedDoor2 = true;
                    raycasted_obj.alreadyOpened = true;
                }
            }
        }

        else
        {
            if (isCrosshairActive)
            {
                CrosshairChange(false);
                doOnce = false;
            }
        }
    }

    void CrosshairChange(bool on)
    {
        if (on && !doOnce)
        {
            crosshair.color = Color.red; 
        }
        else
        { 
            crosshair.color = Color.white;
            
            raycasted_obj.halt = false;
            isCrosshairActive = false;
        }
    }
}