using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class ChipmunkStateHandler : MonoBehaviour
    {
        private EntityData _entityData;
        public enum States
        {
            wander, chase, attack,
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
                case States.wander:
                    {
                        _entityData.updateDelegate -= GetComponent<ChipmunkWander>().WanderUpdate;
                        break;
                    }
                case States.stunned:
                    {
                        _entityData.updateDelegate -= GetComponent<ChipmunkStunned>().StunnedUpdate;
                        break;
                    }
                case States.chase:
                    {
                        _entityData.updateDelegate -= GetComponent<ChipmunkChase>().ChaseUpdate;
                        break;
                    }
                case States.attack:
                    {
                        _entityData.updateDelegate -= GetComponent<ChipmunkAttacking>().AttackUpdate;
                        break;
                    }

            }
        }

        public void EnableNewState(States newState)
        {
            switch (newState)
            {
                case States.wander:
                    {
                        _entityData.updateDelegate += GetComponent<ChipmunkWander>().WanderUpdate;
                        break;
                    }
                case States.stunned:
                    {
                        _entityData.updateDelegate += GetComponent<ChipmunkStunned>().StunnedUpdate;
                        break;
                    }
                case States.chase:
                    {
                        _entityData.updateDelegate += GetComponent<ChipmunkChase>().ChaseUpdate;
                        break;
                    }
                case States.attack:
                    {
                        _entityData.updateDelegate += GetComponent<ChipmunkAttacking>().AttackUpdate;
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
