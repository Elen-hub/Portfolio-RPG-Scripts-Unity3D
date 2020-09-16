using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title_CreateCharacterBTN : MonoBehaviour
{
    int m_handle;
    Image m_img;
    public void Init(int handle)
    {
        m_handle = handle+1;
        m_img = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnClickSelect);
    }
    void OnClickSelect()
    {
        UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).CreateAccount.CreateCharacter.SetHandle(m_handle);
    }
}
