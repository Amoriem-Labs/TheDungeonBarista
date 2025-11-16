using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    //=================================================================================
    // File: PlayerAnimation.cs
    // Author: Zach Lima
    // Date: 11/09/2025
    // Description: Player Animations!!!
    //=================================================================================

    public class PlayerAnimation : MonoBehaviour
    {
        private const string _horizontalAnimDirection = "Horizontal";
        private const string _verticalAnimDirection = "Vertical";

        private const string _isAttackingAnim = "IsAttacking";


        private const string _lastHorizontalAnimDirection = "LastHorizontal";
        private const string _lastVerticalAnimDirection = "LastVertical";

        private Animator _animator;
        private EntityData _entityData;
        // Start is called before the first frame update

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _entityData = GetComponent<EntityData>();
            _entityData.updateDelegate += AnimationUpdate;
        }
        void Start()
        {
        
        }

        // Update is called once per frame
        void AnimationUpdate()
        {
            UpdateAnimationVariables();
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
    }
}
