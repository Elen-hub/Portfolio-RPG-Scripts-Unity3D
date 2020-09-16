using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyListBTN : MonoBehaviour
{
    int m_number;
    public Party Party;
    Image m_cornorImg;
    Text m_nameText;
    Text m_hostText;
    Text m_numberText;
    public PartyListBTN Init()
    {
        GetComponent<Button>().onClick.AddListener(OnClickSelect);
        m_cornorImg = transform.Find("Cornor").GetComponent<Image>();
        m_nameText = transform.Find("Name").GetComponent<Text>();
        m_hostText = transform.Find("Host").GetComponent<Text>();
        m_numberText = transform.Find("Number").GetComponent<Text>();
        return this;
    }
    public void Enabled(Party party, int number, int currNumber)
    {
        m_number = number;
        Party = party;
        m_nameText.text = party.Name;
        m_hostText.text = PlayerMng.Instance.PlayerList[party.PartyHost].Name;
        m_numberText.text = currNumber + "/" +party.Number;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        ChangeColor(Color.white);
        gameObject.SetActive(false);
    }
    void OnClickSelect()
    {
        ChangeColor(Color.red);
        UIMng.Instance.GetUI<PartyMenu>(UIMng.UIName.PartyMenu).SelectHandle = m_number;
    }
    public void ChangeColor(Color color)
    {
        m_cornorImg.color = color;
    }
}
