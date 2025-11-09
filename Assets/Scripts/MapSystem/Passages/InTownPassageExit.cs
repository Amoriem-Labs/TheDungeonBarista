using System.Collections;
using System.Linq;
using TDB.GameManagers;
using UnityEngine;

namespace TDB.MapSystem.Passages
{
    /// <summary>
    /// Only for passages among the town and cafe.
    /// </summary>
    [DisallowMultipleComponent]
    public class InTownPassageHandler : MonoBehaviour, IPassageHandler
    {
        [SerializeField] private PassageEntranceDefinition _passageExit;
        
        public IEnumerator HandleEnterPassage()
        {
            var entrances = FindObjectsOfType<PassageEntrance>().ToList();
            var exitEntrance = entrances.Find(e => e.EntranceDefinition == _passageExit);
            if (!exitEntrance)
            {
                Debug.LogError($"Passage entrance not found: {_passageExit}");
                yield break;
            }

            var player = GameObject.FindGameObjectWithTag("Player");
            if (!player)
            {
                Debug.LogError("Player not found");
                yield break;
            }

            IEnumerator TeleportToExit()
            {
                player.transform.position = exitEntrance.SpawnPoint.position;
                // wait for camera to update
                yield return new WaitForSeconds(1f);
            }

            yield return GameManager.Instance.StartTransition(TeleportToExit());
        }
    }
}