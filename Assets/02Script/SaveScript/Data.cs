using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    // ?? 플레이어 위치 저장
    public float playerX;
    public float playerY;
    public float playerZ;

    public bool[] isQuestComplete = new bool[10]; // ex) 각 퀘스트(스테이지) 완료 여부
    public List<string> clearedStoryKeys = new List<string>();

    public Data()
    {
        playerX = 0f;
        playerY = 0f;
        playerZ = 0f;
        isQuestComplete = new bool[10]; // 모든 챕터 기본 잠금 상태
        isQuestComplete[0] = true; // 0번 챕터만 기본 해금
    }
}
