namespace Practical1.GameSystem;

public sealed class PiercingDamageStrategy : IDamageStrategy
{
    public int Calculate(int damage, int armor, bool isDefending)
    {
        int result = Math.Max(0, damage);
        return isDefending ? result / 2 : result;
    }
}
