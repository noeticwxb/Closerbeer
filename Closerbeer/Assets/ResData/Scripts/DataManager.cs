using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelRecord
{
    const int MaxCount = 10;

    List<float> m_ScoreList = new List<float>();

    public List<float> ScoreList 
    { 
        get { return m_ScoreList; } 

    }

    public void AddRecord(float score)
    {
        m_ScoreList.Add(score);
        
        if (m_ScoreList.Count > MaxCount)
        {
            m_ScoreList.RemoveRange(MaxCount, m_ScoreList.Count - MaxCount);
        }
    }

    public float GetBestRecore()
    {
        if (m_ScoreList.Count != 0)
        {
            return m_ScoreList[0];
        }
        else
        {
            return System.Single.MaxValue;
        }
    }

    public void Sort()
    {
        m_ScoreList.Sort();
    }
}

public class DataManager
{
    public const float Star3Dis = 0.5f;
    public const float Star2Dis = 1.59f;
    public const float Star1Dis = 2.69f;

    //public const float Star3Dis = 5.0f;
    //public const float Star2Dis = 6.0f;
    //public const float Star1Dis = 8;

    Dictionary<int, LevelRecord> m_ScoreRecord = new Dictionary<int, LevelRecord>();
    int m_OpenLevel = 1;

    public int MaxLevel = 5;
    

    public static DataManager Ins { get; set; }

    public int OpenLevel
    {
        get { return m_OpenLevel; }
        set { m_OpenLevel = System.Math.Min(MaxLevel, value); }
    }

    public int CurLevel { get; set; }

    public float CurScore { get; set; }

    public bool IsFirst { get; set; }

    public DataManager()
    {
        m_ScoreRecord.Add(1, new LevelRecord());
        m_ScoreRecord.Add(2, new LevelRecord());
        m_ScoreRecord.Add(3, new LevelRecord());
        m_ScoreRecord.Add(4, new LevelRecord());
        m_ScoreRecord.Add(5, new LevelRecord());

        OpenLevel = 1;

        IsFirst = false;
    }

    LevelRecord GetLevelRecore(int level)
    {
        if (m_ScoreRecord.ContainsKey(level))
        {
            return m_ScoreRecord[level];
        }
        else
        {
            return null;
        }
    }

    public void AddRecord(int level, float score)
    {
        LevelRecord record = GetLevelRecore(level);
        if (record != null)
        {
            record.AddRecord(score);
            record.Sort();
        }
    }

    public float GetBestScore(int level)
    {
        LevelRecord recore = GetLevelRecore(level);
        if (recore != null)
        {
            return Mathf.Abs( recore.GetBestRecore() );
        }
        else
        {
            return 0.0f;
        }       
    }

    public int GetStar(int level)
    {
        LevelRecord recore = GetLevelRecore(level);
        if (recore != null)
        {
            return GetStarByScore(Mathf.Abs(recore.GetBestRecore()));
        }
        else
        {
            return -1;
        }

    }

    public int GetStarByScore(float score)
    {
        if (score >= -0.2f && score <= Star3Dis)
        {
            return 3;
        }

        if (score > Star3Dis && score <= Star2Dis)
        {
            return 2;
        }

        if (score > Star2Dis && score <= Star1Dis)
        {
            return 1;
        }

        return 0;
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/" + "Data.txt";
        Debug.Log("Data Path: " + path);
        if (!System.IO.File.Exists(path))
        {
            IsFirst = true;
            return;
        }
        else
        {
            IsFirst = false;
        }

        string[] lines = System.IO.File.ReadAllLines(path, System.Text.UTF8Encoding.Default);
        if (lines.Length >= 1)
        {
            OpenLevel = System.Convert.ToInt32(lines[0]);
        }

        for (int index = 1; index < lines.Length; ++index)
        {
            string line = lines[index];
            string[] record = line.Split('\t');
            if (record.Length == 0)
            {
                continue;
            }

            int levelIndex = System.Convert.ToInt32(record[0]);

            LevelRecord levelRecord = GetLevelRecore(levelIndex);
            if(levelRecord==null)
            {
                continue;
            }
            levelRecord.ScoreList.Clear();

            for (int iScore = 1; iScore < record.Length; ++iScore)
            {
                if (!string.IsNullOrEmpty(record[iScore]))
                {
                    levelRecord.AddRecord(System.Convert.ToSingle(record[iScore]));
                }
            }

            levelRecord.Sort();
        }
    }

    public void Save()
    {
        IsFirst = false;

        string path = Application.persistentDataPath + "/" + "Data.txt";
        List<string> lines = new List<string>();
        lines.Add( System.Convert.ToString( OpenLevel ) );

        foreach (var item in m_ScoreRecord)
        {
            string record = string.Empty;
            record += System.Convert.ToString(item.Key);
            record += "\t";

            foreach (var r in item.Value.ScoreList)
            {
                record += System.Convert.ToString(r);
                record += "\t";
            }
            lines.Add(record);
        }

        System.IO.File.WriteAllLines(path, lines.ToArray(), System.Text.UTF8Encoding.Default);

    }

}
