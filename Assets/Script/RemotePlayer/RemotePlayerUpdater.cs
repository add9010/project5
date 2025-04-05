using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerUpdater : MonoBehaviour
{
    [SerializeField] private GameObject remotePlayerPrefab;

    public void Apply(Dictionary<string, PlayerSnapshot> data)
    {
        var currentIds = new HashSet<string>(data.Keys);

        // 생성 또는 갱신
        foreach (var pair in data)
        {
            string id = pair.Key;
            PlayerSnapshot snapshot = pair.Value;

            var remote = RemotePlayerManager.FindById(id);
            if (remote != null)
            {
                remote.UpdateFromSnapshot(snapshot);
            }
            else
            {
                GameObject remoteObj = Instantiate(remotePlayerPrefab, snapshot.GetPosition(), Quaternion.identity);
                var manager = remoteObj.GetComponent<RemotePlayerManager>();
                manager.Initialize(id);
                manager.UpdateFromSnapshot(snapshot);
            }
        }
    }
}
