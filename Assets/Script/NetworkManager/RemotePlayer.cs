using System;
using System.Collections.Generic;
using System.Threading;

public class RemotePlayer
{
    private string name;
    private float posX, posY;
    private AnimType animType;

    public RemotePlayer(string name)
    {
        this.name = name;
        posX = 0;
        posY = 0;
        animType = AnimType.Idle; // 기본값 설정
    }

    public void UpdatePosition(float x, float y)
    {
        posX = x;
        posY = y;
    }

    public void SetName(string name) => this.name = name;
    public float GetPosX() => posX;
    public float GetPosY() => posY;
    public string GetName() => name;

    public void SetAnimType(AnimType type) => animType = type;
    public AnimType GetAnimType() => animType;


    public byte GetAnimTypeAsByte() => (byte)animType;
    public void SetAnimTypeFromByte(byte value) => animType = (AnimType)value;
}
