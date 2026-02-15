using System;
using System.Collections;
using TDB.Player.Interaction;
using UnityEngine;

namespace TDB
{
    public abstract class DialogControllerBase : MonoBehaviour, IInteractable
    {
        public abstract IEnumerator StartDialog(Action finishCallback);

        public bool IsInteractable => true;
        public Action OnInteractableUpdated { get; set; }
        public void SetReady()
        {
            // TODO:
        }

        public void SetNotReady()
        {
            // TODO:
        }
    }
}