using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option_Preview : MonoBehaviour
{
    Image m_img;
    Text m_text;
   public Option_Preview Init()
    {
        m_img = transform.Find("Image").GetComponent<Image>();
        m_text = transform.Find("Text").GetComponent<Text>();
        return this;
    }
    public void Open(string imagePath, string textArr)
    {
        m_img.sprite = Resources.Load<Sprite>("Sprite/OptionImg/" + imagePath);
        m_text.text = textArr;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
#if UNITY_EDITOR
        if (0 < Input.touchCount)
            if (Input.GetTouch(0).phase == TouchPhase.Began)
                Close();
#else
        if (Input.GetMouseButtonDown(0))
            Close();
#endif
    }
}
