using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableManager : MonoBehaviour
{
    public GameObject fieldStrategyPrefab, fieldValuePrefab;

    [HideInInspector]
    public CanvasGroup cG;

    [Header("Row Player")]
    public Text txtRowPlayer;
    public Transform rowPlayerStrategies;
    private GridLayoutGroup rowGLG;

    [Header("Column Player")]
    public Text txtColumnPlayer;
    public Transform columnPlayerStrategies;
    private GridLayoutGroup columnGLG;

    [Header("Main Table")]
    public Transform mainTable;
    private GridLayoutGroup tableGLG;
    private List<Image> selectedValue;

    public static TableManager ins;

    private void Awake()
    {
        ins = this;
    }

    private void Start()
    {
        cG = GetComponent<CanvasGroup>();

        rowGLG = rowPlayerStrategies.GetComponent<GridLayoutGroup>();
        columnGLG = columnPlayerStrategies.GetComponent<GridLayoutGroup>();
        tableGLG = mainTable.GetComponent<GridLayoutGroup>();

        selectedValue = new List<Image>();
    }

    
    /// <summary>
    /// Funkcja rysuje na scenie tablicę gry
    /// </summary>
    public  void DisplayTable(GameData gD)
    {
        ClearTable();
        SetUp(gD);
        InsertValues(gD);
    }

    public void DisplayTable(Scripts.GameData gD)
    {
        ClearTable();
        SetUp(gD);
        InsertValues(gD);

    }

    public void UpdateTable(Scripts.GameData gD)
    {
        int id = 0;
        for (int i = 0; i < gD.matrix.GetLength(0); i++)
        {
            for (int j = 0; j < gD.matrix.GetLength(1); j++)
            {
                InputField field = mainTable.GetChild(id).GetComponent<InputField>();
                int oldValue = int.Parse(field.text);
                int newValue = gD.matrix[i, j];
                
                if(oldValue != newValue)
                {
                    gD.matrix[i, j] = oldValue;
                }

                id++;
            }
        }
    }

    public void SelectColumn(byte colCount, byte colID, EColors eColor = EColors.Default, bool unselect = false)
    {
        if (unselect) Unselect();

        Image img;
        for (byte i = 0; i < mainTable.childCount; i++)
        {
            if (i % colCount == colID)
            {
                img = mainTable.GetChild(i).GetComponent<Image>();
                img.color = Colors.getColor(eColor);
                selectedValue.Add(img);
            }
        }
    }

    public void SelectRow(byte rowCount, byte rowID, EColors eColor = EColors.Default, bool unselect = false)
    {
        if (unselect) Unselect();

        byte childInRow = (byte)(mainTable.childCount / rowCount);
        byte startID = (byte)(childInRow * rowID);

        Image img;
        for (byte i = startID; i < startID + childInRow; i++)
        {
            img = mainTable.GetChild(i).GetComponent<Image>();
            img.color = Colors.getColor(eColor);
            selectedValue.Add(img);
        }
    }

    public void SelectCell(byte cellID, EColors eColor = EColors.Default, bool unselect = false)
    {
        if (unselect) Unselect();

        Image img = mainTable.GetChild(cellID).GetComponent<Image>();
        img.color = Colors.getColor(eColor);
        selectedValue.Add(img);
    }

    public void SelectInSelected(byte id, EColors eColor = EColors.Default)
    {
        selectedValue[id].color = Colors.getColor(eColor);
    }

    public void Unselect()
    {
        for (byte i = 0; i < selectedValue.Count; i++)
        {
            if(selectedValue[i] != null)
                selectedValue[i].color = Color.white;
        }

        selectedValue.Clear();
    }

    private void ClearTable()
    {
        foreach (Transform child in rowPlayerStrategies)
            Destroy(child.gameObject);

        foreach (Transform child in columnPlayerStrategies)
            Destroy(child.gameObject);

        foreach (Transform child in mainTable)
            Destroy(child.gameObject);
    }

    public const int CELL_SIZE = 40;
    private void SetUp(GameData gD)
    {
        txtRowPlayer.text = gD.rowPlayer;
        txtColumnPlayer.text = gD.columnPlayer;

        byte rows = (byte)gD.rowStrategies.Count;
        byte cols = (byte)gD.columnStrategies.Count;

        float height = mainTable.GetComponent<RectTransform>().rect.height / rows;
        float width = mainTable.GetComponent<RectTransform>().rect.width  / cols;

        rowGLG.cellSize = new Vector2(CELL_SIZE, height);
        columnGLG.cellSize = new Vector2(width, CELL_SIZE);
        tableGLG.cellSize = new Vector2(width, height);
    }

    private void SetUp(Scripts.GameData gD)
    {
        txtRowPlayer.text = gD.playerA;
        txtColumnPlayer.text = gD.playerB;

        byte rows = (byte)gD.matrix.GetLength(0);
        byte cols = (byte)gD.matrix.GetLength(1);

        float height = mainTable.GetComponent<RectTransform>().rect.height / rows;
        float width = mainTable.GetComponent<RectTransform>().rect.width / cols;

        rowGLG.cellSize = new Vector2(CELL_SIZE, height);
        columnGLG.cellSize = new Vector2(width, CELL_SIZE);
        tableGLG.cellSize = new Vector2(width, height);
    }

    private void InsertValues(GameData gD)
    {
        GameObject newGO;

        for (int i = 0; i < gD.rowStrategies.Count; i++)
        {
            newGO = Instantiate(fieldStrategyPrefab, rowPlayerStrategies);
            newGO.GetComponent<InputField>().text = gD.rowStrategies[i].ToString();
        }

        for (int i = 0; i < gD.columnStrategies.Count; i++)
        {
            newGO = Instantiate(fieldStrategyPrefab, columnPlayerStrategies);
            newGO.GetComponent<InputField>().text = gD.columnStrategies[i].ToString();
        }

        int id = 0;
        for (int i = 0; i < gD.rowStrategies.Count; i++)
        {
            for (int j = 0; j < gD.columnStrategies.Count; j++)
            {
                newGO = Instantiate(fieldValuePrefab, mainTable);
                newGO.name = "Test " + id++;

                if (gD.matrix.GetLength(0) > i && gD.matrix.GetLength(1) > j)
                    newGO.GetComponent<InputField>().text = gD.matrix[i, j].ToString();
            }
        }
    }

    private void InsertValues(Scripts.GameData gD)
    {
        GameObject newGO;

        for (int i = 0; i < gD.matrix.GetLength(0); i++)
        {
            newGO = Instantiate(fieldStrategyPrefab, rowPlayerStrategies);
            newGO.GetComponent<InputField>().text = gD.rowStrategies[i].ToString();
        }

        for (int i = 0; i < gD.matrix.GetLength(1); i++)
        {
            newGO = Instantiate(fieldStrategyPrefab, columnPlayerStrategies);
            newGO.GetComponent<InputField>().text = gD.colStrategies[i].ToString();
        }

        int id = 0;
        for (int i = 0; i < gD.matrix.GetLength(0); i++)
        {
            for (int j = 0; j < gD.matrix.GetLength(1); j++)
            {
                newGO = Instantiate(fieldValuePrefab, mainTable);
                newGO.name = "Value_" + id++;

                if (gD.matrix.GetLength(0) > i && gD.matrix.GetLength(1) > j)
                    newGO.GetComponent<InputField>().text = gD.matrix[i, j].ToString();
            }
        }
    }
}
