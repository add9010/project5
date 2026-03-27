using System.Diagnostics;

public static class NetworkCombatManager
{
    private static Player _currentPlayer;

    public static void Initialize(Player player)
    {
        _currentPlayer = player;
    }

    public static void SendMonsterDamage(int damage)
    {
       

        _currentPlayer.SendMonsterDamage(damage);
    }

    public static void SendTrapDamage(string trapId, int damage)
    {
       
        _currentPlayer.SendTrapDamage(trapId, damage);
    }
}
