using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectRaycast : MonoBehaviour
{
    [Header("Raycast Parameters")]
    //[SerializeField] GiayTestRaycast giayTestRaycast;
    [SerializeField] private int rayLength = 5;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private string exludeLayerName = null;
    [SerializeField] CollideDoorFrame cdf;
    //[SerializeField] AddForce af;
    BasicDoorRaycast bdr;
    bool isCrosshairActive;
    //bool doOnce;
    //bool doOnce2;
    //float initD;
    //bool once;
    //bool avoidFail;

    private void Awake()
    {
        bdr = GetComponent<BasicDoorRaycast>();  
    }
    private void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        int mask = 1 << LayerMask.NameToLayer(exludeLayerName) | layerMaskInteract.value;



        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
        {
            if (hit.collider.CompareTag("DoorTrigger"))
            {
                if (!bdr.raycasted_obj.doOnce)
                {
                    //if (hit.collider.CompareTag("DoorTrigger"))
                    //{
                    if (bdr.raycasted_obj.haltIsNear && !bdr.raycasted_obj.openedOutside)
                    {
                        if (!bdr.raycasted_obj.once)
                        {
                            bdr.raycasted_obj.initD = hit.distance;
                            bdr.raycasted_obj.once = true;
                        }
                        StartCoroutine(avoidFaill());
                        if (hit.distance < bdr.raycasted_obj.initD && bdr.raycasted_obj.avoidFail)
                        {
                            bdr.raycasted_obj.AddForceNear1();
                            bdr.raycasted_obj.doOnce = true;
                        }

                        IEnumerator avoidFaill()
                        {
                            yield return new WaitForSeconds(0.4f);
                            bdr.raycasted_obj.avoidFail = true;
                        }
                    }
                    //}
                }
                if (!bdr.raycasted_obj.doOnce2)
                {
                    if (hit.distance < 0.35f)
                    {
                        bdr.raycasted_obj.AddForceNear2();
                        bdr.raycasted_obj.doOnce2 = true;
                    }
                }
                //isCrosshairActive = true;
                //doOnce = true;
            }
        }
        else
        {
            if (isCrosshairActive)
            {
                bdr.raycasted_obj.doOnce = false;
            }
        }

    }

}

