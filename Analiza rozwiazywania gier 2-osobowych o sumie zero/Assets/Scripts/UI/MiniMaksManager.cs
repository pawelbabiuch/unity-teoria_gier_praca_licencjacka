using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMaksManager : MonoBehaviour
{
    public static MiniMaksManager ins;

    public GameObject valFieldPrefab;
    public Transform minsList, maxsList;

    private GridLayoutGroup minGrid, maxGrid;
    private List<Image> selectedMins = new List<Image>();
    private List<Image> selectedMax = new List<Image>();

    private void Awake()
    {
        ins = this;
    }

    private void Start()
    {
        minGrid = minsList.GetComponent<GridLayoutGroup>();
        maxGrid = maxsList.GetComponent<GridLayoutGroup>();

        Disable();
    }

    public void SetUp(GameData gameData)
    {

        minsList.parent.gameObject.SetActive(true);
        maxsList.parent.gameObject.SetActive(true);

        byte rows = (byte)gameData.rowStrategies.Count;
        byte cols = (byte)gameData.columnStrategies.Count;

        float heighRow = minsList.GetComponent<RectTransform>().rect.height / rows;
        float widthCol = maxGrid.GetComponent<RectTransform>().rect.width / cols;

        minGrid.cellSize = new Vector2(90, heighRow);
        maxGrid.cellSize = new Vector2(widthCol, 90);

        InputField inputField;
        for (int i = 0; i < gameData.matrix.GetLength(0); i++)
            inputField = Instantiate(valFieldPrefab, minsList).GetComponent<InputField>();

        for (int i = 0; i < gameData.matrix.GetLength(1); i++)
            inputField = Instantiate(valFieldPrefab, maxsList).GetComponent<InputField>();
    }

    public void SetUp(Scripts.GameData gD)
    {
        minsList.parent.gameObject.SetActive(true);
        maxsList.parent.gameObject.SetActive(true);

        byte rows = (byte)gD.matrix.GetLength(0);
        byte cols = (byte)gD.matrix.GetLength(1);

        float heighRow = minsList.GetComponent<RectTransform>().rect.height / rows;
        float widthCol = maxGrid.GetComponent<RectTransform>().rect.width / cols;

        minGrid.cellSize = new Vector2(90, heighRow);
        maxGrid.cellSize = new Vector2(widthCol, 90);

        InputField inputField;
        for (byte i = 0; i < rows; i++)
            inputField = Instantiate(valFieldPrefab, minsList).GetComponent<InputField>();

        for (byte i = 0; i < cols; i++)
            inputField = Instantiate(valFieldPrefab, maxsList).GetComponent<InputField>();
    }

    public void SelectMaks(byte iD, EColors eColor = EColors.Red, bool unseelect = false)
    {
        if (unseelect) UnselectMaks();

        Select(maxsList, selectedMax, iD, eColor);
    }

    public void SelectMin(byte iD, EColors eColor = EColors.Red, bool unseelect = false)
    {
        if (unseelect) UnselectMins();

        Select(minsList, selectedMins, iD, eColor);
    }

    private void Select(Transform parent, List<Image> list, byte iD, EColors eColor = EColors.Red)
    {

        Image img = parent.GetChild(iD).GetComponent<Image>();
        img.color = Colors.getColor(eColor);

        list.Add(img);
    }

    private void UnselectMins()
    {
        foreach (Image img in selectedMins)
        {
            img.color = Colors.getColor(EColors.Default);
        }

        selectedMins.Clear();
    }

    private void UnselectMaks()
    {
        foreach (Image img in selectedMax)
        {
            img.color = Colors.getColor(EColors.Default);
        }

        selectedMax.Clear();
    }

    public void ChangeMaxVal(byte maxID, sbyte val)
    {
        ChangeValue(maxsList, maxID, val);
    }

    public void ChangeMinVal(byte minID, sbyte val)
    {
        ChangeValue(minsList, minID, val);
    }

    private void ChangeValue(Transform list, byte iD, sbyte val)
    {
        list.GetChild(iD).GetComponent<InputField>().text = val.ToString();
    }

    public void Disable()
    {
        minsList.parent.gameObject.SetActive(false);
        maxsList.parent.gameObject.SetActive(false);

        if (selectedMax.Count > 0)
        {
            SelectMaks(0, EColors.Default, true);
            selectedMax.Clear();
        }

        if (selectedMins.Count > 0)
        {
            SelectMin(0, EColors.Default, true);
            selectedMins.Clear();
        }

        foreach (Transform child in minsList)
            Destroy(child.gameObject);

        foreach (Transform child in maxsList)
            Destroy(child.gameObject);
    }
}
