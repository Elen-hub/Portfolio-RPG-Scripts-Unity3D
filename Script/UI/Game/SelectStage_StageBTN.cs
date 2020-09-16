using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectStage_StageBTN : MonoBehaviour
{
    int m_mapHandle;
    Image m_img;
    Image m_cornor;
    Text m_subjectText;
    Text m_informationText;
    public int Select { set { m_cornor.enabled = value == m_mapHandle; } }
    public SelectStage_StageBTN Init()
    {
        m_img = transform.Find("Image").GetComponent<Image>();
        m_cornor = transform.Find("Cornor").GetComponent<Image>();
        m_subjectText = transform.Find("Subject").GetComponent<Text>();
        m_informationText = transform.Find("Information").GetComponent<Text>();
        GetComponent<Button>().onClick.AddListener(OnClickSelect);
        return this;
    }
    public void Enabled(int mapHandle)
    {
        m_mapHandle = mapHandle;
        Map map = MapMng.Instance.MapDic[mapHandle];
        m_subjectText.text = map.MapName;
        m_informationText.text = map.Information;
        m_img.sprite = Resources.Load<Sprite>(map.MapIconPath);
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    public void OnClickSelect()
    {
        UIMng.Instance.Open<SelectStage>(UIMng.UIName.SelectStage).SelectHandle(m_mapHandle);
    }
}
