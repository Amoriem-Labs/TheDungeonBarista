using Sirenix.Serialization;
using TDB.MapSystem.Passages;
using TDB.Utils.Misc;
using UnityEngine;

[assembly: RegisterFormatter(typeof(RsoPathFormatter<PassageEntranceDefinition>))]

namespace TDB.MapSystem.Passages
{
    [CreateAssetMenu(menuName = "Data/Map/Passage Entrance", fileName = "New Passage Entrance", order = 0)]
    public class PassageEntranceDefinition : ResourceScriptableObject { }
}