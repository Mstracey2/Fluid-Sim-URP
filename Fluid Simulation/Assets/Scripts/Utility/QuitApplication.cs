using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    [SerializeField] private SPH sim;

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        //To avoid crashing, the buffers must be released.
        sim.ReleaseBuffers();
    }
}
