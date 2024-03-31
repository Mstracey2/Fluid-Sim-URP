using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*BUTTON BUNCH
 * 
 * This script is my own functionality for Unity's UI buttons as I DONT LIKE UNITY'S BUTTONS.
 * It bunches buttons together so one button press can effect other button's highlights.
 * 
 * ISOLATE: Makes the button pressed the only highlighted button. (used for only allowing one feature active of many features)
 * 
 * TOGGLE: Toggles the pressed button on and off from selected. (used so features can be turned on and off)
 * 
 * INTERACTABLE: All buttons in list are deactivated or activated. (disables buttons to lock users out of features for different situations  E.G lock user from pressing the box button when in wide angle mode)
 */


public class ButtonBunch : MonoBehaviour
{
    [SerializeField] private List<Button> buttons = new List<Button>();
    private List<Image> images = new List<Image>();

    bool interacterSwitch = false;  //button deactivator

    private void Awake()
    {
        foreach(Button button in buttons)
        {
            images.Add(button.GetComponent<Image>());
        }
    }

    //ISOLATE: Makes the button pressed the only highlighted button. (used for only allowing one feature active of many features)
    public void Isolate(Button button)
    {
        foreach(Button listButton in buttons)
        {
            if (button != listButton)
            {
                images[buttons.IndexOf(listButton)].color = listButton.colors.normalColor;
            }
            else
            {
                images[buttons.IndexOf(button)].color = button.colors.selectedColor;
            }
        }

    }

    //TOGGLE: Toggles the pressed button on and off from selected. (used so features can be turned on and off)
    public void Toggle(Button button)
    {
        if (images[buttons.IndexOf(button)].color == button.colors.selectedColor)
        {
            images[buttons.IndexOf(button)].color = button.colors.normalColor;
        }
        else
        {
            images[buttons.IndexOf(button)].color = button.colors.selectedColor;
        }

    }

    //INTERACTABLE: All buttons in list are deactivated or activated. (disables buttons to lock users out of features for different situations  E.G lock user from pressing the box button when in wide angle mode)
    public void Interactable(Button button)
    {
        foreach (Button listButton in buttons)
        {
            if (button != listButton)       //makes sure not to deactivate the pressed button
            {
                listButton.interactable = interacterSwitch;
            }
            
        }
        interacterSwitch = !interacterSwitch;
    }

}
