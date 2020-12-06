using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAction : MonoBehaviour
{
    float m_minSpeed, m_maxSpeed, m_lerpSpeed, m_frameSpeed;
    new Transform transform;
    void StartLerpSpeed(float min, float max, float speedPerSecond)
    {
        m_lerpSpeed = 0;
        m_minSpeed = min;
        m_maxSpeed = max;
        m_frameSpeed = speedPerSecond;
    }
    void Awake()
    {
        transform = GetComponent<Transform>();
    }
    IEnumerator MoveCamera(Vector3 target, float time)
    {
        Vector3 curr = transform.position;
        float percent = 0;
        do
        {
            yield return null;
            float delta = Time.deltaTime / time;
            percent += delta;
            transform.position = Vector3.Lerp(curr, target, percent);
        } while (percent <= 1);
    }
    IEnumerator MoveCameraLerpSpeed(Vector3 target, float time)
    {
        Vector3 curr = transform.position;
        float percent = 0;
        do
        {
            yield return null;
            m_lerpSpeed = Mathf.Clamp(m_lerpSpeed + m_frameSpeed * Time.deltaTime, m_minSpeed, m_maxSpeed);
            float delta = Time.deltaTime / time * m_lerpSpeed;
            percent += delta;
            transform.position = Vector3.Lerp(curr, target, percent);
        } while (percent <= 1);
    }
    IEnumerator RotateCamera(Quaternion target, float time)
    {
        Quaternion curr = transform.rotation;
        float percent = 0;
        do
        {
            yield return null;
            float delta = Time.deltaTime / time;
            percent += delta;
            transform.rotation = Quaternion.Lerp(curr, target, percent);
        } while (percent <= 1);
    }
    IEnumerator RotateSinCamera(Quaternion target, float time)
    {
        yield return null;
    }
    public IEnumerator JoinAction_TwilightDesert_Town()
    {
        yield return null;
        transform.position = new Vector3(1.13f, 2.11f, -8.56f);
        transform.rotation = Quaternion.Euler(new Vector3(-102.7f, 180, 0));

        yield return StartCoroutine(RotateCamera(Quaternion.Euler(new Vector3(-3, 180, 0)), 1));

        Vector3 targetPos = new Vector3(1.13f, 2.66f, -7.03f);
        float time = Vector3.Distance(transform.position, targetPos) * 0.5f;
        StartCoroutine(RotateCamera(Quaternion.Euler(new Vector3(13.519f, 180, 0)), time*0.5f));
        yield return StartCoroutine(MoveCamera(targetPos, time));

        targetPos = new Vector3(0.34f, 3.44f, -3.27f);
        time = Vector3.Distance(transform.position, targetPos) * 0.5f;
        StartCoroutine(RotateCamera(Quaternion.Euler(new Vector3(7.491f, 342.437f, 0)), time*0.5f));
        yield return StartCoroutine(MoveCamera(targetPos, time));

        Transform playerCamera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera.transform;
        targetPos = playerCamera.position;
        time = Vector3.Distance(transform.position, targetPos) * 0.5f;
        StartCoroutine(RotateCamera(playerCamera.rotation, time * 0.5f));
        yield return StartCoroutine(MoveCamera(playerCamera.position, time));
    }
    public IEnumerator JoinAction_EmpireOfEstela_DazeForest()
    {
        yield return null;
        transform.position = new Vector3(-15.4f, 30f, 20.2f);
        transform.rotation = Quaternion.Euler(new Vector3(78.6f, 151.6f, 0));
        StartLerpSpeed(0.5f, 10, 0.5f);

        Vector3 targetPos = new Vector3(-14.4f, 19.1f, 18.3f);
        float time = Vector3.Distance(transform.position, targetPos);
        StartCoroutine(RotateCamera(Quaternion.Euler(new Vector3(66.66f, 151.6f, 0)), time * 0.25f));
        yield return StartCoroutine(MoveCameraLerpSpeed(targetPos, time));

        targetPos = new Vector3(-10.62f, 14.23f, 12f);
        time = Vector3.Distance(transform.position, targetPos);
        StartCoroutine(RotateCamera(Quaternion.Euler(new Vector3(2.734f, 101.061f, -8.058f)), time * 0.5f));
        yield return StartCoroutine(MoveCameraLerpSpeed(targetPos, time));

        targetPos = new Vector3(9.61f, 10.66f, 6.95f);
        time = Vector3.Distance(transform.position, targetPos);
        StartCoroutine(RotateCamera(Quaternion.Euler(new Vector3(19.36f, 159f, 0)), time * 0.5f));
        yield return StartCoroutine(MoveCameraLerpSpeed(targetPos, time));

        targetPos = new Vector3(21.64f, 5.35f, -16.76f);
        time = Vector3.Distance(transform.position, targetPos);
        StartCoroutine(RotateCamera(Quaternion.Euler(new Vector3(39.558f, 163f, 0)), time * 0.25f));
        yield return StartCoroutine(MoveCameraLerpSpeed(targetPos, time));
    }
}