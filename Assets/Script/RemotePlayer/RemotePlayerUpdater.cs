using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerUpdater : MonoBehaviour
{
    [SerializeField] private GameObject remotePlayerPrefab;

    public void Apply(Dictionary<string, PlayerSnapshot> data)
    {
        Debug.Log($"[RemotePlayerUpdater] 수신된 스냅샷 수: {data.Count}");

        var currentIds = new HashSet<string>(data.Keys);

        // 생성 또는 갱신
        foreach (var pair in data)
        {
            string id = pair.Key;
            PlayerSnapshot snapshot = pair.Value;

            Debug.Log($"[RemotePlayerUpdater] 플레이어 ID 처리 중: {id}");

            var remote = RemotePlayerManager.FindById(id);
            if (remote != null)
            {
                Debug.Log($"[RemotePlayerUpdater] 기존 리모트 플레이어(ID: {id})를 찾음. 업데이트 진행.");
                remote.UpdateFromSnapshot(snapshot);
            }
            else
            {
                if (id == "UninitPlayer")
                {
                    Debug.LogWarning($"[RemotePlayerUpdater] ⚠️ ID가 'UninitPlayer'인 항목은 무시하고 생성하지 않습니다.");
                    continue;
                }
                Debug.Log($"[RemotePlayerUpdater] 리모트 플레이어(ID: {id})가 존재하지 않음. 새로 생성합니다.");

                GameObject remoteObj = Instantiate(remotePlayerPrefab, snapshot.GetPosition(), Quaternion.identity);
                Debug.Log($"[RemotePlayerUpdater] ✅ 리모트 플레이어 프리팹 생성 완료. 위치: {snapshot.GetPosition()} | ID: {id}");

                var manager = remoteObj.GetComponent<RemotePlayerManager>();
                manager.Initialize(id);
                manager.UpdateFromSnapshot(snapshot);
            }
        }
    }

}
