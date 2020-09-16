using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemMessage : BaseUI
{
    public static SystemMessage Instance;

    public enum MessageType
    {
        Main,
        Sub,
        Tutorial,
    }
    
    public struct Message
    {
        public string Str;
        public Color Color;
        public Sprite Icon;
    }

    Text m_itemMessage;
    List<MainMessage> m_mainMessageList = new List<MainMessage>();
    Transform m_mainMessageGrid;
    Text m_subMessage;
    Text m_tutorialMessage;

    Dictionary<MessageType, List<Message>> m_messageDic = new Dictionary<MessageType, List<Message>>();
    Coroutine m_tutorialTextCoroutine;

    float m_mainElapsedTime = 2;
    float m_subElspasedTime;

    int m_mainPushCount = 0;
    int m_subPushCount = 0;

    bool m_useMain;
    bool m_useSub;

    protected override void InitUI()
    {
        m_mainMessageGrid = transform.Find("MainMessageGrid");
        m_subMessage = transform.Find("SubMessage").GetComponent<Text>();
        m_tutorialMessage = transform.Find("TutorialMessage").GetComponent<Text>();

        m_messageDic.Add(MessageType.Main, new List<Message>());
        m_messageDic.Add(MessageType.Sub, new List<Message>());
        m_messageDic.Add(MessageType.Tutorial, new List<Message>());

        Instance = this;
    }

    public void PushMessage(MessageType type , string str)
    {
        Message msg = new Message()
        {
            Str = str,
        };

        if (type == MessageType.Main)
            msg.Color = Color.white;
        else if (type == MessageType.Sub)
        {
            msg.Color = Color.red;
            PlaySound();
        }
        else if (type == MessageType.Tutorial)
            msg.Color = Color.white;

        m_messageDic[type].Add(msg);
    }

    public void PushMessage(MessageType type, string str, Color color)
    {
        Message msg = new Message()
        {
            Str = str,
            Color = color,
        };

        m_messageDic[type].Add(msg);
    }

    public void Clear()
    {

    }

    public void LateUpdate()
    {
        m_mainElapsedTime += Time.deltaTime;

        if (m_messageDic[MessageType.Main].Count > m_mainPushCount)
            m_useMain = true;

        if (m_messageDic[MessageType.Sub].Count > m_subPushCount)
        {
            m_subElspasedTime = 0;
            m_subPushCount = m_messageDic[MessageType.Sub].Count;
            m_subMessage.text = m_messageDic[MessageType.Sub][m_subPushCount - 1].Str;
            m_useSub = true;
        }
        if (m_useMain)
        {
            if(m_mainElapsedTime>2)
            {
                for (int i = 0; i < m_mainMessageList.Count; ++i)
                {
                    if (!m_mainMessageList[i].gameObject.activeSelf)
                    {
                        m_mainMessageList[i].Enabled(m_messageDic[MessageType.Main][m_mainPushCount]);
                        goto Setting;
                    }
                }

                MainMessage message = Instantiate(Resources.Load<MainMessage>("UI/Instance/MainMessage"), m_mainMessageGrid);
                m_mainMessageList.Add(message);
                message.Init();
                message.Enabled(m_messageDic[MessageType.Main][m_mainPushCount]);

                Setting:
                m_mainElapsedTime = 0;
                m_useMain = false;
                ++m_mainPushCount;
            }
        }
        if (m_useSub)
        {
            m_subElspasedTime += Time.deltaTime;
            Message msg = m_messageDic[MessageType.Sub][m_subPushCount - 1];

            if (m_subElspasedTime < 2.2f)
            {
                m_subMessage.color = Color.Lerp(Color.clear, msg.Color, m_subElspasedTime * 5f);
                return;
            }
            if (m_subElspasedTime < 0.4f + 2 && m_subElspasedTime > 2.2f)
            {
                m_subMessage.color = Color.Lerp(msg.Color, Color.clear, (m_subElspasedTime - (2.2f)) * 5f);
                return;
            }

            m_useSub = false;
            m_subMessage.text = null;
        }
    }
}
