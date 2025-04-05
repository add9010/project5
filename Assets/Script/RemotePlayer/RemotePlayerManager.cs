using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RemotePlayerManager : MonoBehaviour
{
    public string Id;
    public RemoteMove move;
    public RemoteAnimation anim;
    public Sensor_HeroKnight groundSensor;
    private float previousX;

    void Start()
    {
        anim = GetComponent<RemoteAnimation>();
        groundSensor = GetComponentInChildren<Sensor_HeroKnight>();
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;  // 중력 안 받음
            rb.simulated = true;
        }

        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true; // 충돌 반응 제거
        }
    }

    private static Dictionary<string, RemotePlayerManager> RemotePlayers = new Dictionary<string, RemotePlayerManager>();

    public static List<string> GetAllIds()
    {
        return new List<string>(RemotePlayers.Keys);
    }

    public static RemotePlayerManager FindById(string id)
    {
        RemotePlayers.TryGetValue(id, out var player);
        return player;
    }

    public void Initialize(string id)
    {
        Id = id;
        move = GetComponent<RemoteMove>();
        anim = GetComponent<RemoteAnimation>();

        if (!RemotePlayers.ContainsKey(id))
        {
            RemotePlayers.Add(id, this);
        }
    }

    public static void RemoveById(string id)
    {
        if (RemotePlayers.TryGetValue(id, out var player))
        {
            Destroy(player.gameObject);
            RemotePlayers.Remove(id);
            Debug.Log($"[RemotePlayerManager] Removed {id}");
        }
    }
    public void UpdateFromSnapshot(PlayerSnapshot snapshot)
    {
        Vector3 newPosition = snapshot.GetPosition();

        // 위치 갱신
        move?.UpdatePosition(newPosition);

        // 방향 계산
        float deltaX = newPosition.x - previousX;
        if (Mathf.Abs(deltaX) > 0.01f) // 최소 이동값 설정 (노이즈 제거용)
        {
            // 좌우 반전 처리
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.flipX = deltaX < 0; // 왼쪽으로 움직이면 flipX = true
        }

        previousX = newPosition.x;

        // 착지 센서
        bool grounded = groundSensor != null && groundSensor.State();
        anim?.SetGrounded(snapshot.isGrounded);

        // 애니메이션 상태 적용
        anim?.PlayAnimation(snapshot.animType);
    }
}
