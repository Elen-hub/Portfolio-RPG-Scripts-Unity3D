using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPopup_Base : MonoBehaviour
{
    protected Text m_successText;
    protected Text m_exitText;
    protected Text m_status;

    public virtual void Init()
    {
        Transform Status = transform.Find("Text");
        if(Status != null) m_status = Status.GetComponent<Text>();
        Transform Success = transform.Find("Success");
        if (Success != null) m_successText = Success.GetComponentInChildren<Text>();
        Transform Exit = transform.Find("Exit");
        if(Exit != null) m_exitText = Exit.GetComponentInChildren<Text>();
    }
}
