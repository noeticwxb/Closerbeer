using UnityEngine;
using System.Collections;

public class Logic : MonoBehaviour {

    public AudioSource AudioLoop;
    public AudioSource AudioButton;
    public AudioSource AudioCupBake;
    public AudioSource AudioPerfect;
    public AudioSource AudioSuccess;
    public AudioSource AudioFailed;
    public AudioSource AudioSlip;

    public GameObject LogInUI;
    public GameObject SelectUI;
    public GameObject LevelUI;
    public GameObject NavUI;
    public GameObject DeadUI;
    public GameObject SuccessUI;
    public GameObject LoseUI;

    public GameObject Cup;

    public GameObject Level2Collider;

    public GameObject Level3Collider;

    public GameObject Level4Collider;

    public GameObject Level5Collider;

    public GameObject SuccessCamera;

    DataManager m_DataMgr;
    StatusManager m_StatusMgr;

    public bool FaceBookEnabled = false;



	// Use this for initialization
	void Start () {
        m_DataMgr = new DataManager();
        DataManager.Ins = m_DataMgr;
        m_DataMgr.Load();

        m_StatusMgr = new StatusManager();
        StatusManager.Ins = m_StatusMgr;

        m_StatusMgr.LogIn = new LogInStatus(LogInUI);
        m_StatusMgr.SelectLevel = new SelectLevelStatus(SelectUI);
        m_StatusMgr.Dead = new DeadStatus(DeadUI);
        m_StatusMgr.Success = new SuccessStatus(SuccessUI);
        m_StatusMgr.Lose = new LoseStatus(LoseUI);

        m_StatusMgr.Level1 = new LevelStatus(1, LevelUI, Cup, null, SuccessCamera, NavUI);
        m_StatusMgr.Level2 = new LevelStatus(2, LevelUI, Cup, Level2Collider, SuccessCamera, NavUI);
        m_StatusMgr.Level3 = new LevelStatus(3, LevelUI, Cup, Level3Collider, SuccessCamera, NavUI);
        m_StatusMgr.Level4 = new LevelStatus(4, LevelUI, Cup, Level4Collider, SuccessCamera, NavUI);
        m_StatusMgr.Level5 = new LevelStatus(5, LevelUI, Cup, Level5Collider, SuccessCamera, NavUI);

        m_StatusMgr.GameLogic = this;

        m_StatusMgr.ChangeStatus("SelectLevel");
	}

    public void SendCmd(string cmdName, System.Object param = null)
    {
        if (AudioButton != null)
        {
            AudioButton.Play();
        }
        m_StatusMgr.OnCmd(cmdName, param);
    }

    public void ClickCmd(string cmdName)
    {
        SendCmd(cmdName);    
    }

    public void SelectLevel(int level)
    {
        SendCmd("StartLevel", level);
    }
	
	// Update is called once per frame
	void Update () {
        m_StatusMgr.UpdateStatus();
	}
}
