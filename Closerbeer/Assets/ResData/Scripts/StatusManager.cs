using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class StatusManager
{
    public static StatusManager Ins { get; set; }

    public LogInStatus LogIn { get; set; }

    public SelectLevelStatus SelectLevel { get; set; }

    public DeadStatus Dead { get; set; }

    public SuccessStatus Success { get; set; }

    public LoseStatus Lose { get; set; }

    public LevelStatus Level1 { get; set; }

    public LevelStatus Level2 { get; set; }

    public LevelStatus Level3 { get; set; }

    public LevelStatus Level4 { get; set; }

    public LevelStatus Level5 { get; set; }

    public Logic GameLogic { get; set; }

    Status m_Current;

    Status m_CurrrentLevel;

    Status GetStatus(string statusName)
    {
        switch (statusName)
        {
            case "LogIn": return LogIn;
            case "SelectLevel": return SelectLevel;
            case "Dead": return Dead;
            case "Success": return Success;
            case "Lose": return Lose;
            default: return null;
        }
    }
   

    Status GetLevelStatus(int level)
    {
        switch (level)
        {
            case 1: return Level1;
            case 2: return Level2;
            case 3: return Level3;
            case 4: return Level4;
            case 5: return Level5;
            default: return null;
        }
    }

    public void ChangeStatus(string statusName)
    {
        if (string.IsNullOrEmpty(statusName))
        {
            if (m_Current != null)
            {
                m_Current.LeaveStatus();
                m_Current.IsActive = false;
                m_Current = null;
            }
            Debug.Log("Enter Empty");
            return;
        }

        Status newStatus = GetStatus(statusName);
        if (newStatus != null )
        {
            if(newStatus == m_Current)
            {
                Debug.Log("Repeat Status: " + statusName);
                return;
            }

            if (m_Current != null)
            {
                m_Current.LeaveStatus();
                m_Current.IsActive = false;
            }

            newStatus.StartStatus();
            m_Current = newStatus;
            m_Current.IsActive = true;
            Debug.Log("Enter: " + statusName);
        }
        else
        {
            Debug.LogError("Can Not Get Status: " + statusName);
        }
    }

    public void ChangeLevel(int level)
    {
        if (level <= 0 )
        {
            if (m_CurrrentLevel != null)
            {
                m_CurrrentLevel.LeaveStatus();
                m_CurrrentLevel.IsActive = false;
                m_CurrrentLevel = null;
            }
            Debug.Log("Enter Empty Level");
            return;
        }

        Status newLevel = GetLevelStatus(level);
        if (newLevel != null )
        {
            if (newLevel == m_CurrrentLevel)
            {
                Debug.Log("Repeat Level: " + level);
                return;
            }

            if (m_CurrrentLevel != null)
            {
                m_CurrrentLevel.LeaveStatus();
                m_CurrrentLevel.IsActive = false;
            }

            newLevel.StartStatus();
            m_CurrrentLevel = newLevel;
            m_CurrrentLevel.IsActive = true;
            Debug.Log("Enter Level: " + level);
        }
        else
        {
            Debug.LogError("Can Not Get Status: " + level);
        }
    }

    public void ReStartLevel()
    {
        if (m_CurrrentLevel != null)
        {
            m_CurrrentLevel.StartStatus();
        }
    }

    public void UpdateStatus()
    {
        if (m_Current != null)
        {
            m_Current.UpdateStatus();
        }
        if (m_CurrrentLevel != null)
        {
            m_CurrrentLevel.UpdateStatus();
        }
    }

    public void OnCmd(string cmdName, System.Object param)
    {
        if (m_Current != null)
        {
            m_Current.OnCmd(cmdName, param);
        }

        if (m_CurrrentLevel != null)
        {
            m_CurrrentLevel.OnCmd(cmdName, param);
        }
    }
}
