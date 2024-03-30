using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBunch : MonoBehaviour
{
    [SerializeField] private List<Button> buttons = new List<Button>();
    private List<Image> images = new List<Image>();

    bool interacterSwitch = false;

    private void Awake()
    {
        foreach(Button button in buttons)
        {
            images.Add(button.GetComponent<Image>());
        }
    }

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
                Debug.Log("isolating " + button.name);
            }
        }

    }

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

    public void Interactable(Button button)
    {
        foreach (Button listButton in buttons)
        {
            if (button != listButton)
            {
                listButton.interactable = interacterSwitch;
            }
            
        }
        interacterSwitch = !interacterSwitch;
    }

}
