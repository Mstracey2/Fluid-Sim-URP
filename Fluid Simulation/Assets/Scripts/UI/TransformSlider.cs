using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*TRANSFORM SLIDER
 * 
 * Small script to update box transform from slider
 * calls the GPU matrix
 */

public class TransformSlider : MonoBehaviour
{
    #region Variables
    [SerializeField] private BoxTransformUI boxUI;
    private Slider slider;
    public BoxTransformUI.TransformType type;
    public BoxTransformUI.XYZ transformVector;
    #endregion

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
