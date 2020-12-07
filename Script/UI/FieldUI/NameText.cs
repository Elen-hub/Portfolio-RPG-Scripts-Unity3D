using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NameText : BaseFieldUI
{
    BaseCharacter m_character;
    Transform m_camera;
    Color m_color;
    Transform m_pivot;
    Text m_nameText;
    public override void Init(FieldUI.Register register)
    {
        base.Init(register);
        m_camera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera.transform;
        m_nameText = GetComponentInChildren<Text>();
    }

    public void Enabled(BaseCharacter target, string name, Color color)
    {
        m_pivot = target.AttachSystem.GetAttachPoint(EAttachPoint.Name);
        m_character = target;
        m_color = color;
        m_nameText.text = name;
        m_nameText.color = m_color;
        m_nameText.enabled = false;
        gameObject.SetActive(true);
    }
    public void Enabled(BaseCharacter target, string name)
    {
        m_pivot = target.AttachSystem.GetAttachPoint(EAttachPoint.Name);
        m_color = Color.white;
        m_nameText.text = name;
        m_nameText.enabled = false;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        dRegister(FieldUI.EUIFieldType.NameText, this);
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if(m_character == null)
        {
            Disabled();
            return;
        }
        if (!m_character.gameObject.activeSelf)
        {
            Disabled();
            return;
        }

        transform.position = m_pivot.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        m_nameText.enabled = true;
    }
    private void OnTriggerExit(Collider other)
    {
        m_nameText.enabled = false;
    }
}
