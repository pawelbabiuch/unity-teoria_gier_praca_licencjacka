using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrequencyManager : MonoBehaviour
{
    #region Isnpector variables
    public GameObject freqFieldPrefab;
    public GameObject strategyFieldPrefab;
    public CanvasGroup cG;

    [Header("Gracz Wierszowy")]
    public Text txtRowPlayerName;
    public Transform rowPlayerList;
    public Transform rowStrategiesList;
    private GridLayoutGroup rowGridLayout;
    private GridLayoutGroup rowStrategiesGridLayout;

    [Header("Gracz Kolumnowy")]
    public Text txtColumnPlayerName;
    public Transform colPlayerList;
    public Transform colStrategiesList;
    private GridLayoutGroup colGridLayout;
    private GridLayoutGroup colStrategiesLayoutGrid;
    #endregion
    public static FrequencyManager ins;

    private void Awake()
    {
        ins = this;
    }

    private void Start()
    {
        cG.alpha = 0;

        rowGridLayout = rowPlayerList.GetComponent<GridLayoutGroup>();
        colGridLayout = colPlayerList.GetComponent<GridLayoutGroup>();

        rowStrategiesGridLayout = rowStrategiesList.GetComponent<GridLayoutGroup>();
        colStrategiesLayoutGrid = colStrategiesList.GetComponent<GridLayoutGroup>();
    }

    /// <summary>
    /// Ustawia domyślnie interfejs dla względnych częśtotliwości
    /// </summary>
    /// <param name="gameData"></param>
    public void SetUp(GameData gameData)
    {
        txtRowPlayerName.text = gameData.rowPlayer;
        txtColumnPlayerName.text = gameData.columnPlayer;

        float widthRow = rowPlayerList.GetComponent<RectTransform>().rect.width / gameData.matrix.GetLength(0);
        float widthCol = colPlayerList.GetComponent<RectTransform>().rect.width / gameData.matrix.GetLength(1);

        rowGridLayout.cellSize = new Vector2(widthRow, 60);
        colGridLayout.cellSize = new Vector2(widthCol, 60);

        rowStrategiesGridLayout.cellSize = new Vector2(widthRow, 40);
        colStrategiesLayoutGrid.cellSize = new Vector2(widthCol, 40);

        InputField inputField;

        for (int i = 0; i < gameData.matrix.GetLength(0); i++)
        {
            // Wypłata:
            inputField = Instantiate(freqFieldPrefab, rowPlayerList).GetComponent<InputField>();
            inputField.text = "0";

            // Strategia:
            inputField = Instantiate(strategyFieldPrefab, rowStrategiesList).GetComponent<InputField>();
            inputField.text = gameData.rowStrategies[i];

        }

        for (int i = 0; i < gameData.matrix.GetLength(1); i++)
        {
            //Wypłata
            inputField = Instantiate(freqFieldPrefab, colPlayerList).GetComponent<InputField>();
            inputField.text = "0";

            //Strategia:
            inputField = Instantiate(strategyFieldPrefab, colStrategiesList).GetComponent<InputField>();
            inputField.text = gameData.columnStrategies[i];
        }

        cG.alpha = 1;
    }

    public void SetUp(Scripts.GameData gameData)
    {
        txtRowPlayerName.text = gameData.playerA;
        txtColumnPlayerName.text = gameData.playerB;

        float widthRow = rowPlayerList.GetComponent<RectTransform>().rect.width / gameData.matrix.GetLength(0);
        float widthCol = colPlayerList.GetComponent<RectTransform>().rect.width / gameData.matrix.GetLength(1);

        rowGridLayout.cellSize = new Vector2(widthRow, 60);
        colGridLayout.cellSize = new Vector2(widthCol, 60);

        rowStrategiesGridLayout.cellSize = new Vector2(widthRow, 40);
        colStrategiesLayoutGrid.cellSize = new Vector2(widthCol, 40);

        InputField inputField;

        for (int i = 0; i < gameData.matrix.GetLength(0); i++)
        {
            // Wypłata:
            inputField = Instantiate(freqFieldPrefab, rowPlayerList).GetComponent<InputField>();
            inputField.text = "0";

            // Strategia:
            inputField = Instantiate(strategyFieldPrefab, rowStrategiesList).GetComponent<InputField>();
            inputField.text = gameData.rowStrategies[i].ToString();

        }

        for (int i = 0; i < gameData.matrix.GetLength(1); i++)
        {
            //Wypłata
            inputField = Instantiate(freqFieldPrefab, colPlayerList).GetComponent<InputField>();
            inputField.text = "0";

            //Strategia:
            inputField = Instantiate(strategyFieldPrefab, colStrategiesList).GetComponent<InputField>();
            inputField.text = gameData.colStrategies[i].ToString();
        }

        cG.alpha = 1;
    }


    /// <summary>
    /// Zmienia wartość danej częstotliwości Gracza Wierszowego
    /// </summary>
    /// <param name="rowID">która częstotlwiość</param>
    /// <param name="val">jaka wartość</param>
    public void ChangeRowFreq(sbyte rowID, int val)
    {
        ChangeFreq(rowPlayerList, rowID, val);
    }

    public void Clear()
    {
        foreach (Transform child in rowStrategiesList)
            Destroy(child.gameObject);

        foreach (Transform child in colStrategiesList)
            Destroy(child.gameObject);

        foreach (Transform child in rowPlayerList)
            Destroy(child.gameObject);

        foreach (Transform child in colPlayerList)
            Destroy(child.gameObject);

        cG.alpha = 0;
    }

    /// <summary>
    /// Zmienia wartość danej częstotliwości Gracza Kolumnowego
    /// </summary>
    /// <param name="colID">która częstotliwość</param>
    /// <param name="val">jaka wartość</param>
    public void ChangeColFreq(sbyte colID, int val)
    {
        ChangeFreq(colPlayerList, colID, val);
    }

    private void ChangeFreq(Transform list, sbyte id, int val)
    {
        list.GetChild(id).GetComponent<InputField>().text = val.ToString();
    }

}
