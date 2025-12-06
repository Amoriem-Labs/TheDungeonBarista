using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.GameManagers;
using TDB.Utils.Singletons;
using UnityEngine;

namespace TDB.Utils.DataPersistence
{
    public class DataPersistenceManager : PassiveSingleton<DataPersistenceManager>
    {
        [Header("File Storage Config")]
        [SerializeField] private string _fileExt;
        [SerializeField] private string _profileName;
        
        // current game data
        [SerializeField, ReadOnly]
        private GameData _currentGameData;

        private FileDataHandler _dataHandler;

        public string FileExt => _fileExt;
        public GameData CurrentGameData { get => _currentGameData; set => _currentGameData = value; }

        public override void Initialize()
        {
            base.Initialize();

            _dataHandler = new FileDataHandler(Application.persistentDataPath, _fileExt);
        }

        public void StartNewGame()
        {
            // start game with a fresh data
            _currentGameData = GameManager.Instance.GameConfig.NewGameData;
        }

        public void SaveGame()
        {
            // store data to file
            _dataHandler.Save(_currentGameData, _profileName);
        }

        public bool HasGameData()
        {
            var data = _dataHandler.Load(_profileName);
            return data != null;
        }
        
        [Button(ButtonSizes.Large), DisableInEditorMode]
        public void LoadGame()
        {
            //Debug.Log("load game: preparing data");
            _currentGameData = _dataHandler.Load(_profileName);
            if (_currentGameData == null)
            {
                Debug.LogWarning("Load failed. Start a new level instead.");
                StartNewGame();
            }
            else
            {
                _currentGameData = GameDataMigration.Migrate(_currentGameData);
                Debug.Log("Load succeed.");
            }
        }
    }
}
