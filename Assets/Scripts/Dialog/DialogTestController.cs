using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDB
{
    public class DialogTestController : MonoBehaviour
    {
        private DialogueTreeController _controller;

        private void Awake()
        {
            _controller = GetComponent<DialogueTreeController>();
        }

        [Button]
        private void StartDialogue()
        {
            _controller.StartDialogue();
        }
    }
}
