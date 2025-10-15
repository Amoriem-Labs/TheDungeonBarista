using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This is code for clicking the buttons and having images associated 
//with the buttons appear. 

public class ShowBoxOnClick : MonoBehaviour
{
    public GameObject boxToShow;

    public void ShowBox()
    {
        //This should turn off all other images so only the correct button works
        //I believe it works similar to a recursive file directory
        ShowBoxOnClick[] allBoxes = FindObjectsOfType<ShowBoxOnClick>();
        foreach (ShowBoxOnClick box in allBoxes)
        {
            if (box.boxToShow != null)
                box.boxToShow.SetActive(false);
        }

        // 2. Show only this one
        boxToShow.SetActive(true);
    }
}
