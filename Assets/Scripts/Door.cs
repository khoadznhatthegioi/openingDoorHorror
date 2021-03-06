using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Door : MonoBehaviour
{
    public enum DoorType : int
    {
        Nothing = 0,
        NonClosable = 1,
        Closable = 2,
        Autoclose = 3

    }
    public DoorType doorType;
    Rigidbody rb;
    [SerializeField] ObjectRaycast bdr;
    [SerializeField] Image crosshair;
    [HideInInspector] public float originalY;
    [HideInInspector] public bool openedOutside;
    [HideInInspector] public bool changed;
    [HideInInspector] public bool collided;
    [HideInInspector] public bool passedDoor;

    [HideInInspector] public bool openedDoor, openedDoor1, openedDoor2, alreadyOpened, openedPassMinAngle;
    [HideInInspector] public bool haltIsNear, alreadyInside, isNear, halt, reached, startAutoRotateToMax, startAutoClose, l;

    public bool doOnce;
    public bool doOnce2;
    public float initD;
    public bool once;
    public bool avoidFail;

    [Range(0f, 1f)]
    public float forcePushOpen = 0.2f;
    [Range(0f, 1f)]
    public float forceLookUp = 0.6f;
    [Range(0f, 1f)]
    public float forceInteract = 1f;
    [Range(0f, 360f)]
    public float maxAngleOpen = 115;
    [Range(0f, 50f)]
    float speed;
    public float angleEnd;
    HingeJoint hinge;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
        JointLimits limits = new JointLimits();
        hinge = GetComponent<HingeJoint>();
        originalY = transform.eulerAngles.y;
        limits.min = 0;
        limits.max = maxAngleOpen;
        GetComponent<HingeJoint>().limits = limits;
    }

    private void FixedUpdate()
    {
        if(openedDoor && !changed)
        {
            OpenForce();
        }

        if  (!halt && bdr.contacting)
        {   
            bdr.raycasted_obj.l = false;
            bdr.raycasted_obj.halt = true;
        }


        if (((int)doorType) == 1)
        {
            if (transform.eulerAngles.y >= originalY + angleEnd && !l)
            {
                speed = rb.angularVelocity.y * 180 / (Mathf.PI);
                startAutoRotateToMax = true;
                rb.isKinematic = true;
                l = true;
            }

        }
        else if ((int)doorType == 2)
        { 
            if (alreadyOpened && transform.eulerAngles.y >= originalY + 10)
            {
                openedPassMinAngle = true;
            }
        }




        if (reached)
        {
            StartCoroutine(untilStop());
            IEnumerator untilStop()
            {
                yield return new WaitForSeconds(0.1f);
                if (rb.angularVelocity.y >= -0.001 && rb.angularVelocity.y <= 0)
                {
                    rb.isKinematic = true;
                    reached = false;
                }
            }

        }

        if (passedDoor)
        {
            if (transform.eulerAngles.y < originalY + angleEnd)
            {
                rb.isKinematic = true;
                startAutoRotateToMax = true;

            }
        }
        if (startAutoRotateToMax)
        {
            float yVelocity = 0f;
            float smooth = 0.03f;
            RotateTowards(hinge.limits.max, ref yVelocity, smooth, speed);
        }

        if (openedPassMinAngle)
        {
            if (transform.eulerAngles.y < originalY + 5)
            {
                startAutoClose = true;
                speed = Mathf.Abs(rb.angularVelocity.y*180/(Mathf.PI));
                rb.isKinematic = true;
                openedPassMinAngle = false;
            }
        }


        if (startAutoClose)
        { 
            float yVelocity = 0f;
            float smooth = 0.03f;
            RotateTowards(originalY, ref yVelocity, smooth, speed);
            if (transform.eulerAngles.y >= originalY - 0.001 && transform.eulerAngles.y <= originalY + 0.001) 
            {
                startAutoClose = false;
                tag = "DoorHinge";
                ResetBools();
            }
        }

        if (isNear && openedDoor1)
        {
            AddForceNear1();
        }

    
    } 

    void RotateTowards(float yTarget, ref float yVelocity, float smooth, float speed)
    {
        float yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, yTarget, ref yVelocity, smooth, speed);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, yAngle, transform.eulerAngles.z);
    }
    void ResetBools()
    {

        openedOutside = changed = collided = openedDoor = openedDoor1 = openedDoor2 = alreadyOpened = openedPassMinAngle = alreadyInside = haltIsNear = isNear = halt = reached = startAutoRotateToMax = startAutoClose = l = doOnce = doOnce2 = once = avoidFail = false;
    }

    void OpenForce()
    {
        var xzpair = FindXZMultiplier(gameObject.transform.eulerAngles.y);
        rb.AddForce(new Vector3(xzpair[0], 0, xzpair[1]) * forcePushOpen, ForceMode.Impulse);
        Debug.Log("ican");
        openedDoor = false;
        collided = true;
        gameObject.tag = "Opened";
        crosshair.color = Color.white;
        if (!alreadyInside)
        {
            openedOutside = true;
        }
        changed = true;
    }

    public void AddForceNear1()
    { 
        var xzpair = FindXZMultiplier(gameObject.transform.eulerAngles.y); 
        rb.AddForce(new Vector3(xzpair[0], 0, xzpair[1]) * forceLookUp, ForceMode.Impulse);
        openedDoor1 = false;
        //isAddedOnce = true;
    }

    public void AddForceNear2()
    { 
        var xzpair = FindXZMultiplier(gameObject.transform.eulerAngles.y); 
        rb.AddForce(new Vector3(xzpair[0], 0, xzpair[1]) * forceInteract, ForceMode.Impulse);
        openedDoor2 = false;
    }

    float[] FindXZMultiplier(float doorAngle)
    {
        float[] xzpair = { 0, 0 };
        if (doorAngle < 180)
        {
            if (doorAngle < 90)
                xzpair[1] = 1 / (1 + Mathf.Tan(doorAngle * Mathf.PI / 180));
            else if (doorAngle >= 90)
            {
                xzpair[1] = -(1 / (1 + Mathf.Tan((180 - doorAngle) * Mathf.PI / 180)));
            }
            xzpair[0] = 1 - Mathf.Abs(xzpair[1]);
        }
        else if (doorAngle >= 180)
        {
            if (doorAngle < 270)
            {
                xzpair[1] = -(1 / (1 + Mathf.Tan((180 - (360 - doorAngle)) * Mathf.PI / 180)));
            }
            else if (doorAngle >= 270)
            {
                xzpair[1] = 1 / (1 + Mathf.Tan(((360 - doorAngle)) * Mathf.PI / 180));
            }
            xzpair[0] = -1 + Mathf.Abs(xzpair[1]);
        }
        return xzpair;
    }

}
