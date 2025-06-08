using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class GameWorld
{
    private Player localPlayer; // 조작하는 사람 본인
    private Dictionary<string, RemotePlayer> remotePlayers; // 본인을 제외한 타 플레이어
    private Dictionary<string, Trap> traps = new(); // 트랩 정보
    private Dictionary<GameObject, Trap> trapVisualMap = new(); // Visual → Trap
    private BossState? previousBossState = null;
    private readonly object worldMutex;

    public GameWorld()
    {
        remotePlayers = new Dictionary<string, RemotePlayer>();
        worldMutex = new object();
    }

    public void SetLocalPlayer(Player player) => localPlayer = player;
    public Player GetLocalPlayer() => localPlayer;
    public Dictionary<string, Trap> GetTraps() => traps;
    public Dictionary<GameObject, Trap> GetTrapVisualMap() => trapVisualMap;

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
        return snapshots;
    }

    public void SyncWorldData(Packet packet)
    {
        lock (worldMutex)
        {
            int playerCount = packet.Header.playerCount;
            byte bossActed = packet.Header.bossActed;

            var updatedPlayers = new Dictionary<string, bool>();
            string myPlayerName = localPlayer.GetName();

            // 플레이어 정보 업데이트
            for (int i = 0; i < playerCount; ++i)
            {
                string playerName = packet.ReadString();
                float posX = packet.ReadFloat();
                float posY = packet.ReadFloat();
                AnimType animType = (AnimType)packet.ReadByte();

                if (playerName == myPlayerName) continue;

                if (remotePlayers.TryGetValue(playerName, out var remotePlayer))
                {
                    remotePlayer.UpdatePosition(posX, posY);
                    remotePlayer.SetAnimType(animType);
                }
                else
                {
                    var newPlayer = new RemotePlayer(playerName);
                    newPlayer.UpdatePosition(posX, posY);
                    newPlayer.SetAnimType(animType);
                    remotePlayers[playerName] = newPlayer;
                }

                updatedPlayers[playerName] = true;
            }

            // 업데이트되지 않은 플레이어 제거
            var playersToDelete = new List<string>();
            foreach (var kvp in remotePlayers)
            {
                if (!updatedPlayers.ContainsKey(kvp.Key))
                    playersToDelete.Add(kvp.Key);
            }
            foreach (var playerName in playersToDelete)
            {
                remotePlayers.Remove(playerName);
            }

            // 트랩 정보 업데이트
            int trapCount = packet.ReadByte();
            var updatedTrapIds = new HashSet<string>();

            for (int i = 0; i < trapCount; ++i)
            {
                string trapId = packet.ReadString();
                float trapX = packet.ReadFloat();
                float trapY = packet.ReadFloat();
                int trapHp = packet.ReadInt();

                if (traps.TryGetValue(trapId, out var trap))
                {
                    trap.UpdatePosition(trapX, trapY);
                    trap.UpdateHp(trapHp);
                }
                else
                {
                    var newTrap = new Trap(trapId, trapX, trapY, trapHp);
                    traps[trapId] = newTrap;

                    MainThreadDispatcher.RunOnMainThread(() =>
                    {
                        newTrap.SpawnVisual(new Vector2(trapX, trapY));
                        if (newTrap.Visual != null)
                        {
                            trapVisualMap[newTrap.Visual] = newTrap;

                            if (newTrap.Visual != null)
                            {
                                trapVisualMap[newTrap.Visual] = newTrap;
                            }
                        }
                    });
                }
                updatedTrapIds.Add(trapId);
            }

            // 서버에 없는 트랩 제거
            var trapIdsToRemove = new List<string>();
            foreach (var kvp in traps)
            {
                if (!updatedTrapIds.Contains(kvp.Key))
                    trapIdsToRemove.Add(kvp.Key);
            }

            foreach (var id in trapIdsToRemove)
            {
                var trapToRemove = traps[id];
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    trapVisualMap.Remove(trapToRemove.Visual);
                    trapToRemove.DestroyVisual();
                });

                traps.Remove(id);
            }

            // 보스 행동 업데이트
            if (bossActed == 1)
            {
                BossState bossState = (BossState)packet.ReadByte();
                if (previousBossState == null || previousBossState != bossState)
                {
                    BossManager.Instance?.ApplyBossState(bossState);
                    int bossHp = packet.ReadInt();
                    BossManager.Instance?.UpdateBossHp(bossHp);
                    previousBossState = bossState;
                }
            }
        }
    }

    private void Cleanup()
    {
        remotePlayers.Clear();
    }
}
