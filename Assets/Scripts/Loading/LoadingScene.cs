using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadingScene : MonoBehaviour
{
    static public string nextScene;

    public Slider progressBar;

    public static void LoadScene( string sceneName )
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private void Start()
    {
        if (null == progressBar)
        {
            Debug.LogError("LoadingScene.cs / progressBar is null");
            return;
        }

        StartCoroutine(LoadSceneProcess());
    }
    
    IEnumerator LoadSceneProcess()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);

        // 모든 로딩이 끝나면 자동으로 넘어가는것인가? (false 90% 정도에서 멈춤)
        // AssetBundle 로딩이 따로 필요한경우
        // 씬 화면에서 팁을 보여줄때 읽기도 전에 사라지는 경우를 막기 위함
        operation.allowSceneActivation = false;

        float timer = 0.0f;

        while (false == operation.isDone)
        {
            yield return null;

            // 90% 미만일때.
            if (operation.progress < 0.9f)
            {
                progressBar.value = operation.progress;
            }
            else    // 90% 이상일때 fake loading
            {
                timer += Time.unscaledDeltaTime;
                progressBar.value = Mathf.Lerp(0.9f, 1.0f, timer);

                if (1.0f <= progressBar.value)
                {
                    operation.allowSceneActivation = true;
                    yield return new WaitForSeconds(1.0f);
                    yield break;
                }
            }
        }
    }



}
