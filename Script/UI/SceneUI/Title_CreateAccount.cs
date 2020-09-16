using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Title_CreateAccount : MonoBehaviour
{
    public Title_CreateCharacter CreateCharacter;
    GameObject m_privacyPolicy;
    GameObject m_accountUseGuid;
    GameObject m_createField;
    Image m_bgMask;

    public InputField m_id;
    public InputField m_pwd;
    public InputField m_pwdConfirm;
    public InputField m_nickName;
    public InputField m_email;
    public InputField m_phoneNumber;

    public Text m_publishText;
    public Text m_privacyText;
    public Text m_privacyText2;

    public Text m_idHelp;
    public Text m_pwdHelp;
    public Text m_pwdConfirmHelp;
    public Text m_nickNameHelp;
    public Text m_emailHelp;
    public Text m_phoneNumberHelp;

    Transform m_camera;
    Vector3 m_prevPos;
    Vector3 m_prevAngle;
    Vector3 m_targetPos = new Vector3(-11.75f, -0.6f, -12.67f);
    Vector3 m_targetAngle = new Vector3(5, 180, 0);
    float m_elapsedTime;

    public void Init()
    {
        m_camera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.World).transform;

        CreateCharacter = UIMng.Instance.GetUI<Title>(UIMng.UIName.Title).GetComponentInChildren<Title_CreateCharacter>();
        CreateCharacter.Init();
        m_privacyPolicy = transform.Find("PrivacyPolicy").gameObject;
        m_privacyPolicy.transform.Find("Surcess").GetComponent<Button>().onClick.AddListener(() => StartCoroutine(FadeUI(m_privacyPolicy, m_accountUseGuid)));
        m_accountUseGuid = transform.Find("AccountUseGuid").gameObject;
        // m_accountUseGuid.transform.Find("Surcess").GetComponent<Button>().onClick.AddListener(() => StartCoroutine(FadeUI(m_accountUseGuid, m_createField)));
        m_accountUseGuid.transform.Find("Surcess").GetComponent<Button>().onClick.AddListener(() => { StartCoroutine(FadeUI(gameObject, null)); CreateCharacter.Open(); });
        m_createField = transform.Find("Create").gameObject;
        m_bgMask = transform.Find("BGMask").GetComponent<Image>();

        m_id = m_createField.transform.Find("ID").GetComponent<InputField>();
        m_pwd = m_createField.transform.Find("Password").GetComponent<InputField>();
        m_pwdConfirm = m_createField.transform.Find("ConfirmPassword").GetComponent<InputField>();
        m_nickName = m_createField.transform.Find("NickName").GetComponent<InputField>();
        m_email = m_createField.transform.Find("Email").GetComponent<InputField>();
        m_phoneNumber = m_createField.transform.Find("PhoneNumber").GetComponent<InputField>();

        m_publishText = m_createField.transform.Find("PublicyPolicy").GetComponent<Text>();
        m_privacyText = m_createField.transform.Find("PrivacyPolicy").GetComponent<Text>();
        m_privacyText2 = m_createField.transform.Find("PrivacyPolicy2").GetComponent<Text>();
        m_idHelp = m_createField.transform.Find("IDHelp").GetComponent<Text>();
        m_pwdHelp = m_createField.transform.Find("PWDHelp").GetComponent<Text>();
        m_pwdConfirmHelp = m_createField.transform.Find("ConfirmPWDHelp").GetComponent<Text>();
        m_nickNameHelp = m_createField.transform.Find("NickNameHelp").GetComponent<Text>();
        m_emailHelp = m_createField.transform.Find("EmailHelp").GetComponent<Text>();
        m_phoneNumberHelp = m_createField.transform.Find("PhoneNumberHelp").GetComponent<Text>();

        m_createField.transform.Find("Submit").GetComponent<Button>().onClick.AddListener(Submit);

        transform.Find("Cancle").GetComponent<Button>().onClick.AddListener(() =>
        {
            NetworkMng.Instance.RequestPlayerCharactersInformation();
            Close();
        });

        m_privacyPolicy.SetActive(false);
        m_accountUseGuid.SetActive(false);
        m_createField.SetActive(false);
    }

    void Submit()
    {
        //if (NetworkMng.Instance.NetworkState != ENetworkState.ConnectOn)
        //{
        //    SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "서버상태에 접속 할 수 없습니다./Can't connect to the server.");
        //    return;
        //}

        bool isSubmit = true;

        // ResetText();

        if (m_id.text.Length <8)
        {
            m_idHelp.color = Color.red;
            m_idHelp.text = "ID는 8자 이상 입력해야합니다.";
            isSubmit = false;
        }

        string idChecker = Regex.Replace(m_id.text, @"[^0-9a-zA-Z가-힣]{1,10}", "", RegexOptions.Singleline);

        if(m_id.text.Equals(idChecker) == false)
        {
            m_idHelp.color = Color.red;
            m_idHelp.text = "아이디는 특수문자가 들어갈 수 없습니다.";
            isSubmit = false;
        }

        if(m_pwd.text.Length<8)
        {
            m_pwdHelp.color = Color.red;
            m_pwdHelp.text = "비밀번호는 8자 이상 입력해야합니다.";
            isSubmit = false;
        }

        string pwdChecker = Regex.Replace(m_pwd.text, @"[^0-9a-zA-Z가-힣]{1,10}!@#$%^&*-_=+`", "", RegexOptions.Singleline);

        if (m_pwd.text.Equals(pwdChecker) == false)
        {
            m_pwdHelp.color = Color.red;
            m_pwdHelp.text = "비밀번호는 공백, !@#$%^&*-_=+`를 제외한 특수문자를 입력 할 수 없습니다.";
            isSubmit = false;
        }

        if(m_pwd.text.Equals(m_pwdConfirm.text) == false)
        {
            m_pwdConfirmHelp.color = Color.red;
            m_pwdConfirmHelp.text = "비밀번호가 일치하지 않습니다.";
            isSubmit = false;
        }

        string nickNameChecker = Regex.Replace(m_nickName.text, @"[^0-9a-zA-Z가-힣]{1,10}-_@", "", RegexOptions.Singleline);

        if(m_nickName.text == null)
        {
            m_nickNameHelp.color = Color.red;
            m_nickName.text = "닉네임을 입력하세요.";
            isSubmit = false;
        }

        if(m_nickName.text.Equals(nickNameChecker) == false)
        {
            m_nickNameHelp.color = Color.red;
            m_nickNameHelp.text = "닉네임은 공백, @-_를 제외한 특수문자를 입력 할 수 없습니다.";
            isSubmit = false;
        }

        //if (isSubmit)
        //    NetworkMng.Instance.CreataeAccount(m_id.text, m_pwd.text, m_nickName.text, m_email.text, m_phoneNumber.text);
    }

    //public void ResetText()
    //{
    //    m_idHelp.color = Color.white;
    //    m_pwdHelp.color = Color.white;
    //    m_pwdConfirmHelp.color = Color.white;
    //    m_nickNameHelp.color = Color.white;
    //    m_emailHelp.color = Color.white;
    //    m_phoneNumberHelp.color = Color.white;
    //    m_idHelp.text = "아이디는 8~14자리까지 입력가능합니다.";
    //    m_pwdHelp.text = "비밀번호는 8~14자리까지 입력가능합니다.";
    //    m_pwdConfirmHelp.text = "보안을 위해 비밀번호를 한번 더 입력해주세요.";
    //    m_nickNameHelp.text = "사용하실 닉네임을 입력하세요.";
    //    m_emailHelp.text = "이메일을 입력해주세요.";
    //    m_phoneNumberHelp.text = "연락 가능한 번호를 입력해주세요.";

    //    m_publishText.text = "* 필수입력사항";
    //    m_privacyText.text = "* 개인정보구역";
    //    m_privacyText2.text = "해당구역은 입력하지 않아도 무관하나, 고객센터 문의 및 경품수령시 불이익을 받을 수 있습니다.";
    //}

    public void Open()
    {
        m_elapsedTime = 0;
        m_prevAngle = m_camera.eulerAngles;
        m_prevPos = m_camera.position;

        // ResetText();
        m_accountUseGuid.SetActive(false);
        // m_createField.SetActive(false);
        gameObject.SetActive(true);

        StartCoroutine(FadeUI(null, m_privacyPolicy));

        m_id.text = null;
        m_pwd.text = null;
        m_pwdConfirm.text = null;
        m_nickName.text = null;
        m_email.text = null;
        m_phoneNumber.text = null;
    }

    public void Close()
    {
        CreateCharacter.Close();
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if (m_elapsedTime < 1)
        {
            m_elapsedTime += Time.deltaTime * 0.5f;
            m_camera.position = Vector3.Lerp(m_prevPos, m_targetPos, m_elapsedTime);
            m_camera.eulerAngles = Vector3.Lerp(m_prevAngle, m_targetAngle, m_elapsedTime);
        }
    }

    public IEnumerator FadeUI(GameObject before, GameObject after)
    {
        WaitForSeconds wait = new WaitForSeconds(0.033f);

        if (before != null)
        {
            for (int i = 0; i < 15; ++i)
            {
                m_bgMask.color = Color.Lerp(Color.clear, Color.black, i / 14f);
                yield return wait;
            }
            before.SetActive(false);
        }

        if (after != null)
        {
            after.SetActive(true);

            for (int i = 0; i < 15; ++i)
            {
                m_bgMask.color = Color.Lerp(Color.black, Color.clear, i / 14f);
                yield return wait;
            }
        }

        yield return null;
    }
}
