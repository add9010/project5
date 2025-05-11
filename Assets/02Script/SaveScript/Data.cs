using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class Data
{
    public float playerX;
    public float playerY;
    public float playerZ;
    public string savedSceneName = "Stage1";
    public bool[] isUnlock = new bool[10];
    public int currentStage = 0;
    // 런타임용 구조
    [NonSerialized] public Dictionary<string, bool> isQuestComplete = new Dictionary<string, bool>();
    [NonSerialized] public HashSet<string> clearedStoryKeys = new HashSet<string>();

    // 직렬화용 구조
    public List<QuestEntry> questListSerialized = new List<QuestEntry>();
    public List<string> clearedStoryKeysSerialized = new List<string>();

    public Data()
    {
        savedSceneName = "Stage1";
        isUnlock[0] = true;
    }

    public void PrepareForSave()
    {
        questListSerialized = isQuestComplete
            .Select(q => new QuestEntry(q.Key, q.Value)).ToList();

        clearedStoryKeysSerialized = clearedStoryKeys.ToList();
    }

    public void RestoreAfterLoad()
    {
        isQuestComplete = questListSerialized
            .ToDictionary(q => q.questKey, q => q.isComplete);

        clearedStoryKeys = new HashSet<string>(clearedStoryKeysSerialized);
    }

    public bool IsQuestComplete(string questKey)
    {
        return isQuestComplete.ContainsKey(questKey) && isQuestComplete[questKey];
    }
}

[Serializable]
public class QuestEntry
{
    public string questKey;
    public bool isComplete;

    public QuestEntry(string key, bool complete)
    {
        questKey = key;
        isComplete = complete;
    }
}
