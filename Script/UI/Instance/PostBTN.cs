using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PostInfo
{
    public PostInfo(int date, string id, string subject, string content, int package, int number, int gold, int cash)
    {
        Date = date;
        ID = id;
        Subject = subject;
        Content = content;
        Package = package;
        Gold = gold;
        Cash = cash;
        Number = number;
    }

    public System.HashInt Package;
    public System.HashInt Number;
    public System.HashInt Gold;
    public System.HashInt Cash;
    public int Date;
    public string ID;
    public string Subject;
    public string Content;
}
public class PostBTN : MonoBehaviour
{
    PostInfo m_info;
    int m_number;

    Image m_packageIcon;
    Text m_subjectText;
    Text m_idText;
    Text m_dateText;
    public PostBTN Init(int number)
    {
        GetComponent<Button>().onClick.AddListener(OnClickReicevePost);
        m_number = number;
        m_packageIcon = transform.Find("Package").GetComponent<Image>();
        m_subjectText = transform.Find("Subject").GetComponent<Text>();
        m_idText = transform.Find("ID").GetComponent<Text>();
        m_dateText = transform.Find("Date").GetComponent<Text>();
        return this;
    }
    public void Enabled(PostInfo info)
    {
        m_info = info;
        if (info.Package == 0) m_packageIcon.sprite = Resources.Load<Sprite>("Sprite/PostBTN");
        else m_packageIcon.sprite = Resources.Load<Sprite>(ItemMng.Instance.GetItemList[info.Package].Icon);
        m_subjectText.text = info.Subject;
        m_idText.text = info.ID;

        int divisonMinute = info.Date / 60;
        if(divisonMinute < 1)
        {
            m_dateText.text = info.Date + "초 남음";
        }
        else
        {
            int divisonHour = divisonMinute / 60;
            if(divisonHour < 1)
            {
                m_dateText.text = divisonMinute + "분 남음";
            }
            else
            {
                int divisonDay = divisonHour / 24;
                if(divisonDay < 1)
                {
                    m_dateText.text = divisonHour + "시간 남음";
                }
                else
                {
                    m_dateText.text = divisonDay + "일 남음";
                }
            }
        }

        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    public void OnClickReicevePost()
    {
        NetworkMng.Instance.RequestPostReiceve(m_info.ID);
    }
}
