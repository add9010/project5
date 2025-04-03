using UnityEngine;

[System.Serializable]
public class PlayerSnapshot
{
    public float x;
    public float y;

    public Vector3 GetPosition()
    {
        return new Vector3(x, y, 0); // z는 0으로 고정
    }
}
