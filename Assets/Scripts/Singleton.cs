using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // T 클래스의 현재 인스턴스를 저장
    private static T instance;

    // 앱이 현재 닫히고 있는 중인지를 알기 위해서 사용
    private static bool appIsClosing = false;

    public static T Instance
    {
        get
        {
            // 앱이 닫히는 중이라면...?
            if (true == appIsClosing)
            {
                return null;
            }

            // instance가 할당되지 않았다면?
            if (null == instance)
            {
                // 씬에 이미 인스턴스가 존재하는지 확인
                instance = (T)FindObjectOfType(typeof(T));

                // 검색결과 후에도 존재하지 않는다면? 새로 만든다.
                if (null == instance)
                {
                    // 새로운 게임 오브젝트를 생성
                    GameObject newGameObject = new GameObject();

                    // 이것에 T 컴포넌트를 추가
                    instance = newGameObject.AddComponent<T>();

                    // 이것의 이름을 T 클래스 이름으로 바꾼다.
                    newGameObject.name = typeof(T).ToString();
                }

                // 이것을 DontDestroyOnLoad로 표시한다.
                DontDestroyOnLoad(instance);
            }

            // 마지막으로 instance를 반환
            return instance;
        }
    }

    private void Start()
    {
        // 씬 안의 모든 싱글톤의 인스턴스를 얻는다.
        T[] allInstances = FindObjectsOfType(typeof(T)) as T[];

        // 하나 이상 인스턴스가 존재한다면?
        if (allInstances.Length > 1)
        {
            // 발견된 인스턴스에 각각에 대해
            foreach (T instanceToCheck in allInstances)
            {
                // 발견된 인스턴스가 현재 인스턴스가 아니라면?
                if (instanceToCheck != Instance)
                {
                    // 파괴한다.
                    Destroy(instanceToCheck.gameObject);
                }
            }
        }
        // 존재하는 인스턴스를 DontDestroyOnLoad로 표시한다.
        DontDestroyOnLoad((T)FindObjectOfType(typeof(T)));
    }

    private void OnApplicationQuit()
    {
        // appIsClosing을 true로 설정한다.
        appIsClosing = true;
    }
}
