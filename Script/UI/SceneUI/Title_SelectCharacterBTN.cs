using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title_SelectCharacterBTN : MonoBehaviour
{
    public int Number;
    public Player Player;
    public Transform Character;
    public GameObject Cornor;
    public GameObject Add;
    public Image Icon;
    public Text Name;
    public Text Level;
    Transform m_pos;
    public Title_SelectCharacterBTN Init(int number)
    {
        Number = number;
        Icon = GetComponent<Image>();
        Level = transform.Find("Level").GetComponent<Text>();
        Name = transform.Find("Name").GetComponent<Text>();
        Cornor = transform.Find("Cornor").gameObject;
        Add = transform.Find("Add").gameObject;
        m_pos = GameObject.Find("Pos" + number).transform;
        GetComponent<Button>().onClick.AddListener(OnClickBTN);
        return this;
    }
    public void Enabled(string data)
    {
        string[] Datas = data.Split(',');
        Player = new Player();
        int handle = int.Parse(Datas[0]);
        string name = Datas[1];
        int level = int.Parse(Datas[2]);
        Character = CharacterMng.Instance.InstantiatePreview(handle);
        Character.SetParent(m_pos);
        Character.localPosition = Vector3.zero;
        Character.localRotation = Quaternion.identity;
        Icon.sprite = Resources.Load<Sprite>(CharacterMng.Instance.GetCharacterStat(handle).Icon);
        Name.text = name;
        Level.text = "Lv." + level;

        Player.Handle = handle;
        Player.Name = name;
        Player.Level = level;

        Add.SetActive(false);
        Cornor.SetActive(false);
    }
    public void Disabled()
    {
        Icon.sprite = null;
        Level.text = null;
        if(Character != null)
            DestroyImmediate(Character.gameObject);
        Player = null;
        Add.SetActive(true);
        Cornor.SetActive(false);
    }
    public void OnClickBTN()
    {
        if (UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).SelectCharacter.CurrSelect != -1)
            UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).SelectCharacter.Characters[UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).SelectCharacter.CurrSelect].Cornor.SetActive(false);

        if (Player != null)
        {
            Cornor.SetActive(true);
            UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).SelectCharacter.SelectHandle(Number);
            // 프리뷰생성
        }
        else
        {
            UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).SelectCharacter.Close();
            UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).CreateAccount.Open();
        }
    }
}