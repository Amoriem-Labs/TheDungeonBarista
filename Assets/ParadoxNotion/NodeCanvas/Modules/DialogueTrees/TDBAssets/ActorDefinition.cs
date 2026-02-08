using System;
using NodeCanvas.DialogueTrees;
using UnityEngine;

namespace TDB
{
    [CreateAssetMenu(menuName = "Data/Dialog/Actor", fileName = "New Actor", order = 0)]
    public class ActorDefinition : ScriptableObject, IDialogueActor
    {
        [field: SerializeField] public new string name { get; private set; }

        public Texture2D portrait => null;
        [field: SerializeField]
        public Sprite portraitSprite { get; private set; }
        [field: SerializeField]
        public Color dialogueColor { get; private set; } = Color.white;

        // TODO: find actor in the scene
        public Vector3 dialoguePosition => Vector3.zero;
        public Transform transform => null;

        private void OnValidate()
        {
            if (name == null)
            {
                name = base.name;
            }
        }
    }
}