using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI_Train : MonoBehaviour
{
    Animator m_animator;
    Text m_trainText;
    public void Init()
    {
        m_animator = GetComponent<Animator>();
        m_trainText = transform.Find("Train").GetComponentInChildren<ContentSizeFitter>().GetComponentInChildren<Text>();
        transform.Find("Train").Find("Exit").GetComponent<Button>().onClick.AddListener(Disabled);
        gameObject.SetActive(false);
    }
    public void Enabled(int trainHandle)
    {
        gameObject.SetActive(true);
        m_animator.Play("Open");
    }
    public void Disabled()
    {
        UIMng.Instance.GetUI<NPCUI>(UIMng.UIName.NPCUI).ShopUI.Close();
        m_animator.Play("Close");
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
