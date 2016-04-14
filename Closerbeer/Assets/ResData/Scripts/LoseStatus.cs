using UnityEngine;
using System.Collections;

public class LoseStatus: Status
{
    GameObject m_UI;

    public LoseStatus(GameObject ui)
    {
        m_UI = ui;
    }

    public override void StartStatus()
    {
        if (m_UI != null)
        {
            m_UI.SetActive(true);
        }

        if (!StatusManager.Ins.GameLogic.AudioFailed.isPlaying)
        {
            StatusManager.Ins.GameLogic.AudioFailed.Play();
        }
    }

    public override void UpdateStatus()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            || Input.GetMouseButtonUp(0))
        {
            StatusManager.Ins.ReStartLevel();
            StatusManager.Ins.ChangeStatus(null);
        }
    }

    public override void LeaveStatus()
    {
        if (m_UI != null)
        {
            m_UI.SetActive(false);
        }

        if (StatusManager.Ins.GameLogic.AudioFailed.isPlaying)
        {
            StatusManager.Ins.GameLogic.AudioFailed.Stop();
        }
    }

    public override void OnCmd(string cmdName, System.Object param)
    {

    }

}
