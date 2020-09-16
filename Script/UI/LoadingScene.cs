using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : BaseUI
{
    Image m_progressBar;
    Text m_loadingText;
    public string SetText { set { m_loadingText.text = value; } }
    public float Progress { get { return m_progressBar.fillAmount; } set { m_progressBar.fillAmount = value; } }
    public override void Init()
    {
        base.Init();
        m_loadingText = transform.Find("LoadingText").GetComponent<Text>();
        m_progressBar = transform.Find("ProgressBackGround").GetChild(0).GetComponent<Image>();
    }
    public override void Open()
    {
        base.Open();
        m_progressBar.fillAmount = 0;
        m_loadingText.text = "Loading";
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
    }
}
