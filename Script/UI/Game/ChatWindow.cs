using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindow : MonoBehaviour
{
    List<ChatMessage> m_messageList = new List<ChatMessage>();
    bool m_isOpen;
    Animator m_animator;
    RectTransform m_grid;
    InputField m_chatField;
    Scrollbar m_scrollBar;

    public void InputChat()
    {
        if (m_chatField.text == null || m_chatField.text == "")
            return;

        // 네트워크를 통해 채팅을 전달한다.
        // 임시로 그냥 표시될 수 있도록 함.
        string value = m_chatField.text;
        NetworkMng.Instance.NotifySendChat(value);
        m_chatField.text = null;
    }
    public void AddChat(string value)
    {
        ChatMessage message;
        if (m_messageList.Count >=100)
        {
            Destroy(m_messageList[0].gameObject);
            m_messageList.RemoveAt(0);
            return;
        }
        message = Instantiate(Resources.Load<ChatMessage>("UI/Instance/ChatMessage"), m_grid);
        message.Init();
        message.Enabled(value);
        m_messageList.Add(message);
        m_scrollBar.value = 0;
    }
    public void Init()
    {
        NetworkMng.Instance.AddChat = AddChat;
        m_animator = GetComponent<Animator>();
        transform.Find("ActiveButton").GetComponent<Button>().onClick.AddListener(Active);
        transform.Find("ChatIcon").GetComponent<Button>().onClick.AddListener(InputChat);
        m_grid = GetComponentInChildren<VerticalLayoutGroup>(true).GetComponent<RectTransform>();
        m_grid.gameObject.SetActive(true);
        m_chatField = GetComponentInChildren<InputField>();
        m_scrollBar = GetComponentInChildren<Scrollbar>();
        Disabled();
    }
    public void Enabled()
    {
        m_isOpen = true;
        gameObject.SetActive(true);
        m_animator.Play("Open");
    }
    public void Disabled()
    {
        m_isOpen = false;
        m_animator.Play("Close");
    }
    void Active()
    {
        if (m_isOpen) Disabled();
        else Enabled();
    }
}
