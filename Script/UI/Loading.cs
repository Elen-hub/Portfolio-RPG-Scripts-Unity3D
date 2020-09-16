using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : BaseUI
{
    int m_count;
    float m_elapsedTime;
    float m_targetTime = 0.5f;
    float m_disconnectTime;
    Text m_loadingText;
    public override void Init()
    {
        base.Init();
        m_loadingText = transform.Find("LoadingText").GetComponent<Text>();
    }
    public override void Open()
    {
        base.Open();
        m_count = 0;
        m_elapsedTime = 0;
        m_disconnectTime = 0;
        m_loadingText.text = "Loading";
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        m_disconnectTime += Time.deltaTime;
        m_elapsedTime += Time.deltaTime;
        if (m_targetTime < m_elapsedTime)
        {
            m_elapsedTime = 0;
            switch (m_count)
            {
                case 0:
                    m_loadingText.text = "Loading";
                    m_count = 1;
                    break;
                case 1:
                    m_loadingText.text = "Loading.";
                    m_count = 2;
                    break;
                case 2:
                    m_loadingText.text = "Loading..";
                    m_count = 3;
                    break;
                case 3:
                    m_loadingText.text = "Loading...";
                    m_count = 0;
                    break;
            }
        }
        if(m_disconnectTime > 10)
        {
            NetworkMng.Instance.DisconnectServer();
            Close();
        }
    }
}
