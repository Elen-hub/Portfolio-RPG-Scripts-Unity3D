using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPopup_Item : SelectPopup_Base
{
    int m_value;
    protected IntFunction SuccessFunction;
    protected VoidFunction ExitFunction;
    Image m_itemImg;
    Text m_itemGoldText;
    public override void Init()
    {
        base.Init();

        m_itemImg = transform.Find("ItemIcon").GetComponent<Image>();
        m_itemGoldText = transform.Find("ItemGoldText").GetComponent<Text>();
        transform.Find("Success").GetComponent<Button>().onClick.AddListener(Surcess);
        transform.Find("Exit").GetComponent<Button>().onClick.AddListener(Exit);
        gameObject.SetActive(false);
    }
    public void Enabled(IntFunction SuccessF, string SucceesT, VoidFunction ExitF, string ExitT, string Status, int value, string spritePath, string OptionStatus)
    {
        m_value = value;
        m_itemImg.sprite = Resources.Load<Sprite>(spritePath);
        m_itemGoldText.text = OptionStatus;
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
        m_value = 0;

        gameObject.SetActive(false);
    }
    void Surcess()
    {
        SuccessFunction?.Invoke(m_value);
        UIMng.Instance.CLOSE = UIMng.UIName.SelectPopup;
    }

    void Exit()
    {
        ExitFunction?.Invoke();
        UIMng.Instance.CLOSE = UIMng.UIName.SelectPopup;
    }
}
