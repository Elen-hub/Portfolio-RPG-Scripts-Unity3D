using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : BaseFieldUI
{
    BaseCharacter m_character;
    // Transform m_camera;
    Text m_text;
    Image m_img;
    float m_elapsedTime;
    public override void Init(FieldUI.Register register)
    {
        base.Init(register);
        // m_camera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera.transform;
        m_text = GetComponentInChildren<Text>();
        m_img = GetComponent<Image>();
    }
    public void Enabled(BaseCharacter character, string text)
    {
        if (character.ChatBox != null)
            character.ChatBox.Disabled();

        m_elapsedTime = 0;
        m_text.text = text;
        m_character = character;
        transform.position = m_character.AttachSystem.GetAttachPoint(EAttachPoint.ChatBox).position;
        character.ChatBox = this;
        m_text.enabled = false;
        m_img.enabled = false;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        m_text.text = null;
        m_character.ChatBox = null;
        m_character = null;
        dRegister(FieldUI.EUIFieldType.ChatBox, this);
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if(m_character == null)
        {
            Disabled();
            return;
        }

        m_elapsedTime += Time.deltaTime;

        transform.position = m_character.AttachSystem.GetAttachPoint(EAttachPoint.ChatBox).position;
        // transform.LookAt(m_camera);

        if (m_elapsedTime > 5)
            Disabled();
    }
    private void OnTriggerEnter(Collider other)
    {
        m_text.enabled = true;
        m_img.enabled = true;
    }
    private void OnTriggerExit(Collider other)
    {
        m_text.enabled = false;
        m_img.enabled = false;
    }
}
