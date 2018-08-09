using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SumManager : MonoBehaviour
{
    public static SumManager ins;
    public GameObject valFieldPrefab;

    public Transform columnList, rowList;
    private GridLayoutGroup colGrid, rowGrid;

    private void Awake()
    {
        ins = this;
    }

    private void Start()
    {
        colGrid = columnList.GetComponent<GridLayoutGroup>();
        rowGrid = rowList.GetComponent<GridLayoutGroup>();

        columnList.parent.gameObject.SetActive(false);
        rowList.parent.gameObject.SetActive(false);
    }

    public void SetUp(GameData gameData)
    {
        EnableDisableView(true);

        Rect rowRect = rowList.GetComponent<RectTransform>().rect;
        Rect colRect = columnList.GetComponent<RectTransform>().rect;

        float heightRow = rowRect.height / gameData.matrix.GetLength(0);
        float widthCol = colRect.width / gameData.matrix.GetLength(1);

        rowGrid.cellSize = new Vector2(rowRect.width, heightRow);
        colGrid.cellSize = new Vector2(widthCol, colRect.height);

        InputField inputField;

        for (int i = 0; i < gameData.matrix.GetLength(0); i++)
        {
            inputField = Instantiate(valFieldPrefab, rowList).GetComponent<InputField>();
            inputField.text = "0";
        }

        for (int i = 0; i < gameData.matrix.GetLength(1); i++)
        {
            inputField = Instantiate(valFieldPrefab, columnList).GetComponent<InputField>();
            inputField.text = "0";
        }
    }

    public void SetUp(Scripts.GameData gameData)
    {
        EnableDisableView(true);

        Rect rowRect = rowList.GetComponent<RectTransform>().rect;
        Rect colRect = columnList.GetComponent<RectTransform>().rect;

        float heightRow = rowRect.height / gameData.matrix.GetLength(0);
        float widthCol = colRect.width / gameData.matrix.GetLength(1);

        rowGrid.cellSize = new Vector2(rowRect.width, heightRow);
        colGrid.cellSize = new Vector2(widthCol, colRect.height);

        InputField inputField;

        for (int i = 0; i < gameData.matrix.GetLength(0); i++)
        {
            inputField = Instantiate(valFieldPrefab, rowList).GetComponent<InputField>();
            inputField.text = "0";
        }

        for (int i = 0; i < gameData.matrix.GetLength(1); i++)
        {
            inputField = Instantiate(valFieldPrefab, columnList).GetComponent<InputField>();
            inputField.text = "0";
        }
    }

    public void EnableDisableView(bool enable)
    {
        columnList.parent.gameObject.SetActive(enable);
        rowList.parent.gameObject.SetActive(enable);
    }

    public void ChangeRowSm(int[] values)
    {
        ChangeSum(rowList, values);
    }

    public void ChangeColSum(int[] values)
    {
        ChangeSum(columnList, values);
    }

    private void ChangeSum(Transform list, int[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            list.GetChild(i).GetComponent<InputField>().text = values[i].ToString();
        }
    }

    public void SelectRow(byte iD)
    {
        Select(columnList, iD);
    }

    public void SelectColumn(byte iD)
    {
        Select(rowList, iD);
    }

    private void Select(Transform list, byte iD)
    {
        UnselectAll();
        list.GetChild(iD).GetComponent<Image>().color = Colors.getColor(EColors.Red);
    }

    public void UnselectAll()
    {
        for (byte i = 0; i < rowList.childCount; i++)
            rowList.GetChild(i).GetComponent<Image>().color = Colors.getColor(EColors.Default);

        for (byte i = 0; i < columnList.childCount; i++)
            columnList.GetChild(i).GetComponent<Image>().color = Colors.getColor(EColors.Default);
    }

}
