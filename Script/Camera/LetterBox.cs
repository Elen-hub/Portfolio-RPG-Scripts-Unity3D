using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LetterBox : MonoBehaviour
{
    Text m_frame;
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
        m_leftBox.rectTransform.localPosition = new Vector3(-Screen.width/2 + scaleX / 4, 0, 0);
        m_rightBox.rectTransform.sizeDelta = new Vector2(scaleX / 2, 1080);
        m_rightBox.rectTransform.localPosition = new Vector3(Screen.width/2 - scaleX / 4, 0, 0);
        m_frame = transform.Find("Frame").GetComponent<Text>();
    }

    private void Update()
    {
        m_frame.text = (1 / Time.deltaTime).ToString("F0") + "FPS (" + (Time.deltaTime* 1000).ToString("F0") + "ms)";
    }
}
