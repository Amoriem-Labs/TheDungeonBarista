
namespace TDB.Utils.DataPersistance
{
    public interface IGameDataPersistence
    {
        public void LoadDataFrom(GameData data);

        public void SaveDataTo(GameData data);
    }
}