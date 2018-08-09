using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data", order = 1)]
public class ScriptableGameData : ScriptableObject
{
    // Nazwy graczy:
    public string rowPlayerName, columnPlayerName;

    // Macierz wypłat:
    [Tooltip("Liczba wierszy")]
    [SerializeField]
    private ArrayMatrix[] matrix;

    public int[,] getMatrix
    {
        get
        {
            int[,] newMatrix = new int[matrix.Length, matrix[0].matrix.Length];

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].matrix.Length; j++)
                {
                    newMatrix[i, j] = matrix[i].matrix[j];
                }
            }

            return newMatrix;
        }
    }
}

[System.Serializable]
public class ArrayMatrix
{
    [Tooltip("Liczba kolumn")]
    public int[] matrix;
}
