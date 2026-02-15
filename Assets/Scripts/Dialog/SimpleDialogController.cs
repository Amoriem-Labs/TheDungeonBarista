using System;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDB
{
    [RequireComponent(typeof(DialogueTreeController))]
    public class SimpleDialogController : MonoBehaviour
    {
        private DialogueTreeController _controller;

        private void Awake()
        {
            _controller = GetComponent<DialogueTreeController>();
        }

        [Button]
        public void StartDialogue()
        {
            _controller.StartDialogue();
        }
    }
}