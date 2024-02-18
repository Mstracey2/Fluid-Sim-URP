using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseParticleMover : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private SPH sim;
    GameObject refPoint;
    public float sensitivity;
    public float farBounds;
    public float nearBounds;

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
        transform.localScale = new Vector3(sim.mouseRadius, sim.mouseRadius, sim.mouseRadius);
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
