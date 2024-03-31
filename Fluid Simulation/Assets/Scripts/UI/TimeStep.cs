using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*TIME STEP
 * 
 * Small script for updating the time step from slider
 */

public class TimeStep : MonoBehaviour
{
    #region Variables
    [SerializeField] private SPH sim;
    Slider slider;
    #endregion

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetTimeStep()
    {
        sim.SetGpuTimeStep(slider.value);
    }

}
