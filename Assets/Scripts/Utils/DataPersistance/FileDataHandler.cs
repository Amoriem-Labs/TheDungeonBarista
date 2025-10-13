using System;
using System.IO;
using UnityEngine;

namespace TDB.Utils.DataPersistance
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
            string fullPath = Path.Combine(_dataDirPath, fullFileName);
            GameData loadedData = null;
            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad = "";

                    dataToLoad = ReadStringFromFile(fullPath);

                    // deserialize json
                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

                    var info = new FileInfo(fullPath);
                    loadedData.SaveTime = info.LastWriteTime;
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
            string fullPath = Path.Combine(_dataDirPath, fullFileName);
            try
            {
                // create the directory
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                // serialize data to json string
                string dataToStore = JsonUtility.ToJson(data, true);

                SaveStringToFile(fullPath, dataToStore);

                var info = new FileInfo(fullPath);
                data.SaveTime = info.LastWriteTime;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error occured when saving data to {fullPath}.\n{e}");
            }
        }

        public static void SaveStringToFile(string fullPath, string dataToStore)
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

        public static string ReadStringFromFile(string fullPath)
        {
            string dataToLoad;
            // load data from file
            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }

            return dataToLoad;
        }
    }
}
