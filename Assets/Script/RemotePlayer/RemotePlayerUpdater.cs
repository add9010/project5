using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerUpdater : MonoBehaviour
{
    [SerializeField] private GameObject remotePlayerPrefab;

    public void Apply(Dictionary<string, PlayerSnapshot> data)
    {
        string myId = PlayerManager.Instance?.name;

        Debug.Log($"[RemotePlayerUpdater] 내 ID: {myId}");

        foreach (var pair in data)
        {
            string id = pair.Key;
            Debug.Log($"[RemotePlayerUpdater] 수신된 ID: {id}");

            if (id == myId)
            {
                Debug.Log($"[RemotePlayerUpdater] 자기 자신이라서 무시함: {id}");
                continue;
            }

            PlayerSnapshot snapshot = pair.Value;

            var remote = RemotePlayerManager.FindById(id);
            if (remote != null)
            {
                Debug.Log($"[RemotePlayerUpdater] 기존 RemotePlayer 갱신: {id}");
                remote.UpdateFromSnapshot(snapshot);
            }
            else
            {
                Debug.Log($"[RemotePlayerUpdater] 새로운 RemotePlayer 생성: {id}");
                GameObject remoteObj = Instantiate(remotePlayerPrefab, snapshot.GetPosition(), Quaternion.identity);
                var manager = remoteObj.GetComponent<RemotePlayerManager>();
                manager.Initialize(id);
                manager.UpdateFromSnapshot(snapshot);
            }
        }
    }
}
