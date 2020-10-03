using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMessage : MonoBehaviour
{
    Stack<MainMessage> m_stack;
    Text m_text;
    Color m_color = Color.white;
    Vector3 m_nextPos;
    float m_targetTime = 3;
    float m_elapsedTime;
    public MainMessage Init(ref Stack<MainMessage> stack)
    {
        m_text = GetComponentInChildren<Text>();
        m_stack = stack;
        return this;
    }
    public void Enabled(SystemMessage.Message message)
    {
        transform.localPosition = Vector3.zero;
        m_text.text = message.Str;
        m_color = message.Color;
        m_elapsedTime = 0;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        m_stack.Push(this);
        gameObject.SetActive(false);
    }
    private void Update()
    {
        m_elapsedTime += Time.deltaTime;
        if(m_elapsedTime < 0.5f)
        {
            m_text.color = Color.Lerp(Color.clear, m_color, m_elapsedTime * 2);
        }
        else
        {
            m_nextPos = transform.localPosition;
            m_nextPos.y += Time.deltaTime * 30;
            transform.localPosition = m_nextPos;

            if (m_elapsedTime > m_targetTime - 0.5f)
                m_text.color = Color.Lerp(m_color, Color.clear, (m_elapsedTime - 2.5f)*2);
        }
        if (m_elapsedTime > m_targetTime)
            Disabled();
    }
}
