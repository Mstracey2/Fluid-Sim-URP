using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SetupVariableFrame : MonoBehaviour
{
    public UnityEvent firstTimeSetup;
    void Update()
    {
        if (Time.frameCount > 2)
        {
            firstTimeSetup.Invoke();
            Destroy(this);
        }
    }
}
