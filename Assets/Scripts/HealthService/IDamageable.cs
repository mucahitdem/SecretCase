namespace HealthService
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void ApplyDamage(float amount, ulong instigatorClientId);
    }
}
