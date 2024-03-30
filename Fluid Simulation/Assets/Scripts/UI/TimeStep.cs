using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeStep : MonoBehaviour
{
    [SerializeField] private SPH sim;
    Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetTimeStep()
    {
        sim.SetGpuTimeStep(slider.value);
    }

}
