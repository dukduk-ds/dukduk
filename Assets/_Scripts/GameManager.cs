using UnityEngine;
using UnityEngine.SceneManagement;
// using System.IO; 

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
            // 다른 씬으로 넘어가도 이 GameManager는 파괴되지 않음
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 GameManager가 있다면 이 오브젝트는 파괴
            Destroy(gameObject);
        }
        // ---------------------
    }

    // --- 1. 씬 전환 기능 (INT 방식) ---
    // 씬의 '빌드 인덱스(숫자)'를 받아서 해당 씬을 불러오는 함수
    public void LoadScene(int sceneIndex)
    {
        // 시간을 복구하고 씬 전환을 시작합니다.
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneIndex);
    }

    // 현재 씬을 다시 시작 (UI 버튼 연결용)
    public void RestartCurrentScene()
    {
        // 1. TimeScale 복구 (0으로 멈춰있는 경우 대비)
        Time.timeScale = 1f;

        // 2. 현재 활성화된 씬을 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- 2. 게임 상태 제어 기능 (AI, Objective 스크립트에서 호출) ---

    /// <summary>
    /// 게임 오버(Bad Ending) 상태를 처리합니다.
    /// </summary>
    /// <param name="gameOverUIPanel">활성화할 게임 오버 UI 패널</param>
    public void GameOver(GameObject gameOverUIPanel)
    {
        Debug.Log("🚨 게임 오버 발생! - 시간이 멈춥니다.");
        if (gameOverUIPanel != null)
        {
            gameOverUIPanel.SetActive(true);
        }
        Time.timeScale = 0f; // 게임 시간 정지
    }

    /// <summary>
    /// 미션 성공(Good Ending) 상태를 처리합니다.
    /// </summary>
    /// <param name="successUIPanel">활성화할 미션 성공 UI 패널</param>
    public void MissionSuccess(GameObject successUIPanel)
    {
        Debug.Log("🎉 미션 성공! - 시간이 멈춥니다.");
        if (successUIPanel != null)
        {
            successUIPanel.SetActive(true);
        }
        Time.timeScale = 0f; // 게임 시간 정지
    }


    // --- 3. UI 버튼 연결용 기능 ---

    // 예시: 허브 씬 (빌드 인덱스 0번으로 가정)으로 로드
    public void LoadHub()
    {
        LoadScene(0);
    }

    // 게임 종료 (PC 플랫폼용)
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // --- 4. 세이브/로드 기초 (필요시 사용) ---
    public void SaveGame()
    {
        Debug.Log("게임 저장 시도 (아직 구현 안 됨)");
    }

    public void LoadGame()
    {
        Debug.Log("게임 불러오기 시도 (아직 구현 안 됨)");
    }
}