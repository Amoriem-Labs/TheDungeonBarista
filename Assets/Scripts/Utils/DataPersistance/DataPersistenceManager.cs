using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.Utils.Singletons;
using UnityEngine;

namespace TDB.Utils.DataPersistance
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

        private List<IGameDataPersistence> _dataSources;

        public string FileExt => _fileExt;
        public GameData CurrentGameData { get => _currentGameData; set => _currentGameData = value; }

        public override void Initialize()
        {
            base.Initialize();

            _dataHandler = new FileDataHandler(Application.persistentDataPath, _fileExt);
            _dataSources = new List<IGameDataPersistence>();
        }

        public void AddDataSource(IGameDataPersistence source)
        {
            _dataSources.Add(source);
        }

        public void RemoveDataSource(IGameDataPersistence source)
        {
            _dataSources.Remove(source);
        }

        public void StartNewGame()
        {
            // start game with a fresh data
            _currentGameData = new GameData();

            // push data to IGameDataPersistence objects
            foreach (var source in _dataSources)
            {
                source.LoadDataFrom(_currentGameData);
            }
        }

        public void SaveGame()
        {
            // load data from IGameDataPersistence objects
            foreach (var source in _dataSources)
            {
                source.SaveDataTo(_currentGameData);
            }

            // store data to file
            _dataHandler.Save(_currentGameData, _profileName);
        }

        public bool HasGameData()
        {
            var data = _dataHandler.Load(_profileName);
            return data != null;
        }
        
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
                Debug.LogWarning("Load succeed.");
                // push data to IGameDataPersistence objects
                foreach (var source in _dataSources)
                {
                    source.LoadDataFrom(_currentGameData);
                }
            }
        }
    }
}
