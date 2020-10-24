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
    }
    
    public struct Message
    {
        public string Str;
        public Color Color;
        public Sprite Icon;
    }

    Text m_itemMessage;
    Stack<MainMessage> m_mainMessageStack = new Stack<MainMessage>();
    Transform m_mainMessageGrid;
    Text m_subMessage;
    Queue<Message> m_mainQueue = new Queue<Message>();

    float m_mainElapsedTime;
    float m_subElspasedTime;

    bool m_useMain;
    bool m_useSub;

    protected override void InitUI()
    {
        m_mainMessageGrid = transform.Find("MainMessageGrid");
        m_subMessage = transform.Find("SubMessage").GetComponent<Text>();

        Instance = this;
    }
    public void PushMessage(MessageType type , string str)
    {
        if (type == MessageType.Main)
        {
            Message msg = new Message()
            {
                Str = str,
                Color = Color.white,
            };
            if (m_useMain)
            {
                m_mainQueue.Enqueue(msg);
            }
            else
            {
                m_useMain = true;
                PlayMainMessage(msg);
            }
        }
        else if (type == MessageType.Sub)
        {
            m_subElspasedTime = 0;
            m_subMessage.text = str;
            PlaySound();

            m_useSub = true;
        }
    }
    void PlayMainMessage(Message msg)
    {
        Stack<string> a = new Stack<string>();
        a.ToArray();
        MainMessage mainMessage;
        if (m_mainMessageStack.Count > 0)
            mainMessage = m_mainMessageStack.Pop();
        else
            mainMessage = Instantiate(Resources.Load<MainMessage>("UI/Instance/MainMessage"), m_mainMessageGrid).Init(ref m_mainMessageStack);

        mainMessage.Enabled(msg);
        m_mainElapsedTime = 0;
    }
    public void LateUpdate()
    {
        if (m_useMain)
        {
            m_mainElapsedTime += Time.deltaTime;
            if (m_mainElapsedTime>2)
            {
                m_useMain = m_mainQueue.Count > 0;
                if (m_useMain)
                    PlayMainMessage(m_mainQueue.Dequeue());
            }
        }
        if (m_useSub)
        {
            m_subElspasedTime += Time.deltaTime;
            if (m_subElspasedTime < 2.2f)
            {
                m_subMessage.color = Color.Lerp(Color.clear, Color.red, m_subElspasedTime * 5f);
                return;
            }
            if (m_subElspasedTime < 0.4f + 2 && m_subElspasedTime > 2.2f)
            {
                m_subMessage.color = Color.Lerp(Color.red, Color.clear, (m_subElspasedTime - (2.2f)) * 5f);
                return;
            }

            m_useSub = false;
            m_subMessage.text = null;
        }
    }
}
