using Sirenix.Serialization;
using TDB.CraftSystem.Data;
using TDB.Utils.Misc;
using UnityEngine;

// Works for ResourceScriptableObject and any subclass T
public sealed class RsoPathFormatter<T> : MinimalBaseFormatter<T> where T : ResourceScriptableObject
{
    private static readonly Serializer<string> Str = Serializer.Get<string>();

    protected override void Write(ref T value, IDataWriter writer)
    {
        Str.WriteValue(value ? value.ResourcesPath : null, writer);
    }

    protected override void Read(ref T value, IDataReader reader)
    {
        var path = Str.ReadValue(reader);
        if (path == null || string.IsNullOrEmpty(path))
        {
            value = null;
            return;
        }

        value = Resources.Load(path, typeof(T)) as T;   // load full asset; no overrides applied
    }
}
