using TDB.CraftSystem.EffectSystem.Data;

namespace TDB.CraftSystem.EffectSystem.LevelUpEffect
{
    public class LevelUpEffectData : TypedEffectData<LevelUpEffectData>
    {
        private float _progress;

        public int Level => (int)_progress;
        public float Progress => _progress - Level;
        public int ProgressInPercent => (int)(Progress * 100);
        public float TotalProgress => _progress;
        
        public LevelUpEffectData(float progress)
        {
            _progress = progress;
        }

        public override string GetTooltipText() => (Definition as LevelUpEffectDefinition).GetTooltipText(Level);
    }
}