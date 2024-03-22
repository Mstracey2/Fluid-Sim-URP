using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoWideAngle : MonoBehaviour
{
    Camera cam;
    Vector3 camStartPos;
    Vector3 camStartRot;

    [SerializeField] private SPH sim;
    private Transform simBox;

    private Vector3 boxStartPos;
    private Vector3 boxStartScale;

    public Vector3 camWidePos;
    public Vector3 camWideRot;

    private float wideBox = 45;

    private bool switcher = true;

    // Start is called before the first frame update
    void Start()
    {
        simBox = sim.transform;
        cam = Camera.main;
        camStartPos = cam.transform.position;
        camStartRot = cam.transform.eulerAngles;
        boxStartPos = simBox.transform.position;
        boxStartScale = simBox.transform.localScale;
    }

    
    public void ToggleWideAngle()
    {
        if (switcher)
        {
            camWidePos = new Vector3(simBox.position.x, camWidePos.y, camWidePos.z);
            cam.transform.position = camWidePos;
            cam.transform.eulerAngles = camWideRot;
            simBox.position = boxStartPos;
            simBox.localScale = new Vector3(wideBox, wideBox, wideBox);
        }
        else
        {
            cam.transform.position = camStartPos;
            cam.transform.eulerAngles = camStartRot;
            simBox.localScale = new Vector3(boxStartScale.x, boxStartScale.y, boxStartScale.z);
        }

        switcher = !switcher;
        sim.SetGpuMatrix();
    }
}
