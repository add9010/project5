using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerUpdater : MonoBehaviour
{
    [SerializeField] private GameObject remotePlayerPrefab;
 
    public void Apply(Dictionary<string, PlayerSnapshot> data)
    {
        var currentIds = new HashSet<string>(data.Keys);

        Debug.Log($"[Apply] 리모트 {data.Count}명 적용 시도");
        foreach (var pair in data)
        {
            Debug.Log($"[Apply] ID: {pair.Key}, 위치: {pair.Value.x}, {pair.Value.y}");
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

        var existingIds = new List<string>(RemotePlayerManager.GetAllIds());
        foreach (var id in existingIds)
        {
            if (!currentIds.Contains(id))
            {
                RemotePlayerManager.RemoveById(id);
            }
        }



    }
}
