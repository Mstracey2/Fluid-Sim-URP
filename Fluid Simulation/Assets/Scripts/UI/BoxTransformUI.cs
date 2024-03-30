using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxTransformUI : MonoBehaviour
{
    [SerializeField] private SPH sim;
    private Transform simBox;

    Vector3 startingPos;
    Vector3 startingRot;
    Vector3 startingScale;

    Vector3 offsetPos;
    Vector3 offsetRot;
    Vector3 offsetScale;

    public enum TransformType
    {
        Position,
        Rotation,
        Scale,
    }
    public enum XYZ
    {
        x,
        y,
        z,
    }

    private void Awake()
    {
        simBox = sim.transform;
        startingPos = simBox.position;
        startingRot = simBox.eulerAngles;
        startingScale = simBox.localScale;

    }

    public void ChangeTransform(TransformType trans, XYZ transXYZ, float value)
    {
        switch (trans)
        {
            case TransformType.Position:
                offsetPos = CheckXYZ(transXYZ, value, offsetPos);
                Vector3 newPos = SetValues(offsetPos,startingPos);
                simBox.transform.position = newPos;
                break;

            case TransformType.Rotation:
                offsetRot = CheckXYZ(transXYZ, value, offsetRot);
                Vector3 newRot = SetValues(offsetRot, startingRot);
                simBox.transform.eulerAngles = newRot;
                break;

            case TransformType.Scale:
                offsetScale = CheckXYZ(transXYZ, value, offsetScale);
                Vector3 newScale = SetValues(offsetScale,startingScale);
                simBox.transform.localScale = newScale;
                break;

            default:
                offsetPos = CheckXYZ(transXYZ, value, offsetPos);
                 newPos = SetValues(offsetPos, startingPos);
                simBox.transform.position = newPos;
                break;
        }

        sim.SetGpuMatrix();
    }

    private Vector3 SetValues(Vector3 newVals, Vector3 startingVect)
    {
        return new Vector3(startingVect.x + newVals.x, startingVect.y + newVals.y, startingVect.z + newVals.z);
    }

    private Vector3 CheckXYZ( XYZ transXYZ, float value, Vector3 originVect)
    {
        switch (transXYZ)
        {
            case XYZ.x:
                return new Vector3(value, originVect.y, originVect.z);

            case XYZ.y:
                return new Vector3(originVect.x, value, originVect.z);

            case XYZ.z:
                return new Vector3(originVect.x, originVect.y, value);

            default:
                return new Vector3(value, originVect.y, originVect.z);
        }
    }
}
