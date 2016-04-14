using UnityEngine;
using System.Collections;

public class NavAnimation : MonoBehaviour {

    public UnityEngine.UI.Image ImageSprit;
    public UnityEngine.RectTransform RectTransform;
    public float OffsetY;
    public float AnimationSpeed;

    bool m_IsPlaying = false;
    Vector3 m_InitPos = new Vector3();
    Color m_InitColor = new Color();


    public void StartAnimation()
    {
        RectTransform.anchoredPosition3D = m_InitPos;
        m_IsPlaying = true;
        ImageSprit.color = m_InitColor;
    }

    void Awake()
    {
        m_InitPos = RectTransform.anchoredPosition3D;
        m_InitColor = ImageSprit.color;
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (m_IsPlaying)
        {
            float moveY = AnimationSpeed * Time.deltaTime;
            float newY = RectTransform.anchoredPosition3D.y + moveY;

            if (newY > OffsetY)
            {
                RectTransform.anchoredPosition3D = new Vector3(m_InitPos.x, OffsetY, m_InitPos.z);
                ImageSprit.color = m_InitColor;
                StartAnimation();
            }
            else
            {
                float a = (OffsetY - newY ) / (OffsetY - m_InitPos.y);

                RectTransform.anchoredPosition3D = new Vector3(m_InitPos.x, newY, m_InitPos.z);
                
                ImageSprit.color = new Color(m_InitColor.r, m_InitColor.g, m_InitColor.b, a);
                Debug.Log(a);
            }

            //Debug.Log(string.Format("{0} {1} {2}",moveY,newY,OffsetY));
        }
	}
}
