using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAction : MonoBehaviour
{
    new Transform transform;
    private void Awake()
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
    public IEnumerator JoinAction_TwilightDesert_Town()
    {
        yield return null;
        transform.position = new Vector3(1.13f, 2.11f, -8.56f);
        transform.rotation = Quaternion.Euler(new Vector3(-102.7f, 180, 0));
        yield return StartCoroutine(RotateCamera(Quaternion.Euler(new Vector3(-3, 180, 0)), 1));
        StartCoroutine(MoveCamera(new Vector3(1.13f, 2.66f, -7.03f), 0.5f));
        yield return StartCoroutine(RotateCamera(Quaternion.Euler(new Vector3(13.519f, 180, 0)), 0.5f));
        StartCoroutine(MoveCamera(new Vector3(0.34f, 3.44f, -3.27f), 1.5f));
        yield return StartCoroutine(RotateCamera(Quaternion.Euler(new Vector3(7.491f, 342.437f, 0)), 1.5f));
        Transform playerCamera = CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera.transform;
        StartCoroutine(MoveCamera(playerCamera.position, 1));
        yield return StartCoroutine(RotateCamera(playerCamera.rotation, 1.5f));
    }
}