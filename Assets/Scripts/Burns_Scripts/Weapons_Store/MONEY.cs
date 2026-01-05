using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//Goal of this code is to replace the amount of money the user has with the amount 
//certain items cost. 

namespace TDB
{
    public class amountOfMoney : MonoBehaviour
    {
        public TextMeshProUGUI moneyTextComponent;
        public int MONEY;

        void Start()
        {
            moneyTextComponent.text = "$9999";
            string moneyString = moneyTextComponent.text.Replace("$", "");
            MONEY = int.Parse(moneyString);
        }


        public bool SpendMoney(int amount)
        {
            if (MONEY >= amount)
            {
                MONEY -= amount;
                moneyTextComponent.text = "$" + MONEY.ToString();
                return true;
            }
            if (MONEY < amount)
            {
                Debug.Log("Not enough money!");
                return false;
            }
            return false;
        }
    }
}
