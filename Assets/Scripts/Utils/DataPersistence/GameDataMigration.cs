using System;
using TDB.GameManagers;

namespace TDB.Utils.DataPersistence
{
    public static class GameDataMigration
    {
        public static GameData Migrate(GameData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            // TODO: upgrade data from old ones
            
            return data;
        }
    }
}