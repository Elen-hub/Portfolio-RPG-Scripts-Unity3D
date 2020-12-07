using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LetterBox : MonoBehaviour
{
    Image m_leftBox;
    Image m_rightBox;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        m_leftBox = transform.Find("Left").GetComponent<Image>();
        m_rightBox = transform.Find("Right").GetComponent<Image>();
        float scaleX = Screen.width - 1920;
        float scaleY = Screen.height - 1080;
        m_leftBox.rectTransform.sizeDelta = new Vector2(scaleX / 2, 1080);
        m_leftBox.rectTransform.localPosition = new Vector3(-Screen.width / 2 + scaleX / 4, 0, 0);
        m_rightBox.rectTransform.sizeDelta = new Vector2(scaleX / 2, 1080);
        m_rightBox.rectTransform.localPosition = new Vector3(Screen.width / 2 - scaleX / 4, 0, 0);
    }
}
