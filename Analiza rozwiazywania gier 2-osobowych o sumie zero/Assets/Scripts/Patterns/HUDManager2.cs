using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts;

public class HUDManager2 : MonoBehaviour
{

    #region Inspector variables
    [Header("Górny panel")]
    [SerializeField]
    private Button btnGameSelect;

    [Header("Dolny panel")]
    [SerializeField]
    private Slider slidgerSpeed;
    [SerializeField]
    private GameObject gOBtnStart, gOBtnPause, gOBtnEnd;
    [SerializeField]
    private GameObject gOTextSpace;
    [SerializeField]
    private Toggle autoModeToggle;

    [Header("Kreator gry")]
    [SerializeField]
    private InputField rowPlayerField;
    [SerializeField]
    private InputField colPlayerField;
    [SerializeField]
    private Text rowPlayerStrategiesTxt;
    [SerializeField]
    private Text colPlayerStrategiesTxt;

    [Header("Panele")]
    [SerializeField]
    private GameObject createGamePanel;
    [SerializeField]
    private GameObject mainTable;
    [SerializeField]
    private CanvasGroup frequences;
    [SerializeField]
    private CanvasGroup graphicalMethod;
    #endregion

    #region Variables
    private bool paused = false;
    private static HUDManager2 instance;

    private const byte MIN_STRATEGIES = 2;
    private const byte MAX_STRATEGIES = 5;

    private GameAnalysis gameAnalysis;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        slidger_ChangeSpeed();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SetPause();
    }

    public void btn_LoadGame(string xmlFileName)
    {
        LogsManager.ins.ClearLogs();
        LogsManager.ins.AddLog("Gra została załadowana.");
        Scripts.GameData gD = XMLManager.ins.LoadData2(xmlFileName);
        gameAnalysis = new GameAnalysis(gD);
    }

    public void btn_LoadGame(ScriptableGameData gameData)
    {
        LogsManager.ins.ClearLogs();
        LogsManager.ins.AddLog("Gra została załadowana.");
        Scripts.GameData gD = new Scripts.GameData(gameData);
        gameAnalysis = new GameAnalysis(gD);
    }

    public static void StopAnalysing()
    {
        instance.gOBtnPause.SetActive(false);
        instance.gOBtnEnd.SetActive(true);
    }

    public void btn_OpenUserDocs()
    {
        string fileDir = string.Format(@"{0}/StreamingAssets/PDF/{1}.pdf", Application.dataPath, "UserDocs");
        System.Diagnostics.Process.Start(fileDir);
    }

    public void btn_RestartApplication(int sceneID)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneID);
    }

    public void btn_CreateGame()
    {
        LogsManager.ins.ClearLogs();
        LogsManager.ins.AddLog("Nowa gra została utworzona.");

        string rowName = rowPlayerField.text;
        string colName = colPlayerField.text;

        byte rowStrategiesCount = byte.Parse(rowPlayerStrategiesTxt.text);
        byte colStrategiesCount = byte.Parse(colPlayerStrategiesTxt.text);

        int[,] matrix = GetRandomMatrix(rowStrategiesCount, colStrategiesCount);

        gameAnalysis = new GameAnalysis(new Scripts.GameData(matrix, rowName, colName));
    }

    public void btn_IncreaseStrategies(Text changeText)
    {
        byte val = (byte)(byte.Parse(changeText.text) + 1);
        byte clamped = (byte)Mathf.Clamp(val, MIN_STRATEGIES, MAX_STRATEGIES);

        changeText.text = clamped.ToString();
    }

    public void btn_DecreaseStrategies(Text changeText)
    {
        byte val = (byte)(byte.Parse(changeText.text) - 1);
        byte clamped = (byte)Mathf.Clamp(val, MIN_STRATEGIES, MAX_STRATEGIES);

        changeText.text = clamped.ToString();
    }

    public void btn_StartAnalysis()
    {
        try
        {
            if (gameAnalysis == null) throw new System.Exception("Brak utworzonej gry do analizy.");

            gOBtnStart.SetActive(false);
            gOBtnPause.SetActive(true);
            createGamePanel.SetActive(false);
            mainTable.GetComponent<CanvasGroup>().blocksRaycasts = false;
            frequences.alpha = 1;
            graphicalMethod.alpha = 1;
            btnGameSelect.interactable = false;

            StartCoroutine(gameAnalysis.StartGameAnalysis());

        }
        catch (System.Exception e)
        {
            LogsManager.ins.AddLog(e.Message, EColors.Yellow);
        }
    }

    public void btn_ExitApplication()
    {
        Application.Quit();
    }

    public void slidger_ChangeSpeed()
    {
        GameTheory.Game.speed = slidgerSpeed.value;
        GameTheory.Game.wfs = new WaitForSeconds(slidgerSpeed.value);
    }

    private int[,] GetRandomMatrix(byte x, byte y)
    {
        int[,] matrix = new int[x, y];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                matrix[i, j] = Random.Range(-9, 10);
            }
        }

        return matrix;
    }

    private void Pause()
    {
        paused = true;
        gOTextSpace.SetActive(true);
        Time.timeScale = 0;
    }

    private void Play()
    {
        paused = false;
        gOTextSpace.SetActive(false);
        Time.timeScale = 1;
    }

    public static void SetPause(bool setPause)
    {
        if (setPause)
        {
            instance.Pause();
        }
        else
        {
            instance.Play();
        }
    }

    public void SetPause()
    {
        SetPause(!paused);
    }

    public static bool isAutoMode
    {
        get { return instance.autoModeToggle.isOn; }
    }

    public static float gameSpeed
    {
        get
        {
            return instance.slidgerSpeed.value;
        }
    }
}
