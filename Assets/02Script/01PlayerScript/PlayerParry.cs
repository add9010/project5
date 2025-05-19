using UnityEngine;

public class PlayerParry
{
    private PlayerManager pm;
    private float parryRange = 2f;

    public PlayerParry(PlayerManager manager)
    {
        this.pm = manager;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryParry();
        }
    }

    private void TryParry()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(pm.transform.position, parryRange, LayerMask.GetMask("Enemy"));

        foreach (var col in enemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy == null) continue;

            if (enemy.IsParryable() && enemy.IsParryWindow)
            {
                Vector2 dir = (enemy.transform.position - pm.transform.position).normalized;
                CombatManager.ApplyDamage(enemy.gameObject, 0, 80f, pm.transform.position);

                pm.playerStateController.ForceSetParry(); // 👈 여기!
                pm.cameraController.Shake(0.1f, 0.3f);
                Debug.Log($"🎯 패링 성공! {enemy.enemyName} 넉백됨");
            }
            else
            {
                Debug.Log($"❌ 패링 실패: {enemy.enemyName} 상태 아님 또는 타이밍 아님");
            }
        }
    }
}
