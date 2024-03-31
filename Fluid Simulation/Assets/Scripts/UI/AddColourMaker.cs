using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*ADD COLOUR MAKER
 * 
 * The functionality to add colour makers to rendering UI.
 * 
 * THIS ALSO NOW REMOVES COLOUR MAKERS ASWELL.
 * 
 */

public class AddColourMaker : MonoBehaviour
{
    [SerializeField] private GameObject contentsParent; //parent that the colour makers are attached to (keeps the makers in a scroll list in the UI)
    [SerializeField] private GameObject colourMaker;    //colour maker prefab
    [SerializeField] private RenderingUI rendUI;        //holds gradient updating functions
    
    [SerializeField] private Button removeButton;
    private Button addButton;


    private int gradientCount = 3;  //Max gradient keys are 8.

    private void Awake()
    {
        addButton = GetComponent<Button>();
    }

    public void AddGradientColour()
    {
        if (gradientCount < 8)
        {
            if (removeButton.interactable == false) { removeButton.interactable = true; }   //reactivates button if gradient count is below max

            GameObject newColourMaker = Instantiate(colourMaker, contentsParent.transform);
            ColourMaker maker = newColourMaker.GetComponent<ColourMaker>();
            newColourMaker.transform.SetSiblingIndex(gradientCount+2);          //aligns maker to correct pos in UI
            maker.rendUI = rendUI;
            gradientCount++;

            if (gradientCount == 8) { addButton.interactable = false; } //deactivates button if gradient count is at max
        }
    }

    public void RemoveGradientColour()
    {
        if(gradientCount > 2)
        {
            if (addButton.interactable == false) { addButton.interactable = true; } //reactivates button if gradient count is above min

            Destroy(rendUI.uiGradientColours[gradientCount - 1].gameObject);
            rendUI.uiGradientColours.RemoveAt(gradientCount - 1);
            rendUI.UpdateGradient();
            gradientCount--;

            if (gradientCount == 2) { removeButton.interactable = false; }  //deactivates button if gradient count is at min
        }
    }
}
