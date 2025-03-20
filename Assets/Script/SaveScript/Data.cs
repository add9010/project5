using System;
using UnityEngine;

[System.Serializable]
public class Data
{
    // ?? �÷��̾� ��ġ ����
    public float playerX;
    public float playerY;
    public float playerZ;

    // ?? �رݵ� é�� ���� (����)
    public bool[] isUnlock = new bool[10];

    public Data()
    {
        playerX = 0f;
        playerY = 0f;
        playerZ = 0f;
        isUnlock = new bool[10]; // ��� é�� �⺻ ��� ����
        isUnlock[0] = true; // 0�� é�͸� �⺻ �ر�
    }
}