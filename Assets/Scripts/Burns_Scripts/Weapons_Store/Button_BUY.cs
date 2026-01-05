using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TDB;

//The goal of this code is to keep track of the different amount that each item costs and send it to the MONEY.cs code.
namespace TDB
{
    public class BuyButton : MonoBehaviour
    {
        public int itemCost = 100; //Can be changed depending on the item.
        public amountOfMoney moneyManager;

        public void Impact()
        {
            if (moneyManager != null)
            {
                moneyManager.SpendMoney(itemCost);
            }
        }
    }
}

