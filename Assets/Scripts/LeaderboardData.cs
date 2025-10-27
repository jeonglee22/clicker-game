using System;
using UnityEngine;

[Serializable]
public class LeaderboardData : MonoBehaviour
{
    public string nickname;
    public int score;
    public long timestamp;

    public LeaderboardData()
    {

    }

    public LeaderboardData(string nickname, int score, long timestamp)
    {
        this.nickname = nickname;
        this.score = score;
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

    public static LeaderboardData FromJson(string data)
    {
        return JsonUtility.FromJson<LeaderboardData>(data);
    }
}
