using UnityEngine;

public class RemoteMove : MonoBehaviour
{
    private Vector3 targetPos;

    public void UpdatePosition(Vector3 newPos)
    {
        targetPos = newPos;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 10f);
    }
}