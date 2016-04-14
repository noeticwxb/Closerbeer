using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelButton
{
    public GameObject Base;
    public GameObject Lock;
    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;
    public GameObject ScoreText;

    public void SetLock()
    {
        Base.SetActive(true);
        Base.GetComponent<UnityEngine.UI.Button>().interactable = false;
        Lock.SetActive(true);

        Star1.SetActive(false);
        Star2.SetActive(false);
        Star3.SetActive(false);
        ScoreText.SetActive(false);
    }

    public void SetStar(int star, float score)
    {
        Base.SetActive(true);
        Lock.SetActive(false);
        Base.GetComponent<UnityEngine.UI.Button>().interactable = true;

        if (star >= 1)
        {
            ScoreText.SetActive(true);
            ScoreText.GetComponent<UnityEngine.UI.Text>().text = string.Format("{0:0.00}", score);
        }
        else
        {
            ScoreText.SetActive(false);
        }

        if (star >= 1)
        {
            Star1.SetActive(true);
        }
        else
        {
            Star1.SetActive(false);
        }

        if (star >= 2)
        {
            Star2.SetActive(true);
        }
        else
        {
            Star2.SetActive(false);
        }

        if (star >= 3)
        {
            Star3.SetActive(true);
        }
        else
        {
            Star3.SetActive(false);
        }
        
    }
}

public class SelectLevelStatus : Status
{
    GameObject m_UI;

    Dictionary<int, LevelButton> m_LevelButton = new Dictionary<int, LevelButton>();

    LevelButton CreateLevelBtn(int level)
    {
        LevelButton lbtn = new LevelButton();
        string path = "UI_Select/Level" + level;
        lbtn.Base = GameObject.Find(path);
        lbtn.Lock = GameObject.Find(path + "/Lock");
        lbtn.Star1 = GameObject.Find(path + "/Image1");
        lbtn.Star2 = GameObject.Find(path + "/Image2");
        lbtn.Star3 = GameObject.Find(path + "/Image3");
        lbtn.ScoreText = GameObject.Find(path + "/Text");
        return lbtn;
    }

    public SelectLevelStatus(GameObject ui)
    {
        m_UI = ui;
    }

    public override void StartStatus()
    {
        if (m_UI != null)
        {
            m_UI.SetActive(true);

            if (m_LevelButton.Count == 0)
            {
                m_LevelButton[1] = CreateLevelBtn(1);
                m_LevelButton[2] = CreateLevelBtn(2);
                m_LevelButton[3] = CreateLevelBtn(3);
                m_LevelButton[4] = CreateLevelBtn(4);
                m_LevelButton[5] = CreateLevelBtn(5);
            }
        }

        m_LevelButton[1].SetLock();
        m_LevelButton[2].SetLock();
        m_LevelButton[3].SetLock();
        m_LevelButton[4].SetLock();
        m_LevelButton[5].SetLock();

        for (int i = 1; i <= DataManager.Ins.OpenLevel; ++i)
        {
            m_LevelButton[i].SetStar(DataManager.Ins.GetStar(i), DataManager.Ins.GetBestScore(i));
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
    }

    public override void OnCmd(string cmdName, System.Object param)
    {
        if (cmdName == "StartLevel" && param!=null)
        {
            int level = (int)param;
            StatusManager.Ins.ChangeLevel(level);
            StatusManager.Ins.ChangeStatus(null);
        }
    }
}
