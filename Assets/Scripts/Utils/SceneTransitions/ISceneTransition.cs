using System.Collections;

namespace TDB.Utils.SceneTransitions
{
    public interface ISceneTransition
    {
        IEnumerator StartTransitionIntro();
        IEnumerator StartTransitionOutro();
    }
}