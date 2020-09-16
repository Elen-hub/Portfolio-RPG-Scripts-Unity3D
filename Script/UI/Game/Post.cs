using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Post : BaseUI
{
    Transform m_grid;
    GameObject m_receiveBTN;

    List<PostBTN> m_postList = new List<PostBTN>();
    protected override void InitUI()
    {
        m_grid = GetComponentInChildren<ContentSizeFitter>().transform;
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Close);

        m_receiveBTN = transform.Find("Receive").gameObject;
        m_receiveBTN.GetComponent<Button>().onClick.AddListener(OnClickReceive);
    }
    public override void Open()
    {
        for (int i = 0; i < m_postList.Count; ++i)
            m_postList[i].Disabled();

        List<PostInfo> InfoList = NetworkMng.Instance.PostList;
        Debug.Log(InfoList.Count);
        for (int i = 0; i < InfoList.Count; ++i)
        {
            PostBTN btn;
            if (m_postList.Count <= i)
            {
                btn = Instantiate(Resources.Load<PostBTN>("UI/Instance/PostBTN"), m_grid).Init(i);
                m_postList.Add(btn);
            }
            else
                btn = m_postList[i];

            btn.Enabled(InfoList[i]);
        }
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        gameObject.SetActive(false);
        // m_postInformation.Close();
    }
    void OnClickReceive()
    {

    }
}
