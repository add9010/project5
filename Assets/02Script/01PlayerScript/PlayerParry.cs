using UnityEngine;

public class PlayerParry
{
    private PlayerManager pm;
    private float parryRange = 2f;

    private float parryCooldown = 1.2f;
    private float lastUsedTime = -999f; 

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
        if (IsCoolingDown()) return;
        lastUsedTime = Time.time;

        bool parried = false

        Collider2D[] enemies = Physics2D.OverlapCircleAll(pm.transform.position, parryRange, LayerMask.GetMask("Enemy"));

        foreach (var col in enemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy == null) continue;

            if (enemy.IsParryable() && enemy.IsParryWindow)
            {
                Vector2 dir = (enemy.transform.position - pm.transform.position).normalized;
                CombatManager.ApplyDamage(enemy.gameObject, 0, 200f, pm.transform.position);

                pm.playerStateController.ForceSetParry();
                pm.cameraController.Shake(0.1f, 0.3f);

                pm.AddMana(1);

                parried = true;
                break; 
            }
        }

    
        if (parried && pm.parrySuccessSFX != null)
        {
            SoundManager.Instance.PlaySFX(pm.parrySuccessSFX);
        }
        else if (!parried && pm.parryFailSFX != null)
        {
            SoundManager.Instance.PlaySFX(pm.parryFailSFX);
        }
    }

    public float GetLastUsedTime() => lastUsedTime;
    public float GetCooldownDuration() => parryCooldown;
    public bool IsCoolingDown() => Time.time < lastUsedTime + parryCooldown;
}
