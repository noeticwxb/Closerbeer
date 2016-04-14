using UnityEngine;
using System.Collections;

public class LogInStatus : Status
{
    GameObject m_UI;

    public LogInStatus(GameObject logInUI)
    {
        m_UI = logInUI;
    }


    public override void StartStatus()
    {
        if (m_UI != null)
        {
            m_UI.SetActive(true);
        }
    }

    public override void UpdateStatus()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            || Input.GetMouseButtonUp(0))
        {
            StatusManager.Ins.ChangeStatus("SelectLevel");
        }
    }

    public override void LeaveStatus()
    {
        if (m_UI != null)
        {
            m_UI.SetActive(false);
        }
    }

    public override void OnCmd(string cmdName, System.Object param)
    {

    }
}
