using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필수!
// using System.IO; // (나중에 세이브/로드 시 파일 입출력을 위해 필요)

public class GameManager : MonoBehaviour
{
    // 싱글톤(Singleton) 패턴: 게임 내내 딱 하나만 존재하도록 만듦
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // --- 싱글톤 설정 ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 다른 씬으로 넘어가도 이 GameManager는 파괴되지 않음!
        }
        else
        {
            Destroy(gameObject); // 이미 GameManager가 있다면 이 오브젝트는 파괴
        }
        // ---------------------
    }

    // --- 1. 씬 전환 기능 (INT 방식) ---
    // 씬의 '빌드 인덱스(숫자)'를 받아서 해당 씬을 불러오는 함수
    public void LoadScene(int sceneIndex)
    {
        // (나중에 여기에 로딩 화면(UI) 켜는 코드를 넣을 수 있음)
        SceneManager.LoadScene(sceneIndex);
    }

    // (예시) 나중에 UI 버튼에 이 함수들을 바로 연결할 수 있습니다.

    // 'Gyeongseong_Hub' 씬 (빌드 인덱스 0번으로 가정)
    public void LoadHub()
    {
        LoadScene(0);
    }

    // 'Stage_1_KimMaria' 씬 (빌드 인덱스 1번으로 가정)
    public void LoadStage1()
    {
        LoadScene(1);
    }

    // 'Stage_2_KimIkSang' 씬 (빌드 인덱스 2번으로 가정)
    public void LoadStage2()
    {
        LoadScene(2);
    }

    // 'Stage_3_KangWooKyu' 씬 (빌드 인덱스 3번으로 가정)
    public void LoadStage3()
    {
        LoadScene(3);
    }


    // --- 2. 세이브/로드 기초 (Week 1에서는 비워둠) ---

    public void SaveGame()
    {
        // (Week 2-3) 나중에 여기에 플레이어 위치, 굶주림, 신뢰도 등을
        // PlayerPrefs나 JSON 파일로 저장하는 코드를 작성합니다.
        Debug.Log("게임 저장 시도 (아직 구현 안 됨)");
    }

    public void LoadGame()
    {
        // (Week 2-3) 나중에 여기에서 저장된 데이터를 불러오는 코드를 작성합니다.
        Debug.Log("게임 불러오기 시도 (아직 구현 안 됨)");
    }
}