using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseParticleMover : MonoBehaviour
{
    [SerializeField] private Camera cam;
    GameObject refPoint;
    
    public float farBounds;
    public float nearBounds;

    private float sensitivity;
    private float radius;

    public float Radius { set { radius = value; } }
    public float Sensitivity { set { sensitivity = value; } }

    float scrollVal;
    float offset;
    // Start is called before the first frame update
    void Start()
    {
        refPoint = this.gameObject;
        offset = nearBounds;

    }

    // Update is called once per frame
    void Update()
    {
        float originalOffset = offset;
        offset += scrollVal * sensitivity;
        CheckBounds(originalOffset);
        refPoint.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.z + offset));
        transform.localScale = new Vector3(radius, radius, radius);
    }

    public void Scroll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            scrollVal = context.ReadValue<float>();
        }
    }

    private void CheckBounds(float original)
    {
        if(offset > farBounds )
        {
            offset = original;
        }
        else if(offset <nearBounds)
        {
            offset = original;
        }
    }

}
