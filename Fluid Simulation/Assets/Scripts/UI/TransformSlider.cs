using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformSlider : MonoBehaviour
{
    [SerializeField] private BoxTransformUI boxUI;
    private Slider slider;
    public BoxTransformUI.TransformType type;
    public BoxTransformUI.XYZ transformVector;

    private void Start()
    {
        slider = GetComponent<Slider>();
        SetTransformValue();
    }

    public void SetTransformValue()
    {
        boxUI.ChangeTransform(type, transformVector, slider.value);
    }

}
