using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubWindow_PartyBTN : EnergyBarBase
{
    public Player Player;
    Text m_nameText;

    public override void Init()
    {
        base.Init();
        m_nameText = GetComponentInChildren<Text>();
    }
    public void Enabled(Player player)
    {
        Player = player;
        m_nameText.text = player.Name;
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        Player = null;
        gameObject.SetActive(false);
    }
    protected void LateUpdate()
    {
        if (Player != null && Player.Character != null)
        {
            m_nameText.color = Color.white;
            float currentFill = Img.fillAmount;
            float targetFill = Player.Character.StatSystem.CurrHP / Player.Character.StatSystem.GetHP;
            Img.fillAmount = currentFill + (targetFill - currentFill) * Time.deltaTime * 1.5f;
        }
        else
        {
            m_nameText.color = Color.grey;
            Img.fillAmount = 0;
        }
    }
}
