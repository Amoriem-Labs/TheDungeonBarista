namespace TDB.Damage
{
    public interface IDamageable
    {
        public void TakeDamage(DamageData damage);
    }

    public class DamageData
    {
        public int Amount;
        public int DamageSourceLayer { get; set; }
    }
}