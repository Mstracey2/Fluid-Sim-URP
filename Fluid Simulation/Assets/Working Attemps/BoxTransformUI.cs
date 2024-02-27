using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxTransformUI : MonoBehaviour
{
    [SerializeField] private Transform simBox;

    [Header("Position Sliders")]
    [SerializeField] private Slider xPosSl;
    [SerializeField] private Slider yPosSl;
    [SerializeField] private Slider zPosSl;
   
    [Header("Rotation Sliders")]
    [SerializeField] private Slider xRotSl;
    [SerializeField] private Slider yRotSl;
    [SerializeField] private Slider zRotSl;

    [Header("Scale Sliders")]
    [SerializeField] private Slider xScaleSl;
    [SerializeField] private Slider yScaleSl;
    [SerializeField] private Slider zScaleSl;

    private float posX;
    private float posY;
    private float posZ;

    private float rotX;
    private float rotY;
    private float rotZ;

    private float scaleX;
    private float scaleY;
    private float scaleZ;

    private Vector3 originalPos;
    private Quaternion originalRot;
    private Vector3 originalScale;



    private void Awake()
    {
        originalPos = simBox.transform.position;
        originalRot = simBox.transform.rotation;
        originalScale = simBox.transform.localScale;
    }

    public void Pos()
    {
        SetValues(ref posX, ref posY, ref posZ, xPosSl, yPosSl, zPosSl, originalPos);
        simBox.transform.position = new Vector3(posX, posY,posZ);
    }

    public void Rot()
    {
        SetValues(ref rotX, ref rotY, ref rotZ, xRotSl, yRotSl, zRotSl, originalRot.eulerAngles);
        simBox.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
    }

    public void Scale()
    {
        SetValues(ref scaleX, ref scaleY, ref scaleZ, xScaleSl, yScaleSl, zScaleSl, originalScale);
        simBox.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }

    private void SetValues(ref float x, ref float y, ref float z,Slider xSl, Slider ySl, Slider zSl , Vector3 original)
    {
        x = original.x + xSl.value;
        y = original.y + ySl.value;
        z = original.z + zSl.value;
    }
}
