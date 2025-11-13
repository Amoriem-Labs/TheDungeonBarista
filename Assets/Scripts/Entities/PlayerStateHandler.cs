using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================================================
// File: PlayerStateHandler.cs
// Author: Zach Lima
// Date: 10/21/2025
// Description: Handles all player states as well as changing states
//=================================================================================
namespace TDB
{
    public class PlayerStateHandler : MonoBehaviour
    {
        private EntityData _entityData;
        public enum States
        {
            free,
            stunned
        }

        public States currentState;


        public void ChangeState(States newState)
        {
            //DISABLE CURRENT STATE 
            if (newState != currentState)
            {

                DisableCurrentState();
                EnableNewState(newState);
               
            }
            //disable last state 
        }

        void DisableCurrentState()
        {
            switch (currentState)
            {
                case States.free:
                    {
                        _entityData.updateDelegate -= GetComponent<PlayerMovement>().MovementUpdate;
                        break;
                    }
                case States.stunned:
                    {
                        _entityData.updateDelegate -= GetComponent<PlayerStunned>().StunnedUpdate;
                        break;
                    }

            }
        }

        public void EnableNewState(States newState)
        {
            switch (newState)
            {
                case States.free:
                    {
                        _entityData.updateDelegate += GetComponent<PlayerMovement>().MovementUpdate;
                       
                        break;
                    }
                case States.stunned:
                    {
                        _entityData.updateDelegate += GetComponent<PlayerStunned>().StunnedUpdate;
                        break;
                    }

            }
            currentState = newState;
        }


        
        private void Awake()
        {
            _entityData = GetComponent<EntityData>();
            EnableNewState(currentState);
        }

   
    }
}
