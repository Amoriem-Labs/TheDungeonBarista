using System;
using System.IO;
using Sirenix.Serialization;
using UnityEngine;

namespace TDB.Utils.DataPersistence
{
    public class FileDataHandler
    {
        private string _dataDirPath = "";

        private string _dataFileExt = "";

        public FileDataHandler(string dataDirPath, string dataFileExt)
        {
            if (string.IsNullOrEmpty(dataDirPath))
            {
                dataDirPath = Application.consoleLogPath;
            }
            _dataDirPath = dataDirPath;
            _dataFileExt = dataFileExt;
        }
        
        public GameData Load(string fullFileName)
        {
            if (!fullFileName.EndsWith(_dataFileExt)) fullFileName += _dataFileExt;
            
            string fullPath = Path.Combine(_dataDirPath, fullFileName);
            GameData loadedData = null;
            if (File.Exists(fullPath))
            {
                try
                {
                    ReadDataFromFile(fullPath, out byte[] dataToLoad);

                    // deserialize json
                    // loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                    loadedData = SerializationUtility.DeserializeValue<GameData>(dataToLoad, DataFormat.JSON);

                    // var info = new FileInfo(fullPath);
                    // loadedData.SaveTime = info.LastWriteTime;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error occured when loading data from {fullPath}.\n{e}");
                }
            }
            return loadedData;
        }

        public void Save(GameData data, string fullFileName)
        {
            if (!fullFileName.EndsWith(_dataFileExt)) fullFileName += _dataFileExt;
            
            string fullPath = Path.Combine(_dataDirPath, fullFileName);
            try
            {
                // create the directory
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                // serialize data to json string
                var dataToStore = SerializationUtility.SerializeValue(data, DataFormat.JSON);
                // string dataToStore = JsonUtility.ToJson(data, true);

                SaveDataToFile(fullPath, dataToStore);

                // var info = new FileInfo(fullPath);
                // data.SaveTime = info.LastWriteTime;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error occured when saving data to {fullPath}.\n{e}");
            }
        }

        private void SaveDataToFile(string fullPath, byte[] dataToStore)
        {
            // write data to file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }

        public static void SaveDataToFile(string fullPath, string dataToStore)
        {
            // write data to file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        
        public static void ReadDataFromFile(string fullPath, out byte[] dataToLoad)
        {
            dataToLoad = File.ReadAllBytes(fullPath);
            // // load data from file
            // using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            // {
            //     using (StreamReader reader = new StreamReader(stream))
            //     {
            //         dataToLoad = reader.ReadToEnd();
            //     }
            // }
        }

        public static void ReadDataFromFile(string fullPath, out string dataToLoad)
        {
            // load data from file
            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }
        }
    }
}
