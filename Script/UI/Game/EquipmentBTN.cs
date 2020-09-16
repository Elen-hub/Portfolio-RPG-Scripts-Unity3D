using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentBTN : MonoBehaviour
{
    Item_Base m_item;
    Text m_nameText;
    EItemType m_type;
    Image m_icon;
    float m_touchElapsedTime;
    bool m_doubleTouch;

    public EquipmentBTN Init(EItemType type)
    {
        GetComponent<Button>().onClick.AddListener(OnClickDisarm);
        m_icon = transform.Find("Icon").GetComponent<Image>();
        m_nameText = transform.Find("Text").GetComponent<Text>();
        m_type = type;
        return this;
    }
    public void Enabled(BaseCharacter character)
    {
        Item_Equipment item = character.StatSystem.GetEquipment(m_type);

        if(item == null)
        {
            m_nameText.gameObject.SetActive(false);
            m_item = null;
            m_icon.sprite = Resources.Load<Sprite>("Sprite/EquipBTN");
            return;
        }
        m_item = item;
        string text = "<color=" + GameSystem.ColorRarity[(int)item.Rarity] + ">";
        if(item.Value != 0)
            text += "+" + item.Value + " ";
        text += item.Name + "</color> \n " + ParseLib.GetRairityKorConvert(item.Rarity);
        m_nameText.text = text;
        m_nameText.gameObject.SetActive(true);
        m_icon.sprite = Resources.Load<Sprite>(m_item.Icon);
        m_icon.material = Resources.Load<Material>("Material/ItemMaterial_" + m_item.Rarity);
    }
    void OnClickDisarm()
    {
        if(m_doubleTouch)
        {
            m_doubleTouch = false;

            if (m_item != null)
                NetworkMng.Instance.RequestItemUnequip(m_type);
        }
        else
        {
            UIMng.Instance.Open<Inventory>(UIMng.UIName.Inventory).Open(false);
            CameraMng.Instance.SetCamera(CameraMng.CameraStyle.UI | CameraMng.CameraStyle.Player);
            m_doubleTouch = true;
            m_touchElapsedTime = 0;
        }
    }
    private void LateUpdate()
    {
        if(m_doubleTouch)
        {
            m_touchElapsedTime += Time.deltaTime;
            if (m_touchElapsedTime > GameSystem.DoubleTouchTime)
            {
                m_doubleTouch = false;
                m_touchElapsedTime = 0;
            }
        }
    }
}
