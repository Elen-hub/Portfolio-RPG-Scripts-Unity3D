using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNameWindow : MonoBehaviour
{
    Text m_text;
    public void Init()
    {
        m_text = GetComponentInChildren<Text>();
        gameObject.SetActive(false);
    }
    public void Enabled()
    {
        m_text.text = MapMng.Instance.CurrMap.MapName;
        m_text.color = Color.clear;
        gameObject.SetActive(true);
        StartCoroutine(FadeColor());
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    IEnumerator FadeColor()
    {
        yield return null;
        WaitForSeconds wait = new WaitForSeconds(0.02f);
        Color m_prevColor = new Color(1, 1, 1, 0);
        for(int i=0;i<75; ++i)
        {
            m_prevColor.a = Mathf.Lerp(0, 1, i / 74f);
            m_text.color = m_prevColor;
            yield return wait;
        }
        yield return new WaitForSeconds(1.5f);
        for(int i =0; i<50; ++i)
        {
            m_prevColor.a = Mathf.Lerp(1, 0, i / 49f);
            m_text.color = m_prevColor;
            yield return wait;
        }
        gameObject.SetActive(false);
    }
}
