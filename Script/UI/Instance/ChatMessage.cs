using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    Text m_text;
    public void Init()
    {
        m_text = GetComponentInChildren<Text>();
    }

    public void Enabled(string text)
    {
        m_text.text = text;
    }
}
