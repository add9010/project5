using UnityEngine;

public class PlayerParry
{
    private PlayerManager pm;
    private float parryRange = 2f;

    private float parryCooldown = 1.2f; //  쿨타임 시간
    private float lastUsedTime = -999f; //  마지막 시도 시간

    public PlayerParry(PlayerManager manager)
    {
        this.pm = manager;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            TryParry();
        }
    }

    private void TryParry()
    {
        if (IsCoolingDown()) return; // ✅ 쿨타임 체크
        lastUsedTime = Time.time;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(pm.transform.position, parryRange, LayerMask.GetMask("Enemy"));

        foreach (var col in enemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy == null) continue;

            if (enemy.IsParryable() && enemy.IsParryWindow)
            {
                Vector2 dir = (enemy.transform.position - pm.transform.position).normalized;
                CombatManager.ApplyDamage(enemy.gameObject, 0, 80f, pm.transform.position);

                pm.playerStateController.ForceSetParry();
                pm.cameraController.Shake(0.1f, 0.3f);
                Debug.Log($"패링 성공! {enemy.enemyName} 넉백됨");
            }
            else
            {
                Debug.Log($" 패링 실패: {enemy.enemyName} 상태 아님 또는 타이밍 아님");
            }
        }
    }

    // ✅ 쿨타임용 Getter
    public float GetLastUsedTime() => lastUsedTime;
    public float GetCooldownDuration() => parryCooldown;
    public bool IsCoolingDown() => Time.time < lastUsedTime + parryCooldown;
}
