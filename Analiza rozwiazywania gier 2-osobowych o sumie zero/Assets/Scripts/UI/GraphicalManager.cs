using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.UI.Extensions;
using GameTheory;

public class GraphicalManager : MonoBehaviour
{
    public static GraphicalManager ins;

    public GameObject textValuePrefab;
    public GameObject linePrefab;

    public Transform leftValues, rightValues, valuesList;

    [HideInInspector]
    public CanvasGroup cG;
    private int[,] matrix;

    private List<UILineRenderer> lines = new List<UILineRenderer>();
    public UILineRenderer highLine;

    [SerializeField]
    private RectTransform selectedPoint;

    private void Awake()
    {
        ins = this;
    }

    private void Start()
    {
        cG = GetComponent<CanvasGroup>();
        cG.alpha = 0;
    }

    public IEnumerator SetUp(sbyte[,] matrix)
    {
        //cG.alpha = 1;
        //this.matrix = matrix;
        yield return null;
        int[] strategyA = (matrix.GetLength(0) == 2) ? GetRow(0) : GetColumn(0);
        int[] strategyB = (matrix.GetLength(0) == 2) ? GetRow(1) : GetColumn(1);

        #region Wyszukiwanie najmniejszej i największej wartości
        int min, max;

        int minA = strategyA.Min();
        int minB = strategyB.Min();

        int maxA = strategyA.Max();
        int maxB = strategyB.Max();

        min = (minA < minB) ? minA : minB;
        max = (maxA > maxB) ? maxA : maxB;
        #endregion

        //LogsManager.ins.AddLog("Trwa wypisywanie wartości dla gracza, który posiada dokładnie dwie strategie.");
        yield return Game.Wait();

        WriteValues(min, max);

        yield return WriteLines(strategyA, strategyB);
    }

    public IEnumerator SetUp(int[,] matrix)
    {
        //cG.alpha = 1;
        this.matrix = matrix;
        yield return null;
        int[] strategyA = (matrix.GetLength(0) == 2) ? GetRow(0) : GetColumn(0);
        int[] strategyB = (matrix.GetLength(0) == 2) ? GetRow(1) : GetColumn(1);

        #region Wyszukiwanie najmniejszej i największej wartości
        int min, max;

        int minA = strategyA.Min();
        int minB = strategyB.Min();

        int maxA = strategyA.Max();
        int maxB = strategyB.Max();

        min = (minA < minB) ? minA : minB;
        max = (maxA > maxB) ? maxA : maxB;
        #endregion

        LogsManager.ins.AddLog("Trwa wypisywanie wartości dla gracza, który posiada dokładnie dwie strategie.");
        yield return LogsManager.Wait();
        WriteValues(min, max);

        yield return WriteLines(strategyA, strategyB);
    }

    private void WriteValues(int minVal, int maxVal)
    {

        for (int i = maxVal; i >= minVal; i--)
        {
            Text txt = Instantiate(textValuePrefab, leftValues).GetComponent<Text>();
            Text txt2 = Instantiate(textValuePrefab, rightValues).GetComponent<Text>();


            txt2.text = string.Format(".{0}", i);
            txt2.alignment = TextAnchor.MiddleLeft;
            txt.text = string.Format("{0}.", i);
        }
    }

    private IEnumerator WriteLines(int[] strA, int[] strB)
    {
        
        LogsManager.ins.AddLog("Trwa wypisywanie strategii drugiego gracza w postaci prostych.");
        yield return LogsManager.Wait();
        Rect rect = valuesList.GetComponent<RectTransform>().rect;

        int valA, valB;
        float height = rect.height / leftValues.childCount;
        float width = rect.width;

        float halfOnce = height / 2;

        for (int i = 0; i < strA.Length; i++)
        {
            valA = strA[i];
            valB = strB[i];

            UILineRenderer lR = Instantiate(linePrefab, valuesList).GetComponent<UILineRenderer>();

            lR.Points[0] = new Vector2(0, rect.height - (GetChildID(valA) * height) - halfOnce);
            lR.Points[1] = new Vector2(width, rect.height - (GetChildID(valB) * height) - halfOnce);

            lines.Add(lR);

            LogsManager.ins.AddLog("Wypisano strategię.");
            yield return LogsManager.Wait();
        }
    }

    private int GetChildID(int childValue)
    {
        int childID = 0;

        for (; childID < leftValues.childCount; childID++)
        {
            int value = int.Parse(leftValues.GetChild(childID).GetComponent<Text>().text.TrimEnd('.'));
            if (value == childValue) break;
        }

        return childID;
    }

    public void SelectLine(byte lineID, EColors eColor = EColors.Default, bool unselect = false)
    {
        if (unselect) Unselect();
        lines[lineID].color = Colors.getColor(eColor);
    }

    public void SetHighLinePoint(Vector2 point)
    {
        List<Vector2> newPoints;
        Rect rect = valuesList.GetComponent<RectTransform>().rect;


        if (highLine.Points.Length == 0) newPoints = new List<Vector2>();
        else newPoints = ((Vector2[])highLine.Points.Clone()).ToList();

        float width = rect.width;
        float height = rect.height / leftValues.childCount;

        float y = CalculateY(point, rect.height, height);

        Vector2 newPoint = new Vector2(point.y * width, y);

        if (newPoints.Count > 1)
        {
            if (!selectedPoint.gameObject.activeInHierarchy)
                selectedPoint.gameObject.SetActive(true);

            selectedPoint.anchoredPosition = newPoints[newPoints.Count - 1];
        }
        else
        {
            selectedPoint.gameObject.SetActive(false);
        }

        newPoints.Add(newPoint);
        highLine.Points = newPoints.ToArray();

        highLine.transform.SetAsLastSibling();
        selectedPoint.SetAsLastSibling();
    }

    private float CalculateY(Vector2 point, float rectHeight, float pointHeight)
    {
        int y = Mathf.RoundToInt(point.x);
        float rest = y - point.x;
        int childID = GetChildID(y);
        float calculate = rectHeight - (childID * pointHeight) - (rest * pointHeight) - (pointHeight /2);

        return calculate;
    }

    public void Disable()
    {
        foreach (Transform child in leftValues)
            Destroy(child.gameObject);

        foreach (Transform child in rightValues)
            Destroy(child.gameObject);

        highLine.Points = new Vector2[0];
        selectedPoint.gameObject.SetActive(false);

        while (lines.Count > 0)
        {
            Destroy(lines[0].gameObject);
            lines.RemoveAt(0);
        }

        matrix = null;
        lines.Clear();
        cG.alpha = 0;
    }

    private void Unselect()
    {
        foreach (UILineRenderer line in lines)
        {
            line.color = Colors.getColor(EColors.Default);
        }
    }

    /// <summary>
    /// Funkcja zwraca cały wiersz Gracza Wierszowego dla wybranej strategii
    /// </summary>
    /// <param name="rowID">ID strategii do przepisania</param>
    /// <returns>Przepisana strategia</returns>
    private int[] GetRow(byte rowID)
    {
        int[] row = new int[matrix.GetLength(1)];

        for (int i = 0; i < row.Length; i++)
        {
            row[i] = matrix[rowID, i];
        }

        return row;
    }

    /// <summary>
    /// Funkcja zwraca całą kolumnę Gracza Kolumnowego dla wybranej strategii
    /// </summary>
    /// <param name="colID">ID strategii do przepisania</param>
    /// <returns>Przepisana strategia</returns>
    private int[] GetColumn(byte colID)
    {
        int[] col = new int[matrix.GetLength(0)];

        for (int i = 0; i < col.Length; i++)
        {
            col[i] = matrix[i, colID];
        }

        return col;
    }
}
