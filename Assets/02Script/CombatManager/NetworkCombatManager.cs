using System.Diagnostics;

public static class NetworkCombatManager
{
    // Player 객체 참조를 저장할 정적 변수
    private static Player _currentPlayer;

    // Player 참조 설정 메서드
    public static void Initialize(Player player)
    {
        _currentPlayer = player;
    }

    // 플레이어가 몬스터에게 데미지를 입힐 때 호출
    public static void SendMonsterDamage(int damage)
    {
       

        _currentPlayer.SendMonsterDamage(damage);
    }

    // 플레이어가 트랩에게 데미지를 입힐 때 호출
    public static void SendTrapDamage(string trapId, int damage)
    {
       
        _currentPlayer.SendTrapDamage(trapId, damage);
    }
}
