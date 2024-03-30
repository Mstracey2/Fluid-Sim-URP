using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scrollmanager : MonoBehaviour
{
    [SerializeField] private List<GameObject> specifiedObjects = new List<GameObject>();

    public void SwitchScroll(bool on)
    {
        foreach (GameObject gameObj in specifiedObjects)
        {
            gameObj.gameObject.SetActive(on);
        }
    }
    
}
