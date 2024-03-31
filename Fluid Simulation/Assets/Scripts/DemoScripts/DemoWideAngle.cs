using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * DEMO WIDE ANGLE 
 * 
 * A script used to demonstrate the interesting capabilities of the project, 
 * this gives the user a different bird's eye view of the water physics.
 */


public class DemoWideAngle : MonoBehaviour
{
    [SerializeField] private SPH sim;
    private Transform simBox;

    private Camera cam;

    /*
     * Below are the vectors used for the transform changes, 
     * this includes holding the original transform data to return to.
     */
    private Vector3 camStartPos;
    private Vector3 camStartRot;
    private Vector3 boxStartPos;
    private Vector3 boxStartScale;

    // wide rotate and position for the camera
    public Vector3 camWidePos;
    public Vector3 camWideRot;

    private float wideBox = 45; //change of box scale

    private bool switcher = true;

    // Start is called before the first frame update
    void Start()
    {
        simBox = sim.transform;
        cam = Camera.main;

        //gets all original transform data
        camStartPos = cam.transform.position;
        camStartRot = cam.transform.eulerAngles;
        boxStartPos = simBox.transform.position;
        boxStartScale = simBox.transform.localScale;
    }

    
    public void ToggleWideAngle()
    {
        if (switcher) //Change to wide angle
        {
            //camera change
            camWidePos = new Vector3(simBox.position.x, camWidePos.y, camWidePos.z);
            cam.transform.position = camWidePos;
            cam.transform.eulerAngles = camWideRot;
            //box change
            simBox.position = boxStartPos;
            simBox.localScale = new Vector3(wideBox, wideBox, wideBox);
        }
        else  //Return to original
        {
            //camera change
            cam.transform.position = camStartPos;
            cam.transform.eulerAngles = camStartRot;
            //box change
            simBox.localScale = new Vector3(boxStartScale.x, boxStartScale.y, boxStartScale.z);
        }

        switcher = !switcher;

        sim.SetGpuMatrix(); //update compute shader box transform matrix
    }
}
