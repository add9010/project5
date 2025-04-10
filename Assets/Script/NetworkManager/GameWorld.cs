using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class GameWorld
{
    private Player localPlayer; // 조작하는 사람 본인
    private Dictionary<string, RemotePlayer> remotePlayers; // 본인을 제외한 타 플레이어
    private readonly object worldMutex;        // 플레이어의 월드는 서버의 월드와 동기화 됨

    public GameWorld()
    {
        remotePlayers = new Dictionary<string, RemotePlayer>();
        worldMutex = new object();
    }

    public void SetLocalPlayer(Player player) => localPlayer = player;
    public Player GetLocalPlayer() => localPlayer;

    public Dictionary<string, PlayerSnapshot> GetRemoteSnapshots()
    {
        var snapshots = new Dictionary<string, PlayerSnapshot>();
        string myName = localPlayer.GetName();

        foreach (var kvp in remotePlayers)
        {
            if (kvp.Key == myName) continue;

            var remote = kvp.Value;
            snapshots[kvp.Key] = new PlayerSnapshot
            {
                x = remote.GetPosX(),
                y = remote.GetPosY(),
                animType = remote.GetAnimType()
            };
        }
        //Debug.Log($"[Snapshots] 생성된 스냅샷 수: {snapshots.Count}");
        return snapshots;
    }
    public void SyncWorldData(Packet packet)
    {
        lock (worldMutex)
        {
            int playerCount = packet.Header.playerCount;
            var updatedPlayers = new Dictionary<string, bool>();
            string myPlayerName = localPlayer.GetName();
            for (int i = 0; i < playerCount; ++i)
            {
                string playerName = packet.ReadString();
                float posX = packet.ReadFloat();
                float posY = packet.ReadFloat();
                AnimType animType = (AnimType)packet.ReadByte();

                if (playerName == myPlayerName) continue;

                if (remotePlayers.TryGetValue(playerName, out var remotePlayer))
                {
                    // 기존 목록 최신화
                    remotePlayer.UpdatePosition(posX, posY);
                    remotePlayer.SetAnimType(animType);
                }
                else
                {
                    // 신규 플레이어 추가
                    var newPlayer = new RemotePlayer(playerName);
                    newPlayer.UpdatePosition(posX, posY);
                    newPlayer.SetAnimType(animType);
                    remotePlayers[playerName] = newPlayer;
                }

                updatedPlayers[playerName] = true;
            }

            // 기존 플레이어 중에서 업데이트되지 않은 플레이어 삭제
            var playersToDelete = new List<string>();
            foreach (var kvp in remotePlayers)
            {
                if (!updatedPlayers.ContainsKey(kvp.Key))
                {
                    playersToDelete.Add(kvp.Key);
                }
            }

            foreach (var playerName in playersToDelete)
            {
                remotePlayers.Remove(playerName);
            }
            // Debug.Log($"현재 리모트 플레이어 수: {remotePlayers.Count}");
        }
    }
    private void Cleanup()
    {
        remotePlayers.Clear();
    }


    ~GameWorld()
    {
        Cleanup();
    }
}
