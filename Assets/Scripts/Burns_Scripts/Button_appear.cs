using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This is code for clicking the buttons and having images associated 
//with the buttons appear. 

public class ShowBoxOnClick : MonoBehaviour
{
    public List<GameObject> objectsToShow;

    public void ShowBox()
    {
        // 1. Hide everything controlled by any button
        ShowBoxOnClick[] allButtons = FindObjectsOfType<ShowBoxOnClick>();
        foreach (ShowBoxOnClick btn in allButtons)
        {
            foreach (GameObject obj in btn.objectsToShow)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        // 2. Show all objects belonging to THIS button
        foreach (GameObject obj in objectsToShow)
        {
            if (obj != null)
                obj.SetActive(true);
        }

    }
}