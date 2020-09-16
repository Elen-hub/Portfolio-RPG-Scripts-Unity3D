using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPopup_Normal : SelectPopup_Base
{
    protected VoidFunction SuccessFunction;
    protected VoidFunction ExitFunction;

    public override void Init()
    {
        base.Init();

        transform.Find("Success").GetComponent<Button>().onClick.AddListener(Surcess);
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Exit);
        gameObject.SetActive(false);
    }
    public void Enabled(VoidFunction SuccessF, string SucceesT, VoidFunction ExitF, string ExitT, string Status)
    {
        SuccessFunction = SuccessF;
        m_successText.text = SucceesT;
        ExitFunction = ExitF;
        m_exitText.text = ExitT;
        m_status.text = Status;

        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        SuccessFunction = null;
        ExitFunction = null;

        gameObject.SetActive(false);
    }
    void Surcess()
    {
        SuccessFunction?.Invoke();
        UIMng.Instance.CLOSE = UIMng.UIName.SelectPopup;
    }

    void Exit()
    {
        ExitFunction?.Invoke();
        UIMng.Instance.CLOSE = UIMng.UIName.SelectPopup;
    }
}
