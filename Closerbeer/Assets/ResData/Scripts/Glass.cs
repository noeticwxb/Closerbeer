using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Glass : MonoBehaviour 
{



    Rigidbody m_Rigidbody;
    BoxCollider m_BoxCollider;

    Vector3 m_OldPostion;
    Quaternion m_OldRotate;
    Vector3 m_OldScale;

    Vector3 m_OldCameraPos;

    Vector3 m_OffsetCamera;

    Vector3 m_End = new Vector3(0.0f, 0.0f, 2.19f);

    //public float V = 6.0f;
    public float CameraSmoothTime = 0.08f;

    bool m_inDrag = false;

    bool m_isRunning = false;

#if UNITY_EDITOR
    bool UseMouse = true;
#else
    bool UseMouse = false;
#endif
    bool UseTouch = true;

    bool m_IsDead = false;

    struct PositonAndTime
    {
        public Vector3 Pos;
        public float DeltaTime;
    }

    List<PositonAndTime> m_InitPos = new List<PositonAndTime>();

    UnityEngine.UI.Text m_ScoreText = null;

    UnityEngine.UI.Text m_ResultText = null;
    
    float maxCheck;
    float MaxDis;

    public GameObject m_Level2Collider = null;

    public GameObject m_Level3Collider = null;

    int m_Level = 1;

    public float ColliderRorateSpeed = 30.0f;
         
	// Use this for initialization
	void Start () {
        m_OldPostion = this.transform.position;
        m_OldRotate = this.transform.rotation;
        m_OldScale = this.transform.localScale;

        m_OldCameraPos = Camera.main.transform.position;

        m_OffsetCamera = m_OldCameraPos - m_OldPostion;

        m_Rigidbody = this.GetComponent<Rigidbody>();
        m_BoxCollider = this.GetComponent<BoxCollider>();

        GameObject go = GameObject.Find("ScoreText");
        if (go != null)
        {
            m_ScoreText = go.GetComponent<UnityEngine.UI.Text>();
        }

        go = GameObject.Find("ResultText");
        if (go != null)
        {
            m_ResultText = go.GetComponent<UnityEngine.UI.Text>();
            m_ResultText.enabled = false;
        }

        MaxDis = Mathf.Abs(this.transform.position.z - m_End.z);
        maxCheck = MaxDis * 2.0f / 3.0f;

        //m_Level2Collider = GameObject.Find("Level2Collider");
        //m_Level3Collider = GameObject.Find("Level3Collider");

        Debug.Log(MaxDis + " " + maxCheck);

	}
	
	// Update is called once per frame
	void Update () 
    {

        if (m_IsDead)
        {
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                || Input.GetMouseButtonUp(0))
            {
                Reset();
            }
        }

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


            if(UseMouse)
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
                        //Debug.Log("Running");
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
            /// camera trace
            if (this.transform.position.y < -10.0f)
            {
                ShowResult(10.0f);
                StartCoroutine(TimeReset());
            }
            else
            {

                float dis_z = Mathf.Abs(this.transform.position.z - m_End.z);

                if (m_ScoreText != null)
                {
                    m_ScoreText.text = string.Format("{0:0.00}",dis_z);
                }

                float speedMag = m_Rigidbody.velocity.magnitude;
                if (Mathf.Approximately(speedMag, 0.0f))
                {
                    Vector3 derisedCamera = this.transform.position + m_OffsetCamera;
                    Vector3 curVelocity = new Vector3();

                    Vector3 newCameraPos = Vector3.SmoothDamp(Camera.main.transform.position, derisedCamera, ref curVelocity, CameraSmoothTime);
                    Camera.main.transform.position = newCameraPos;

                    if (!m_ResultText.enabled)
                    {
                        ShowResult(dis_z);
                    }
                }
            }
        }

        if (m_Level == 3)
        {
            float r_speed = ColliderRorateSpeed * Time.deltaTime;
            
            m_Level3Collider.transform.Rotate(m_Level3Collider.transform.up, r_speed,Space.World);
        }
	}

    void ShowResult(float dis)
    {
        string ret_text = "Great";
        if (dis <= 0.2f)
        {
            ret_text = string.Format("Perfect! \n {0:0.00}", dis);
        }
        else if ( dis >= 0.2f && dis <= MaxDis / 10.0f)
        {
            ret_text = string.Format("Great! \n {0:0.00}", dis);
        }
        else if (dis >= MaxDis / 10.0f && dis < MaxDis / 3.0f)
        {
            ret_text = string.Format("Good! \n {0:0.00}", dis);
        }
        else if (dis >= MaxDis / 3.0f && dis <= MaxDis * 2 / 3.0f)
        {
            ret_text = "Not Bad!";
            
        }
        else
        {
            //ret_text = "Not Bad!";
            ret_text = "You Are Drunk!";
        }

        if (m_ScoreText != null)
        {
            m_ResultText.text = ret_text;
            m_ResultText.enabled = true;
        }

        m_IsDead = true;
    } 

    IEnumerator TimeReset()
    {
        yield return new WaitForSeconds(1.0f);
        this.Reset();
    }

    public void SetLevel(int level)
    {
        m_Level = level;
        Reset();
    }

    void InitMove()
    {
        if (m_InitPos.Count > 2)
        {
            this.m_BoxCollider.enabled = true;
            this.m_Rigidbody.isKinematic = false;

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
            for (int index = speedList.Count - 1; index >= 0; --index )
            {
                speed += (speedList[index] * scale);
                scale /= 2;
            }

            //speed /= speedList.Count;


            this.m_Rigidbody.velocity = speed;
        }
    }

    void SetGlassPositon(Vector3 pos, float deltaTime)
    {
        float dis_z = Mathf.Abs(pos.z - m_End.z);
        if (dis_z > maxCheck)
        {
            Vector3 newGlassPos = new Vector3(pos.x, this.transform.position.y, pos.z);
            this.transform.position = newGlassPos;
            m_InitPos.Add(new PositonAndTime() { Pos = pos, DeltaTime = deltaTime });
        }


    }

    public void Reset()
    {
        this.transform.position = m_OldPostion;
        this.transform.rotation = m_OldRotate;
        this.transform.localScale = m_OldScale;

        this.m_BoxCollider.enabled = false;
        this.m_Rigidbody.isKinematic = true;

        Camera.main.transform.position = m_OldCameraPos;

        m_OffsetCamera = m_OldCameraPos - m_OldPostion;

        m_inDrag = false;
        m_isRunning = false;
        m_InitPos.Clear();
        m_IsDead = false;


        if(m_ResultText!=null)
        {
            m_ResultText.enabled = false;
            m_ResultText.text = "";
        }

        if (m_ScoreText != null)
        {
            m_ScoreText.text = "000";
        }

        if (m_Level == 2)
        {
            m_Level2Collider.SetActive(true);
        }
        else
        {
            m_Level2Collider.SetActive(false);
        }

        if (m_Level == 3)
        {
            m_Level3Collider.SetActive(true);
        }
        else
        {
            m_Level3Collider.SetActive(false);
        }
    }
}
