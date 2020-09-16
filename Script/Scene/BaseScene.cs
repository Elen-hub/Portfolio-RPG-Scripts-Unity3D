using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseScene : MonoBehaviour {

    // 동기형 호출방식
    protected void LoadScene (EScene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    // 비동기형 호출방식
    protected AsyncOperation LoadSceneAsync (EScene scene)
    {
        return SceneManager.LoadSceneAsync(scene.ToString());
    }

    private IEnumerator IELoadStatusCheck(AsyncOperation async, System.Action<float> progress, System.Action after)
    {
        // 신을 로딩중일때 해야할 일이 있다면 progress함수를 호출 할 수 있도록 합니다.
        if (progress != null)
            progress(async.progress);

        // 신의 로딩이 끝나기 전까지 점유를 풀어 줄 수 있도록 합니다.
        while (!async.isDone)
            yield return null;

        // 로딩을 완료한 후 해야할 일이 있다면 after함수를 호출시킬 수 있도록 합니다.
        if (after != null)
            after();

        yield return null;
    }

    protected void LoadSceneAsync(EScene scene, System.Action<float> progress, System.Action after)
    {
        AsyncOperation async = LoadSceneAsync(scene);

        StartCoroutine(IELoadStatusCheck(async, progress, after));
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }
}
