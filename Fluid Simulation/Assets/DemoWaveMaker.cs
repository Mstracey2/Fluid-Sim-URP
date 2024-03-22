using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoWaveMaker : MonoBehaviour
{
    [SerializeField] private SPH sim;
    private Transform simBox;
    bool waveBox;
    public float timeMultiplier;
    public float scaleMultiplier;
    private float increment;
    private float startScaleX;
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
            sim.SetGpuMatrix();
        }
    }

    public void TurnWaveBox()
    {
        startScaleX = simBox.localScale.x;
        waveBox = !waveBox;
    }
}
