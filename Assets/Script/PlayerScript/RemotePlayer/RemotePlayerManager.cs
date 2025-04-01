using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RemotePlayerManager : MonoBehaviour
{
    public string Id { get; private set; }

    private RemoteMove move;
    private RemoteAnimation anim;

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

    public void UpdateFromSnapshot(PlayerSnapshot snapshot)
    {
        move?.UpdatePosition(snapshot.GetPosition());
        anim?.PlayAnimation(snapshot.animationState);
    }

    // === Static °ü¸® ===
    public static Dictionary<string, RemotePlayerManager> RemotePlayers = new();

    public static RemotePlayerManager FindById(string id)
    {
        return RemotePlayers.TryGetValue(id, out var player) ? player : null;
    }
}