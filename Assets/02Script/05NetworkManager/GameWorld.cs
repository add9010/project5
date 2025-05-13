using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class GameWorld
{
    private Player localPlayer; // 조작하는 사람 본인
    private Dictionary<string, RemotePlayer> remotePlayers; // 본인을 제외한 타 플레이어

    //public GameObject trapPrefab;
    private Dictionary<string, Trap> traps = new(); // 트랩 정보
    public Dictionary<string, Trap> GetTraps() => traps;

    private BossState? previousBossState = null;
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
            byte bossActed = packet.Header.bossActed;

            var updatedPlayers = new Dictionary<string, bool>();
            string myPlayerName = localPlayer.GetName();


            ///// 플레이어 정보 업데이트/////
            for (int i = 0; i < playerCount; ++i)
            {
                string playerName = packet.ReadString();
                float posX = packet.ReadFloat();
                float posY = packet.ReadFloat();
                AnimType animType = (AnimType)packet.ReadByte();
                Debug.Log($"[Player Sync] {playerName} pos: ({posX}, {posY}), animType: {animType}");
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



            ///// 트랩 정보 업데이트 /////
            int trapCount = packet.ReadByte();
            var updatedTrapIds = new HashSet<string>();

            for (int i = 0; i < trapCount; ++i)
            {
                string trapId = packet.ReadString();
                float trapX = packet.ReadFloat();
                float trapY = packet.ReadFloat();
               // Debug.Log($"[Trap Sync] trapId: {trapId}, pos: ({trapX}, {trapY})");
                if (traps.TryGetValue(trapId, out var trap))
                {
                    trap.UpdatePosition(trapX, trapY);
                }
                else
                {
                    var newTrap = new Trap(trapId, trapX, trapY);
                    traps[trapId] = newTrap;

                    Debug.Log($"[Trap Created] trapId: {trapId}");
                    MainThreadDispatcher.RunOnMainThread(() =>
                    {
                        newTrap.SpawnVisual(new Vector2(trapX, trapY));
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
                    trapToRemove.DestroyVisual(); // 트랩 제거 시 시각화 객체도 제거
                });

                traps.Remove(id);
            }



            //////// 보스 행동 업데이트 ///////
            // 보스 행동 여부 처리
            if (bossActed==1)
            {
                BossState bossState = (BossState)packet.ReadByte();
                // 보스 상태가 이전과 동일한지 체크
                if (previousBossState == null || previousBossState != bossState)
                {
                    // 상태가 다르면 업데이트 처리
                    BossManager.Instance?.ApplyBossState(bossState);
                    int bossHp = packet.ReadInt();
                    BossManager.Instance?.UpdateBossHp(bossHp);

                    // 보스 상태 업데이트
                    previousBossState = bossState;
                }
                else
                {
                    // 이전과 동일한 상태일 경우 아무 처리도 하지 않음
                    Debug.Log("[BossState] 보스 상태 변경 없음.");
                }
                // Debug.Log($"[BossState] 보스 HP: {bossHp}");
            }
        }
    }
    private void Cleanup()
    {
        remotePlayers.Clear();
    }

}
