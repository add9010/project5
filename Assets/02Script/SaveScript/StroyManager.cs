using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }

    private HashSet<string> storyFlags = new HashSet<string>(); // 진행도 키 저장

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void SetProgress(string key)
    {
        storyFlags.Add(key);
    }

    public bool HasProgress(string key)
    {
        return storyFlags.Contains(key);
    }
}
