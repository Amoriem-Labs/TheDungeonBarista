
using TDB.GameManagers;

namespace TDB.Utils.DataPersistence
{
    public interface IGameDataWriter
    {
        public void WriteToData(GameData data);
    }
    
    
    public interface IDataWriterDestination
    {
        public void RegisterDataWriter(IGameDataWriter dataWriter);

        public void UnregisterDataWriter(IGameDataWriter dataWriter);
    }

}