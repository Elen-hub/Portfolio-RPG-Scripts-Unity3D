using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HPBar : EnergyBarBase
{
    static Color BaseColor = new Color(1, 0.25f, 0.25f, 1);
    BaseCharacter m_character;
    Transform m_camera;
    public override void Init()
    {
        base.Init();

        m_camera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera.transform;
    }

    public void Enabled(BaseCharacter target, Color color)
    {
        m_character = target;
        Img.color = color;
        Img.enabled = false;
        gameObject.SetActive(true);
    }
    public void Enabled(BaseCharacter target)
    {
        m_character = target;
        Img.color = BaseColor;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        gameObject.SetActive(false);
    }
    protected void LateUpdate()
    {
        if (m_character == null)
        {
            Disabled();
            return;
        }
        if (!m_character.gameObject.activeSelf)
        {
            Disabled();
            return;
        }
        float currentFill = Img.fillAmount;
        float targetFill = m_character.StatSystem.CurrHP / m_character.StatSystem.GetHP;
        Img.fillAmount = currentFill + (targetFill - currentFill) * Time.deltaTime * 1.5f;
        transform.position = m_character.AttachSystem.GetAttachPoint(EAttachPoint.HP).position;


        // transform.LookAt(m_camera);
    }

    private void OnTriggerEnter(Collider other)
    {
        Img.enabled = true;
    }
    private void OnTriggerExit(Collider other)
    {
        Img.enabled = false;
    }
}
