using System.Collections;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDB
{
    [CreateAssetMenu(fileName = "New Two Actor Dialog", menuName = "Data/Dialog/Two Actor Dialog", order = 0)]
    public class SimpleDialogGroup : ScriptableObject
    {
        [SerializeField] private List<ActorStatement> _statements = new();
        
        public List<ActorStatement> Statements => _statements;
    }

    [System.Serializable]
    public class ActorStatement
    {
        [SerializeField] private ActorDefinition _actor;
        [SerializeField, InlineProperty, HideLabel, BoxGroup("Statement")]
        public Statement Statement;

        public IDialogueActor GetActor() => _actor;
    }
}