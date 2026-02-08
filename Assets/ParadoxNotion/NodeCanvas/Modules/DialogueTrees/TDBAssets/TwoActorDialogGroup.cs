using System.Collections;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDB
{
    [CreateAssetMenu(fileName = "New Two Actor Dialog", menuName = "Data/Dialog/Two Actor Dialog", order = 0)]
    public class TwoActorDialogGroup : ScriptableObject
    {
        [SerializeField] private List<TwoActorStatement> _statements = new();
        
        public List<TwoActorStatement> Statements => _statements;
    }

    [System.Serializable]
    public class TwoActorStatement
    {
        [SerializeField] private ActorDefinition _actor;
        [SerializeField, InlineProperty, HideLabel, BoxGroup("Statement")]
        public Statement Statement;

        public IDialogueActor GetActor() => _actor;
        
        public enum Actor
        {
            Self, Other
        }
    }
}