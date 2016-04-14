using UnityEngine;
using System.Collections;

public class Status 
{
    public bool IsActive { get; set; }

    public Status()
    {
        IsActive = false;
    }

    public virtual void StartStatus()
    {
        
    }

    public virtual void UpdateStatus()
    {
        
    }

    public virtual void LeaveStatus()
    {
        
    }

    public virtual void OnCmd(string cmdName, System.Object param)
    {
        
    }
}
