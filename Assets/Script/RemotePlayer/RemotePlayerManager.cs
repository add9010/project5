using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RemotePlayerManager : MonoBehaviour
{
    public string Id;
    public RemoteMove move;
    public RemoteAnimation anim;



    void Start()
    {
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
        move?.UpdatePosition(snapshot.GetPosition());
        anim?.PlayAnimation(snapshot.animType);
    }
}
