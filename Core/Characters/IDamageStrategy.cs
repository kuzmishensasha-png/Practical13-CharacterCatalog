namespace Practical1.GameSystem;

public interface IDamageStrategy
{
    int Calculate(int damage, int armor, bool isDefending);
}
