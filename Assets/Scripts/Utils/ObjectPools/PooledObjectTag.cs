namespace TDB.Utils.ObjectPools
{
    /// <summary>
    /// Each tag can only have one instance of pool.
    /// </summary>
    public enum PooledObjectTag
    {
        Untagged = 0,
        // projectiles (1-)
        SampleBullet = 1,
        // enemies (33-)
        SampleEnemy = 33,
        // particles (65-)
        SampleParticle = 65,
        // others (97-)
        SFXSource = 97,
    }
}