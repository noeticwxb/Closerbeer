using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelStatus: Status
{
    int m_Level;
    GameObject m_UI;
    UnityEngine.UI.Text m_ScoreText = null;

    GameObject m_Cup;
    GameObject m_Collider;

    Rigidbody m_Rigidbody;
    BoxCollider m_BoxCollider;

    Vector3 m_OldPostion;
    Quaternion m_OldRotate;
    Vector3 m_OldScale;

    Vector3 m_OldCameraPos;
    Quaternion m_OldCameraRotate;
    Vector3 m_OldCameraScale;
    Vector3 m_OffsetCamera;

    Vector3 m_SucessCameraPos;
    Quaternion m_SucessCameraRotate;
    Vector3 m_SucessCameraScale;

    Vector3 m_End = new Vector3(0.0f, 0.0f, 2.19f);

    //public float V = 6.0f;
    public float CameraSmoothTime = 0.08f;

    bool m_inDrag = false;

    bool m_isRunning = false;

    bool m_isEnd = false;

#if UNITY_EDITOR
    bool UseMouse = true;
#else
    bool UseMouse = false;
#endif
    bool UseTouch = true;

    struct PositonAndTime
    {
        public Vector3 Pos;
        public float DeltaTime;
    }

    List<PositonAndTime> m_InitPos = new List<PositonAndTime>();

    float maxCheck;

    float MaxDis;

    GameObject m_Nav;

    public float ColliderRorateSpeed = 30.0f;

    public LevelStatus(int level, GameObject ui, GameObject cup, GameObject collider, GameObject sucessCamera, GameObject Nav)
    {
        m_Level = level;
        m_UI = ui;

        m_Cup = cup;
        m_Collider = collider;

        m_Nav = Nav;

        m_OldPostion = m_Cup.transform.position;
        m_OldRotate = m_Cup.transform.rotation;
        m_OldScale = m_Cup.transform.localScale;

        m_OldCameraPos = Camera.main.transform.position;
        m_OldCameraRotate = Camera.main.transform.rotation;
        m_OldCameraScale = Camera.main.transform.localScale;

        m_SucessCameraPos = sucessCamera.transform.position;
        m_SucessCameraRotate = sucessCamera.transform.rotation;
        m_OldCameraScale = sucessCamera.transform.localScale;

        m_OffsetCamera = m_OldCameraPos - m_OldPostion;

        m_Rigidbody = m_Cup.GetComponent<Rigidbody>();
        m_BoxCollider = m_Cup.GetComponent<BoxCollider>();

        MaxDis = Mathf.Abs(m_Cup.transform.position.z - m_End.z);
        maxCheck = MaxDis * 2.0f / 3.0f;

        //Debug.Log(MaxDis + " " + maxCheck);
    }

    public override void StartStatus()
    {
        if (m_UI != null && !m_UI.activeInHierarchy)
        {
            m_UI.SetActive(true);
        }

        if (m_Cup!=null && !m_Cup.activeInHierarchy)
        {
            m_Cup.SetActive(true);
        }

        if (m_Collider != null && !m_Collider.activeInHierarchy)
        {
            m_Collider.SetActive(true);
        }

        if (m_Nav!=null )
        {
            if (DataManager.Ins.IsFirst)
            {
                m_Nav.SetActive(true);
                m_Nav.GetComponent<NavAnimation>().StartAnimation();
            }
            else
            {
                m_Nav.SetActive(false);
            }

            //m_Nav.SetActive(true);
            //m_Nav.GetComponent<NavAnimation>().StartAnimation();

        }

        DataManager.Ins.CurLevel = m_Level;

        GameObject go = GameObject.Find("ScoreText").gameObject;
        if (go != null)
        {
            m_ScoreText = go.GetComponent<UnityEngine.UI.Text>();
        }
        else
        {
            Debug.LogWarning("Not Find Score");
        }

        GameObject levelName = GameObject.Find("UI_Level/LevelName");
        levelName.GetComponent<UnityEngine.UI.Text>().text = "Level " + m_Level;

        m_Cup.transform.position = m_OldPostion;
        m_Cup.transform.rotation = m_OldRotate;
        m_Cup.transform.localScale = m_OldScale;

        this.m_BoxCollider.enabled = false;
        this.m_Rigidbody.isKinematic = true;

        Camera.main.transform.position = m_OldCameraPos;
        Camera.main.transform.rotation = m_OldCameraRotate;
        Camera.main.transform.localScale = m_OldCameraScale;

        m_OffsetCamera = m_OldCameraPos - m_OldPostion;

        m_inDrag = false;
        m_isRunning = false;
        m_isEnd = false;
        m_InitPos.Clear();


        if (m_ScoreText != null)
        {
            m_ScoreText.text = "000";
        }

        if (StatusManager.Ins.GameLogic.AudioCupBake.isPlaying)
        {
            StatusManager.Ins.GameLogic.AudioCupBake.Stop();
        }

        if (StatusManager.Ins.GameLogic.AudioSlip.isPlaying)
        {
            StatusManager.Ins.GameLogic.AudioSlip.Stop();
        }
    }

    void InitMove()
    {
        if (m_InitPos.Count > 2)
        {
            this.m_BoxCollider.enabled = true;
            this.m_Rigidbody.isKinematic = false;

            DataManager.Ins.IsFirst = false;
            if (m_Nav != null)
            {
                m_Nav.SetActive(false);
            }

            List<Vector3> speedList = new List<Vector3>();
            for (int iCount = 1; iCount <= m_InitPos.Count - 1; ++iCount)
            {
                Vector3 posEnd = m_InitPos[iCount].Pos;
                Vector3 posBeforeEnd = m_InitPos[iCount - 1].Pos;
                float t = m_InitPos[iCount].DeltaTime;
                Vector3 s = (posEnd - posBeforeEnd) / t;
                speedList.Add(s);
            }

            Vector3 speed = new Vector3();
            float scale = 0.5f;
            for (int index = speedList.Count - 1; index >= 0; --index)
            {
                speed += (speedList[index] * scale);
                scale /= 2;
            }

            this.m_Rigidbody.velocity = speed;
        }

        if (!StatusManager.Ins.GameLogic.AudioSlip.isPlaying )
        {
            StatusManager.Ins.GameLogic.AudioSlip.time = 0.9f;
            StatusManager.Ins.GameLogic.AudioSlip.Play();
        }
    }

    void SetGlassPositon(Vector3 pos, float deltaTime)
    {
        float dis_z = Mathf.Abs(pos.z - m_End.z);
        if (dis_z > maxCheck)
        {
            Vector3 newGlassPos = new Vector3(pos.x, m_Cup.transform.position.y, pos.z);
            m_Cup.transform.position = newGlassPos;
            m_InitPos.Add(new PositonAndTime() { Pos = pos, DeltaTime = deltaTime });
        }
    }

    public override void UpdateStatus()
    {
        if (m_isEnd)
            return;


        if (!m_isRunning)
        {
            var layermask = 1 << 9;

            if (UseTouch && Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (!m_inDrag && touch.phase == TouchPhase.Began)
                {
                    RaycastHit hitInfo;
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out hitInfo, layermask))
                    {
                        m_inDrag = true;
                        m_InitPos.Clear();
                        SetGlassPositon(hitInfo.point, Time.deltaTime);
                    }
                }

                if (m_inDrag && touch.phase == TouchPhase.Ended)
                {
                    m_inDrag = false;

                    RaycastHit hitInfo;
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out hitInfo, layermask))
                    {
                        SetGlassPositon(hitInfo.point, Time.deltaTime);
                        InitMove();
                        m_isRunning = true;
                    }
                }

                if (m_inDrag && touch.phase == TouchPhase.Moved)
                {
                    RaycastHit hitInfo;
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out hitInfo, layermask))
                    {
                        SetGlassPositon(hitInfo.point, Time.deltaTime);
                    }
                }
            }


            if (UseMouse)
            {
                if (!m_inDrag && Input.GetMouseButtonDown(0))
                {
                    RaycastHit hitInfo;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hitInfo, layermask))
                    {
                        m_inDrag = true;
                        SetGlassPositon(hitInfo.point, Time.deltaTime);
                    }
                }

                if (m_inDrag && Input.GetMouseButtonUp(0))
                {
                    m_inDrag = false;

                    RaycastHit hitInfo;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hitInfo, layermask))
                    {
                        SetGlassPositon(hitInfo.point, Time.deltaTime);
                        InitMove();
                        m_isRunning = true;
                    }
                }

                if (m_inDrag && Input.GetMouseButton(0))
                {
                    RaycastHit hitInfo;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hitInfo, layermask))
                    {
                        SetGlassPositon(hitInfo.point, Time.deltaTime);
                    }
                }
            }
        }

        if (m_isRunning)
        {
            if( m_Cup.transform.position.y < 0.0)
            {
                if (StatusManager.Ins.GameLogic.AudioSlip.isPlaying)
                {
                    StatusManager.Ins.GameLogic.AudioSlip.Stop();
                }
            }

            if (m_Cup.transform.position.y < -2.0f)
            {
                if ( !StatusManager.Ins.GameLogic.AudioCupBake.isPlaying)
                {
                    StatusManager.Ins.GameLogic.AudioCupBake.time = 0.5f;
                    StatusManager.Ins.GameLogic.AudioCupBake.Play();
                }

                StatusManager.Ins.ChangeStatus("Dead");
                m_isEnd = true;
            }
            else
            {

                float dis_z = Mathf.Abs(m_Cup.transform.position.z - m_End.z);

                if (m_ScoreText != null)
                {
                    m_ScoreText.text = string.Format("{0:0.00}", dis_z);
                }

                float speedMag = m_Rigidbody.velocity.magnitude;
                if (Mathf.Approximately(speedMag, 0.0f))
                {
                    if (StatusManager.Ins.GameLogic.AudioSlip.isPlaying)
                    {
                        StatusManager.Ins.GameLogic.AudioSlip.Stop();
                    }

                    Vector3 derisedCamera = m_Cup.transform.position + m_OffsetCamera;
                    Vector3 curVelocity = new Vector3();

                    Vector3 newCameraPos = Vector3.SmoothDamp(Camera.main.transform.position, derisedCamera, ref curVelocity, CameraSmoothTime);

                    if (newCameraPos != Camera.main.transform.position)
                    {
                        Camera.main.transform.position = newCameraPos;
                    }
                    else
                    {
                        if (dis_z < DataManager.Star1Dis)
                        {
                            DataManager.Ins.CurScore = dis_z;
                            DataManager.Ins.AddRecord(m_Level, dis_z);

                            if (dis_z < DataManager.Star3Dis)
                            {
                                int NextLevel = m_Level + 1;
                                if (NextLevel > DataManager.Ins.MaxLevel)
                                {
                                    NextLevel = DataManager.Ins.MaxLevel;
                                }

                                DataManager.Ins.OpenLevel = System.Math.Max(DataManager.Ins.OpenLevel, NextLevel);
                            }

                            DataManager.Ins.Save();
                            StatusManager.Ins.ChangeStatus("Success");
                        }
                        else
                        {
                            DataManager.Ins.CurScore = dis_z;
                            StatusManager.Ins.ChangeStatus("Lose");
                        }

                        m_isEnd = true;
                    }
                }
            }
        }
    }

    public override void LeaveStatus()
    {
        if (m_UI!=null)
        {
            m_UI.SetActive(false);
        }

        if (m_Cup != null)
        {
            m_Cup.SetActive(false);
        }

        if (m_Collider != null)
        {
            m_Collider.SetActive(false);
        }

        if (StatusManager.Ins.GameLogic.AudioSlip.isPlaying)
        {
            StatusManager.Ins.GameLogic.AudioSlip.Stop();
        }

    }

    public override void OnCmd(string cmdName, System.Object param)
    {
        if (cmdName == "Back")
        {
            StatusManager.Ins.ChangeStatus("SelectLevel");
            StatusManager.Ins.ChangeLevel(-1);
        }
    }
}
