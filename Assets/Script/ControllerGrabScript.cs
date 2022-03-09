using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerGrabScript : MonoBehaviour
{
    //handType, controllerPose and grabAction store references to the hand type and actions:
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean grabAction;
    
    private GameObject collidingObject; //stores the GameObject the trigger is colliding with
    private GameObject objectInHand; //serves as a reference to the object currently being grabbed

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (grabAction.GetLastStateDown(handType))
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }
        if (grabAction.GetLastStateUp(handType))
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
    }

    //trigger methods:
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }
        collidingObject = null;
    }

    private void SetCollidingObject(Collider col)
    {
       //if there is already a colliding object or if the other collider object doesnt have a rigidbody, dont need to set the colliding object repeatedly
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject; //set colldiing object
    }

    //methods to grab and release objects:
    private void GrabObject()
    {
        objectInHand = collidingObject; //set grabbedObject
        collidingObject = null; //once object has been grabbed (by controller), then make collidingObject available again

        var joint = AddFixedJoint(); //connect object to the controller
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>(); //get rigidbody of grabbedObject - adding fixed joint to grabbedOject(objectInHand), to connect the grabbedOject to the controller
    }
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>(); //adding a fixed joint to the related gameObject
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null; //disconnect the object from current controller
            Destroy(GetComponent<FixedJoint>());

            objectInHand.GetComponent<Rigidbody>().velocity = controllerPose.GetVelocity();
            objectInHand.GetComponent<Rigidbody>().angularVelocity = controllerPose.GetAngularVelocity();
        }
        objectInHand = null; //drop and dereference the grabbedObject
    }

}
