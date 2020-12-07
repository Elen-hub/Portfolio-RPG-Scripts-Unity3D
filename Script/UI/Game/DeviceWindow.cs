using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceWindow : MonoBehaviour
{
    float m_elasedTime;
    [Range(0, 1)] float m_battery;
    Image m_batteryImg;
    Text m_pingTime;
    Dropdown m_channel;

    public int CurrChannel {
        set {
            if (m_channel.value == value)
                return;

            m_channel.enabled = false;
            m_channel.value = value;
            m_channel.enabled = true;
        } 
    }
    public void Init()
    {
        m_batteryImg = transform.Find("BatteryFill").GetComponent<Image>();
        m_pingTime = transform.Find("Ping").GetComponent<Text>();
        m_channel = transform.Find("ChannelDropDown").GetComponent<Dropdown>();
        m_channel.onValueChanged.AddListener(ChannelChange);
    }
    public void Enabled()
    {
        m_channel.enabled = MapMng.Instance.CurrMap.Type == EMapType.Public;
        gameObject.SetActive(true);
    }
    
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    void ChannelChange(int value)
    {
        if (MapMng.Instance.CurrMap.Type == EMapType.Private)
            return;

        NetworkMng.Instance.RequestChangePublicChannel(value);
    }
    private void LateUpdate()
    {
        m_elasedTime += Time.deltaTime;
        if(m_elasedTime>2)
        {
            m_elasedTime = 0;
            m_battery = SystemInfo.batteryLevel;
            m_pingTime.text = NetworkMng.Instance.GetPing().ToString();

            if (m_battery > 0.7f)
                m_batteryImg.color = Color.green;
            else if (m_battery>0.3f) m_batteryImg.color = Color.yellow;
            else m_batteryImg.color = Color.red;

            m_batteryImg.fillAmount = m_battery;
        }
    }
}
