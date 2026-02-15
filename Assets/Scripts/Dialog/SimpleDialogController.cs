using System;
using System.Collections;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDB
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(DialogueTreeController))]
    public class SimpleDialogController : DialogControllerBase
    {
        private DialogueTreeController _controller;

        private void Awake()
        {
            _controller = GetComponent<DialogueTreeController>();
        }

        /// <summary>
        /// TEST ONLY
        /// </summary>
        [Button]
        private void StartDialogue()
        {
            _controller.StartDialogue();
        }

        public override IEnumerator StartDialog(Action finishCallback)
        {
            bool finished = false;
            _controller.StartDialogue(callback: _ => finished = true);
            yield return new WaitUntil(() => finished);
            finishCallback();
        }
    }
}