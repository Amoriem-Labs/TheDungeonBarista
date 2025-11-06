using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TDB
{
    public class PlayerMovement : MonoBehaviour
    {
        //=================================================================================
        // File: PlayerMovement.cs
        // Author: Zach Lima
        // Date: 10/21/2025
        // Description: Player Movement, pretty self explanatory. 
        //=================================================================================

        private EntityData _entityData;
        private Animator _animator;


        private const string _horizontalAnimDirection = "Horizontal";
        private const string _verticalAnimDirection = "Vertical";

        private const string _isAttackingAnim = "IsAttacking";


        private const string _lastHorizontalAnimDirection = "LastHorizontal";
        private const string _lastVerticalAnimDirection = "LastVertical";


       
        
        private void Awake()
        {
            _entityData = GetComponent<EntityData>();
            _animator = GetComponent<Animator>();
            InputManager.attackKeyPressed += AttackKeyPressed;

           
           GetComponentInChildren<AttackHitbox>().dealDamage += DealDamage;
        }



        private void DealDamage(GameObject _damagedEntity)
        {
            _damagedEntity.GetComponent<EntityData>().CurrentHealth -= 1;

            _damagedEntity.GetComponent<EntityData>().Velocity = _entityData.lastDirection * _entityData.Knockback;
            

            if (_damagedEntity.GetComponent<EntityData>().CurrentHealth <= 0)
            {
                //run the die method
               Destroy(_damagedEntity);

            }
        }

        private void AttackKeyPressed()
        {
            _entityData.IsAttacking = true;
           
        }
        // Start is called before the first frame update
        void Start()
        {
            
        }

        void Update()
        {
            //first, update movement for this frame.
            _entityData.movementDirection.Set(InputManager.Movement.x, InputManager.Movement.y);

            UpdateAnimationVariables();
           

            //finally, update velocity for this frame. 

            if (_entityData.IsAttacking)
            {
                _entityData.Velocity = Vector2.zero;
            }
            else
            {
                DoBasicPlayerMovement();
            }

            _entityData.Rb.velocity = _entityData.Velocity;
            
           
        }
        

        private void UpdateAnimationVariables()
        {
            _animator.SetFloat(_horizontalAnimDirection, _entityData.movementDirection.x);
            _animator.SetFloat(_verticalAnimDirection, _entityData.movementDirection.y);

            //Update values for the idle and attack anims
            _animator.SetFloat(_lastHorizontalAnimDirection, _entityData.lastDirection.x);
            _animator.SetFloat(_lastVerticalAnimDirection, _entityData.lastDirection.y);

            _animator.SetBool(_isAttackingAnim, _entityData.IsAttacking);
        }
        //accelerates or decelerate based on input
        private void DoBasicPlayerMovement()
        {
            //if the player is pressing an input
            if (_entityData.movementDirection != Vector2.zero)
            {
                
                //updates the last direction the player was facing
                _entityData.lastDirection = new Vector2(_entityData.movementDirection.x, _entityData.movementDirection.y);

                _entityData.Velocity = new Vector2(Mathf.MoveTowards(_entityData.Velocity.x, _entityData.MaxSpeed * _entityData.movementDirection.x,_entityData.Acceleration * Time.deltaTime)
                                                    ,Mathf.MoveTowards(_entityData.Velocity.y, _entityData.MaxSpeed * _entityData.movementDirection.y, _entityData.Acceleration * Time.deltaTime));
            }
            else
            {
               
                _entityData.Velocity = new Vector2(Mathf.MoveTowards(_entityData.Velocity.x, 0, _entityData.Decceleration * Time.deltaTime)
                                                  , Mathf.MoveTowards(_entityData.Velocity.y, 0, _entityData.Decceleration * Time.deltaTime));
            }

        }

       
    }
}
