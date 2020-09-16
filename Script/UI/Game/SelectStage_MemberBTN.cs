using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectStage_MemberBTN : MonoBehaviour
{
    Image m_img;
    GameObject m_cornor;
    Text m_nameText;
    Text m_jobText;
    Text m_titleText;
    public SelectStage_MemberBTN Init()
    {
        m_cornor = transform.Find("Cornor").gameObject;
        m_img = transform.Find("Image").GetComponent<Image>();
        m_nameText = transform.Find("Name").GetComponent<Text>();
        m_jobText = transform.Find("Job").GetComponent<Text>();
        m_titleText = transform.Find("Title").GetComponent<Text>();
        gameObject.SetActive(false);
        return this;
    }
   public void Enabled(Nettention.Proud.HostID player)
    {
        m_cornor.SetActive(false);
        m_img.sprite = Resources.Load<Sprite>(PlayerMng.Instance.PlayerList[player].Character.StatSystem.BaseStat.Icon);
        m_nameText.text = PlayerMng.Instance.PlayerList[player].Name;
        m_jobText.text = "Lv." + PlayerMng.Instance.PlayerList[player].Level + " " + ParseLib.GetClassKorConvert(PlayerMng.Instance.PlayerList[player].Class);
        m_titleText.text = PlayerMng.Instance.PlayerList[player].Title;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    public void Ready(bool isReady)
    {
        m_cornor.SetActive(isReady);
    }
}
