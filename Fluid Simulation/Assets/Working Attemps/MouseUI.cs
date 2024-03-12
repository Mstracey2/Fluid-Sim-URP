using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class MouseUI : MonoBehaviour
{
    [SerializeField] private SPH sim;
    [SerializeField] private MouseParticleMover mouseMover;

    public void UpdateSensitivity(Slider slider)
    {
        mouseMover.sensitivity = slider.value;
    }
    public void UpdateRadius(Slider slider)
    {
        sim.mouseRadius = slider.value;
    }
    public void UpdateForce(Slider slider)
    {
        sim.pushPullForce = slider.value;
    }
}
