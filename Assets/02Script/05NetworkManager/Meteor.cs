using UnityEngine;

public class Meteor : MonoBehaviour
{
    private void Start()
    {
        // 1초 후 자동 제거
        Destroy(gameObject, 1.9f);
    }
}
