using System;
using UnityEngine;

[Serializable]
public class ScoreData
{
    public int score;
    public long timestamp;

    public ScoreData()
    {
        
    }

    public ScoreData(int highScore, long timestamp)
    {
        this.score = highScore;
        this.timestamp = timestamp;
    }

    public DateTime GetDateTime()
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
    }

    public string GetDateString()
    {
        return GetDateTime().ToString("yyyy-MM-dd HH::mm:ss");
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static ScoreData FromJson(string data)
    {
        return JsonUtility.FromJson<ScoreData>(data);
    }
}
