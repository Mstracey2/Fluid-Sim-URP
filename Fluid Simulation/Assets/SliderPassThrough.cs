using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Values
{
    public Slider slider;
    public SPH simVal;
}

public class SliderPassThrough : MonoBehaviour
{
    public event EventHandler<Values> passThrough;
    [SerializeField] private SPH sim;
    public void InvokeEvent(Slider sl)
    {
        passThrough?.Invoke(this, new Values { slider = sl, simVal = sim });
    }

    public void PassThroughFloatValue(Slider slider, float fVal)
     {
        fVal = slider.value;
    }
}
