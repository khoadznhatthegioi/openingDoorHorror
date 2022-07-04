using UnityEngine;

public class CollideDoorFrame : MonoBehaviour
{
    [SerializeField] ObjectRaycast bdr; 

    private void Update()
    {
        if (bdr.raycasted_obj)
        {
            var currentDoor = bdr.raycasted_obj;
            if (currentDoor && currentDoor.collided && currentDoor.alreadyInside)
            {
                currentDoor.haltIsNear = true;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (bdr.raycasted_obj)
        {
            var currentDoor = bdr.raycasted_obj;
            if (other.tag == "DoorTrigger")
            {
                currentDoor.alreadyInside = true;
            }
            if (other.tag == "DoorTrigger" && bdr.raycasted_obj.collided && !currentDoor.haltIsNear)
            {
                currentDoor.isNear = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (bdr.raycasted_obj)
        {
            var currentDoor = bdr.raycasted_obj;
            if (other.tag == "DoorTrigger")
            {
                currentDoor.isNear = false;
                currentDoor.alreadyInside = false;
            }
        }
    }
}
