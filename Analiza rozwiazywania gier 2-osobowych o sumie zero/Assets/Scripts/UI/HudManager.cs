using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
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

    private bool paused = false;
    private bool overwriteGameData { get; set; }
    private static HudManager ins;

    private const byte MIN_STRATEGIES = 2;
    private const byte MAX_STRATEGIES = 5;

    private GameTheory.Game game;
    private IEnumerator executive;

    private void Awake()
    {
        ins = this;
    }

    private void Start()
    {
        ChangeSpeed();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && (!isAutoMode || Time.timeScale == 0 ))
            PausePlay();
    }

    public void PausePlay()
    {
        paused = !paused;

        if (paused)
        {
            Time.timeScale = 0;
            gOTextSpace.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            gOTextSpace.SetActive(false);
        }
    }

    public static void SetPause()
    {
        ins.paused = false;
        ins.PausePlay();
    }

    public void StartAnalysis()
    {
        try
        {
            if (game != null && !overwriteGameData)
            {
                gOBtnStart.SetActive(false);
                gOBtnPause.SetActive(true);
                createGamePanel.SetActive(false);
                mainTable.GetComponent<CanvasGroup>().blocksRaycasts = false;

                frequences.alpha = 1;
                graphicalMethod.alpha = 1;
                btnGameSelect.interactable = false;

                executive = game.Executive();
                StartCoroutine(executive);
            }
            else if (overwriteGameData)
            {
                sbyte[,] matrix = new sbyte[GameTheory.Game.gameData.rowStrategies.Count, GameTheory.Game.gameData.columnStrategies.Count];

                byte id = 0;
                for (byte i = 0; i < matrix.GetLength(0); i++)
                {
                    for (byte j = 0; j < matrix.GetLength(1); j++)
                    {
                        string value = TableManager.ins.mainTable.GetChild(id).GetComponent<InputField>().text;

                        if (string.IsNullOrEmpty(value))
                            throw new FormatException("Pusta wartość w tabeli wypłat");
                        else
                        {
                            matrix[i, j] = sbyte.Parse(value);
                            id++;
                        }
                    }
                }

                GameData gD = new GameData(GameTheory.Game.gameData, matrix);
                game = new GameTheory.Game(gD);

                overwriteGameData = false;
                StartAnalysis();
            }
            else
            {
                LogsManager.ins.AddLog("Nie można rozpocząc pustej gry", EColors.Red);
            }
        }
        catch(Exception e)
        {
            LogsManager.ins.AddLog(e.Message, EColors.Red);
        }

    }

    public void EndAnalysis()
    {
        gOBtnEnd.SetActive(false);
        gOBtnStart.SetActive(true);
        createGamePanel.SetActive(true);
        mainTable.GetComponent<CanvasGroup>().blocksRaycasts = true;

        LogsManager.ins.ClearLogs();
        GraphicalManager.ins.Disable();
        FrequencyManager.ins.Clear();
        TableManager.ins.Unselect();
        MiniMaksManager.ins.Disable();
        SumManager.ins.EnableDisableView(false);

        btnGameSelect.interactable = true;
        overwriteGameData = true;

        if (executive != null)
        {
            StopCoroutine(executive);
            executive = null;
        }

    }

    public void CreateGame()
    {
        LogsManager.ins.ClearLogs();
        EndAnalysis();

        string rowPlayerName = rowPlayerField.text;
        string colPlayerName = colPlayerField.text;

        byte rowStrategiesCount = byte.Parse(rowPlayerStrategiesTxt.text);
        byte colStrategiesCount = byte.Parse(colPlayerStrategiesTxt.text);

        GameData gD = new GameData(rowPlayerName, colPlayerName, rowStrategiesCount, colStrategiesCount);
        game = new GameTheory.Game(gD);
    }

    public void IncreaseStrategies(Text changeText)
    {
        byte val = (byte)(byte.Parse(changeText.text) + 1);
        byte clamped = (byte)Mathf.Clamp(val, MIN_STRATEGIES, MAX_STRATEGIES);

        changeText.text = clamped.ToString();
    }

    public void DecreaseStrategies(Text changeText)
    {
        byte val = (byte)(byte.Parse(changeText.text) - 1);
        byte clamped = (byte)Mathf.Clamp(val, MIN_STRATEGIES, MAX_STRATEGIES);

        changeText.text = clamped.ToString();
    }

    public void ChangeSpeed()
    {
        GameTheory.Game.speed = slidgerSpeed.value;
        GameTheory.Game.wfs = new WaitForSeconds(slidgerSpeed.value);
    }

    public void LoadGame(string xmlFileName)
    {
        LogsManager.ins.ClearLogs();
        game = new GameTheory.Game(XMLManager.ins.LoadData(xmlFileName));
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void RestartApplication()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public static bool isAutoMode
    {
        get { return ins.autoModeToggle.isOn; }
    }

    public static void StopAnalysing()
    {
        ins.gOBtnPause.SetActive(false);
        ins.gOBtnEnd.SetActive(true);
    }
}
