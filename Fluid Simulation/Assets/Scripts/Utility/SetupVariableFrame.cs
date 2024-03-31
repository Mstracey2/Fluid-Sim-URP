using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*SETUP VARIABLE FRAME
 * 
 *On load up, all UI scroll objects are active, to allow the values to be set from the sliders for the first time.
 *after a couple of frames, it turns off the UI scrolls except the default one.
 *
 *Acknowledged this is not the best way about it but this small startup script resolves having to change a lot of code.
 *if game objects are disabled at startup, it will cause values not to be set on start.
 */


public class SetupVariableFrame : MonoBehaviour
{
    public UnityEvent firstTimeSetup;

    void Update()
    {
        if (Time.frameCount > 2)
        {
            firstTimeSetup.Invoke();    //disables the UI scrolls.

            Destroy(this);  // not needed after this so bye bye!
        }
    }
}
