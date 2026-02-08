using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class Quit_Game : MonoBehaviour
    {
        public void ExitApp()
        {
            Debug.Log("Game has been quit");
            Application.Quit();
        }


    }
}
