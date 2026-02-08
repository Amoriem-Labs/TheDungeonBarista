using System.Collections;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using TDB.MapSystem.Passages;
using UnityEngine;

namespace TDB
{

    //=================================================================================
    // File: Boon_pickup_item.cs
    // Author: Nathan B
    // Date: 2/1/2026
    // Description: Script for all boons to be picked up. This includes coroutines for
    // time delay manipulation. It is all self-contained. ItemValue values correspond to: 
    // 1: Speed Boon
    // 2: Health Boon
    // 3: Knockback Boon
    // -1: Speed Curse
    // -2: Health Curse
    // -3: A Trap that traps the player and stops movement
    //=================================================================================

    public class Boon_pickup_item : MonoBehaviour
    {
        public int ItemValue;
        public EntityData PlayerDungeon;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (ItemValue == 1)
                {
                   StartCoroutine(TemporarySpeedBoost());
                }

                else if (ItemValue == 2)
                {
                   StartCoroutine(TemporaryHealthBoost());
                }

                else if (ItemValue == 3)
                {
                   StartCoroutine(TemporaryKnockbackBoost());
                }

                else if (ItemValue == -1)
                {
                   StartCoroutine(TemporarySpeedDecrease());
                }

                else if (ItemValue == -2)
                {
                   StartCoroutine(TemporaryHealthDecrease());
                }

                else if (ItemValue == -3)
                {
                   StartCoroutine(TRAP());
                }

                gameObject.GetComponent<Renderer>().enabled = false;
                gameObject.GetComponent<Collider2D>().enabled = false;
            }

        }

        private IEnumerator TemporarySpeedBoost()
        {
            PlayerDungeon.MaxSpeed += 3;
            PlayerDungeon.Acceleration += 15;

            // Pauses the code for 5 seconds before decreasing the values
            yield return new WaitForSeconds(5);

            PlayerDungeon.MaxSpeed -= 3;
            PlayerDungeon.Acceleration -= 15;

            Destroy(gameObject);
        }

        private IEnumerator TemporaryHealthBoost()
        {
            PlayerDungeon.MaxHealth += 5;

            // Pauses the code for 5 seconds before decreasing the values
            yield return new WaitForSeconds(5);

            PlayerDungeon.MaxHealth -= 5;

            Destroy(gameObject);
        }

        private IEnumerator TemporaryKnockbackBoost()
        {
            PlayerDungeon.Knockback += 5;

            // Pauses the code for 5 seconds before decreasing the values
            yield return new WaitForSeconds(5);

            PlayerDungeon.Knockback -= 5;

            Destroy(gameObject);
        }

        private IEnumerator TemporarySpeedDecrease()
        {
            PlayerDungeon.MaxSpeed -= 3;
            PlayerDungeon.Acceleration -= 15;

            // Pauses the code for 5 seconds before decreasing the values
            yield return new WaitForSeconds(5);

            PlayerDungeon.MaxSpeed += 3;
            PlayerDungeon.Acceleration += 15;

            Destroy(gameObject);
        }

        private IEnumerator TemporaryHealthDecrease()
        {
            PlayerDungeon.MaxHealth -= 5;

            // Pauses the code for 5 seconds before decreasing the values
            yield return new WaitForSeconds(5);

            PlayerDungeon.MaxHealth += 5;

            Destroy(gameObject);
        }

        private IEnumerator TRAP()
        {

            // THERE IS A POTENTIAL FOR A BUG HERE WHERE THE PLAYER COULD HAVE A 
            // TEMPORARY MAX SPEED THAT IS DIFFERENT FROM WHAT IS NORMAL SO WHEN 
            // THE CODE TRIES TO RESET THE SPEED IT RESETS TO THE INCORRECT VALUE
            float normalSpeed = PlayerDungeon.MaxSpeed;
            PlayerDungeon.MaxSpeed = 0;

            // Pauses the code for 5 seconds before decreasing the values
            yield return new WaitForSeconds(5);

            PlayerDungeon.MaxSpeed = normalSpeed;

            Destroy(gameObject);
        }
    }
}
