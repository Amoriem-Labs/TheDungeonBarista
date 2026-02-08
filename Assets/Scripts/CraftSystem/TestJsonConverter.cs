using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TDB.GameManagers;
using TDB.Utils.DataPersistence;
using UnityEngine;

namespace TDB.CraftSystem
{
    /// <summary>
    /// Ensure serializations for save data work.
    /// </summary>
    public class TestJsonConverter : MonoBehaviour
    {
        [SerializeField, ReadOnly, HideLabel, BoxGroup("Loaded Game Data")]
        private GameData _gameData;
        
        [Button(ButtonSizes.Large)]
        public void Test()
        {
            var originalData = GameManager.Instance.GameConfig.NewGameData;

            var path = Path.Combine(Application.persistentDataPath, "TestSave.sav");
            Debug.Log(path);
            
            var bytes = SerializationUtility.SerializeValue(originalData, DataFormat.JSON);
            
            File.WriteAllBytes(path, bytes);
            var loadedBytes = File.ReadAllBytes(path);
            
            _gameData = SerializationUtility.DeserializeValue<GameData>(loadedBytes, DataFormat.JSON);
        }
    }
}