using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI_Quest : MonoBehaviour
{
    List<QuestBTN> m_questList = new List<QuestBTN>();
    Animator m_animator;
    Transform m_grid;
    public void Init()
    {
        transform.GetChild(0).Find("Exit").GetComponent<Button>().onClick.AddListener(Disabled);
        m_animator = GetComponent<Animator>();
        m_grid = transform.GetComponentInChildren<ContentSizeFitter>().transform;
        gameObject.SetActive(false);
    }
    public void Enabled(List<Quest> questList)
    {
        for(int i =0; i< questList.Count; ++i)
        {
            if (m_questList.Count <= i)
                m_questList.Add(Instantiate(Resources.Load<QuestBTN>("UI/Instance/QuestBTN"), m_grid).Init());

            m_questList[i].Enabled(questList[i]);
        }
        gameObject.SetActive(true);
        m_animator.Play("Open");
    }
    public void Disabled()
    {
        for (int i = 0; i < m_questList.Count; ++i)
            m_questList[i].Disabled();

        m_animator.Play("Close");
    }
    public void Close()
    {
        for (int i = 0; i < m_questList.Count; ++i)
            m_questList[i].Disabled();

        gameObject.SetActive(false);
    }
}
