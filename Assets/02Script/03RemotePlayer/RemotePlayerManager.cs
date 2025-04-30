using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerManager : MonoBehaviour
{
    public string Id;
    public RemoteMove move;
    public RemoteAnimation anim;

    private float previousX;

    void Start()
    {
        anim = GetComponent<RemoteAnimation>();

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
        }

        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private static Dictionary<string, RemotePlayerManager> RemotePlayers = new Dictionary<string, RemotePlayerManager>();

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
            RemotePlayers.Add(id, this);
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
        move?.UpdatePosition(newPosition);

        float deltaX = newPosition.x - previousX;
        if (Mathf.Abs(deltaX) > 0.01f)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.flipX = deltaX < 0;
        }

        previousX = newPosition.x;

        anim?.PlayAnimation(snapshot.animType);
    }
}
