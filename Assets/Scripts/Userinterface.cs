using System.Diagnostics;
using OriginLabs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Userinterface : MonoBehaviour
{
    [SerializeField] public CanvasManager m_CanvasManager;

    // Singleton
    public static Userinterface Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start(){
        Invoke("mainstart", 0.25f);
    }

    void mainstart(){
        m_CanvasManager.SwitchCanvas(MenuType.MainMenu);
    }

    void Update()
    {
        if (m_CanvasManager != null)
        {
            // Check if the active canvas is LevelSelect or Teleporting
            Player_Runtime.IsInMenu = 
                m_CanvasManager.activeCanvasType == MenuType.LevelSelect ||
                m_CanvasManager.activeCanvasType == MenuType.Teleporting;
        }
        else
        {
            UnityEngine.Debug.LogWarning("CanvasManager is not assigned in Userinterface.");
        }
    if (Input.GetKeyDown(KeyCode.Escape) && Player_Runtime.IsInSafeArea)
    {
        // Check the current active canvas type
        if (m_CanvasManager.activeCanvasType == MenuType.LevelSelect)
        {
            // If LevelSelect is open, switch to GameUI
            m_CanvasManager.SwitchCanvas(MenuType.GameUI);
        }
        else
        {
            // Otherwise, open LevelSelect
            m_CanvasManager.SwitchCanvas(MenuType.LevelSelect);
        }
    }
    }

    public void OpenUI(MenuType type)
    {
        if (m_CanvasManager != null)
        {
            m_CanvasManager.SwitchCanvas(type);
        }
        else
        {
            UnityEngine.Debug.LogError("CanvasManager is not assigned in Userinterface.");
        }
    }
}
