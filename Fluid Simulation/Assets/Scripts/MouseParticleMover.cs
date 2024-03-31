using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/*
 * MOUSE PARTICLE MOVER
 * 
 * A script used to Allow the user more interactions with the fluid through the mouse, 
 * user can push and pull particles from the mouse radius.
 */


public class MouseParticleMover : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private GameObject refPoint; // mouse game object
    
    //far and near bounds for how far the mouse can be scrolled on the X transform
    public float farBounds;     
    public float nearBounds;

    private float sensitivity;
    public float Sensitivity { set { sensitivity = value; } }


    float scrollVal;    //current scroll sensitivity value
    float offset;       //Original position

    // Start is called before the first frame update
    void Start()
    {
        refPoint = this.gameObject;
        offset = nearBounds;
    }

    // Update is called once per frame
    void Update()
    {
        refPoint.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.z + offset));
    }

    public void Scroll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            scrollVal = context.ReadValue<float>();

            float originalOffset = offset;      //old pos
            offset += scrollVal * sensitivity;  // new pos

            CheckBounds(originalOffset);        //checks if new pos is in bounds
        }
    }

    private void CheckBounds(float original)
    {
        //if offset is beyond bounds, return to original position.
        if(offset > farBounds)
        {
            offset = original;
        }
        else if(offset < nearBounds)
        {
            offset = original;
        }
    }

    public void Radius(float newRadius)
    {
        transform.localScale = new Vector3(newRadius, newRadius, newRadius);
    }

}
