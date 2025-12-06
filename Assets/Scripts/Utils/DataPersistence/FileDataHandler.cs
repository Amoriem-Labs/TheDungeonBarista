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
                    var loadedBytes = File.ReadAllBytes(fullPath);
            
                    loadedData = SerializationUtility.DeserializeValue<GameData>(loadedBytes, DataFormat.JSON);
                    
                    // ReadDataFromFile(fullPath, out byte[] dataToLoad);
                    //
                    // // deserialize json
                    // loadedData = SerializationUtility.DeserializeValue<GameData>(dataToLoad, DataFormat.JSON);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error occured when loading data from {fullPath}.\n{e}");
                }
            }
            else
            {
                Debug.LogError($"File {fullFileName} does not exist.");
            }
            return loadedData;
        }

        public void Save(GameData data, string fullFileName)
        {
            if (!fullFileName.EndsWith(_dataFileExt)) fullFileName += _dataFileExt;
            
            string fullPath = Path.Combine(_dataDirPath, fullFileName);
            try
            {
                // // create the directory
                // Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                //
                // // serialize data to json string
                // var dataToStore = SerializationUtility.SerializeValue(data, DataFormat.JSON);
                //
                // SaveDataToFile(fullPath, dataToStore);
                
                var bytes = SerializationUtility.SerializeValue(data, DataFormat.JSON);
            
                File.WriteAllBytes(fullPath, bytes);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error occured when saving data to {fullPath}.\n{e}");
            }
        }

        private void SaveDataToFile(string fullPath, byte[] dataToStore)
        {
            File.WriteAllBytes(fullPath, dataToStore);
        }
        
        public static void ReadDataFromFile(string fullPath, out byte[] dataToLoad)
        {
            dataToLoad = File.ReadAllBytes(fullPath);
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
