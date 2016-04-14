using UnityEngine;
using System.Collections;

public class SuccessStatus : Status
{
    GameObject m_UI;
    GameObject m_NextBtn;

    public SuccessStatus(GameObject ui)
    {
        m_UI = ui;
    }

    public override void StartStatus()
    {
        if (m_UI != null)
        {
            m_UI.SetActive(true);
        }

        int star = DataManager.Ins.GetStarByScore(DataManager.Ins.CurScore);

        if (m_NextBtn == null)
        {
            m_NextBtn = GameObject.Find("UI_Success/Next");
        }

        if(m_NextBtn!=null)
        {
            if (DataManager.Ins.CurLevel == DataManager.Ins.MaxLevel ||
                DataManager.Ins.OpenLevel <= DataManager.Ins.CurLevel)
            {
                m_NextBtn.SetActive(false);
            }
            else
            {
                m_NextBtn.SetActive(true);
            }
        }


        
        if (star >= 1)
        {
            GameObject.Find("UI_Success/Star1").GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
        else
        {
            GameObject.Find("UI_Success/Star1").GetComponent<UnityEngine.UI.Image>().color = Color.black;
        }

        if (star >= 2)
        {
            GameObject.Find("UI_Success/Star2").GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
        else
        {
            GameObject.Find("UI_Success/Star2").GetComponent<UnityEngine.UI.Image>().color = Color.black;
        }

        if (star >= 3)
        {
            GameObject.Find("UI_Success/Star3").GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
        else
        {
            GameObject.Find("UI_Success/Star3").GetComponent<UnityEngine.UI.Image>().color = Color.black;
        }

        GameObject.Find("UI_Success/ResultText").GetComponent<UnityEngine.UI.Text>().text = string.Format("{0:0.00}", DataManager.Ins.CurScore);

        if (star >= 3)
        {
            if (!StatusManager.Ins.GameLogic.AudioPerfect.isPlaying)
            {
                StatusManager.Ins.GameLogic.AudioPerfect.Play();
            }
        }
        else if( star >= 1)
        {
            if (!StatusManager.Ins.GameLogic.AudioSuccess.isPlaying)
            {
                StatusManager.Ins.GameLogic.AudioSuccess.Play();
            }
        }

    }

    public override void UpdateStatus()
    {
        
    }

    public override void LeaveStatus()
    {
        if (m_UI != null)
        {
            m_UI.SetActive(false);
        }

        if (StatusManager.Ins.GameLogic.AudioPerfect.isPlaying)
        {
            StatusManager.Ins.GameLogic.AudioPerfect.Stop();
        }

        if (StatusManager.Ins.GameLogic.AudioSuccess.isPlaying)
        {
            StatusManager.Ins.GameLogic.AudioSuccess.Stop();
        }
    }

    public override void OnCmd(string cmdName, System.Object param)
    {
        if (cmdName == "Return")
        {
            StatusManager.Ins.ChangeStatus("SelectLevel");
            StatusManager.Ins.ChangeLevel(-1);
            return;
        }

        if (cmdName == "Repeat")
        {
            StatusManager.Ins.ChangeStatus(null);
            StatusManager.Ins.ReStartLevel();
        }

        if(cmdName == "Next")
        {
            int level = DataManager.Ins.CurLevel + 1;
            if (level > DataManager.Ins.OpenLevel)
            {
                level = DataManager.Ins.OpenLevel;             
            }

            if (level > DataManager.Ins.MaxLevel)
            {
                level = DataManager.Ins.MaxLevel;
            }

            StatusManager.Ins.ChangeLevel(level);
            StatusManager.Ins.ChangeStatus(null);
        }
    }
}
