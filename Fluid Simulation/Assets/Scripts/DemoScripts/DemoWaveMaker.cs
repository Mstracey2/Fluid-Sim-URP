using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * DEMO WAVE MAKER 
 * 
 * A script used to demonstrate the interesting capabilities of the project, 
 * ping pong the x scale of the sim box to make the fluid create waves. 
 */


public class DemoWaveMaker : MonoBehaviour
{
    [SerializeField] private SPH sim;
    private Transform simBox;

    bool waveBox;                   //switcher(on or off)
    private float startScaleX;      //reference of the starting scale
    private float increment;

    public float timeMultiplier;    //how fast the box scales
    public float scaleMultiplier;   //how large the scale change of the box.


    private void Awake()
    {
        simBox = sim.transform;
    }

    private void Update()
    {
        if (waveBox)
        {
            increment = Mathf.PingPong(Time.time * timeMultiplier, scaleMultiplier);
            simBox.localScale = new Vector3(startScaleX + increment, simBox.localScale.y, simBox.localScale.z);
            
            sim.SetGpuMatrix(); // sends transform data to the compute shader to update the box boundaries
        }
    }

    public void TurnWaveBox()   //called from a UI button
    {
        startScaleX = simBox.localScale.x;
        waveBox = !waveBox;
    }
}
