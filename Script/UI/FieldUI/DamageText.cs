using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DamageText : BaseFieldUI
{
    // Transform m_camera;
    Transform m_criticalTrs;
    Text m_criticalText;
    Transform m_damageTrs;
    Text m_damageText;
    Color m_color;
    Vector3 m_textPos;
    Vector3 m_nextPos;
    Vector3 m_reverseVector3 = new Vector3(1, 1, 1);
    float m_elapsedTime;
    public override void Init(FieldUI.Register register)
    {
        base.Init(register);
        m_criticalTrs = transform.Find("CriticalText");
        m_criticalText = m_criticalTrs.GetComponent<Text>();
        m_damageTrs = transform.Find("DamageText");
        m_damageText = m_damageTrs.GetComponent<Text>();
        // m_camera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera.transform;
    }

    public void Enabled(BaseCharacter target, string Damage, Color color, bool isCritical = false)
    {
        m_criticalTrs.gameObject.SetActive(isCritical);
        m_textPos = target.AttachSystem.GetAttachPoint(EAttachPoint.UnderHead).position;
        m_textPos.x += Random.Range(-0.5f, 0.5f);
        m_textPos.y += Random.Range(-0.05f, 0.2f);
        m_nextPos = Vector3.zero;
        m_elapsedTime = 0;
        m_damageText.color = color;
        m_damageText.text = Damage;
        m_color = color;
        transform.position = m_nextPos + m_textPos;
        transform.localScale = m_reverseVector3 * 0.5f * 0.15f;
        m_damageText.enabled = false;
        m_criticalText.enabled = false;
        gameObject.SetActive(true);
    }
    public void Enabled(BaseCharacter target, float Damage, bool isCritical = false)
    {
        if(isCritical)
        {
            m_damageText.fontStyle = FontStyle.Italic;
            m_criticalTrs.gameObject.SetActive(true);
        }
        else
        {
            m_damageText.fontStyle = FontStyle.Normal;
            m_criticalTrs.gameObject.SetActive(false);
        }
        m_textPos = target.AttachSystem.GetAttachPoint(EAttachPoint.UnderHead).position;
        m_textPos.x += Random.Range(-1.5f, 1.5f);
        m_textPos.y += Random.Range(-2f, 0.5f);
        m_nextPos = Vector3.zero;
        m_elapsedTime = 0;
        m_damageText.color = Color.red;
        m_damageText.text = Damage.ToString("F0");
        transform.position = m_nextPos + m_textPos;
        transform.localScale = m_reverseVector3 * 0.5f * 0.15f;
        m_damageText.enabled = false;
        m_criticalText.enabled = false;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        dRegister(FieldUI.EUIFieldType.DamageText, this);
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        m_elapsedTime += Time.deltaTime;

        if (m_elapsedTime < 0.25f)
            transform.localScale = m_reverseVector3 * (m_elapsedTime * 2 + 0.5f) * 0.15f;
        if (m_elapsedTime > 1f)
            m_damageText.color = m_color * (1.5f - m_elapsedTime);

        m_nextPos.y += m_elapsedTime * Time.deltaTime * 0.1f;
        transform.position = m_nextPos + m_textPos;
        // transform.LookAt(m_camera);

        if (m_elapsedTime > 1.5f)
            Disabled();
    }
    private void OnTriggerEnter(Collider other)
    {
        m_damageText.enabled = true;
        m_criticalText.enabled = true;
    }
    private void OnTriggerExit(Collider other)
    {
        m_damageText.enabled = false;
        m_criticalText.enabled = false;
    }
}
