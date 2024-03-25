using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    [SerializeField] private SPH sim;
    public void QuitGame()
    {
        sim.ReleaseBuffers();
        Application.Quit();
    }
}
