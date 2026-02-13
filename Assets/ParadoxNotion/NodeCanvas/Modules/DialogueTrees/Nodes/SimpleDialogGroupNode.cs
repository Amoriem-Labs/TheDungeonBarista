using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace TDB.Dialog
{
    [Name("Dialog Group")]
    [Description("Make the selected Dialogue Actor talk. You can make the text more dynamic by using variable names and actor names in square brackets\ne.g. [myVarName] or [Global/myVarName]")]
    public class SimpleDialogGroupNode : DTNode
    {
        public override bool requireActorSelection => false;

        private IBlackboard _blackboard;
        private SimpleDialogGroup _dialog;
        private int _dialogIndex;

        protected override Status OnExecute(Component agent, IBlackboard bb)
        {
            _blackboard = bb;
            var dialog = bb.GetVariable<SimpleDialogGroup>("dialog asset");
            if (!dialog.value)
            {
                Debug.LogError("No Dialog Asset Selected");
                return Status.Error;
            }

            _dialog = dialog.value;
            _dialogIndex = 0;
            if (!ForwardDialog())
            {
                Debug.Log("Empty Dialog Asset Selected");
                DLGTree.Continue();
                return Status.Success;
            }
            return Status.Running;
        }

        private bool ForwardDialog()
        {
            if (_dialogIndex >= _dialog.Statements.Count) return false;
            
            var currentStatement = _dialog.Statements[_dialogIndex];
            _dialogIndex++;
            
            Debug.Log(currentStatement.Statement.text);
            var statement = currentStatement.Statement.ProcessStatementBrackets(_blackboard, DLGTree);
            
            var actor = currentStatement.GetActor();
            DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(actor, statement, OnStatementFinish));
            return true;
        }

        void OnStatementFinish() {
            if (!ForwardDialog())
            {
                Debug.Log("Dialog Finished");
                status = Status.Success;
                DLGTree.Continue();
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
        // protected override void OnNodeGUI() {
        //     GUILayout.BeginVertical(Styles.roundedBox);
        //     GUILayout.Label("\"<i> " + statement1.text.CapLength(30) + "</i> \"");
        //     GUILayout.EndVertical();
        // }
#endif

    }
}