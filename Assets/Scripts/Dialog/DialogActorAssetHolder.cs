using NodeCanvas.DialogueTrees;
using UnityEngine;

namespace TDB
{
    public class DialogActorAssetHolder : MonoBehaviour, IDialogueActor
    {
        [SerializeField] private ActorDefinition _actor;
        
        private void OnEnable()
        {
            ActorStatement.RegisteredActors.Add(_actor, this);
        }

        private void OnDisable()
        {
            ActorStatement.RegisteredActors.Remove(_actor);
        }

        public new string name => _actor.name;
        public Texture2D portrait => _actor.portrait;
        public Sprite portraitSprite => _actor.portraitSprite;
        public Color dialogueColor => _actor.dialogueColor;
        public Vector3 dialoguePosition => transform.position;
    }
}