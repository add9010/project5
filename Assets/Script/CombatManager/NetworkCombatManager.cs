using UnityEngine;

public static class NetworkCombatManager
{
    /// <summary>
    /// 서버에 보스 데미지 요청을 전송합니다.
    /// </summary>
    public static void SendDamage(string targetId, float damage)
    {
        var net = NetworkClient.Instance;
        if (net == null || string.IsNullOrEmpty(targetId))
        {
            Debug.LogWarning("[NetworkCombatManager] 네트워크 연결 또는 ID 없음");
            return;
        }

       // net.SendAttack(targetId, damage);
    }
}
