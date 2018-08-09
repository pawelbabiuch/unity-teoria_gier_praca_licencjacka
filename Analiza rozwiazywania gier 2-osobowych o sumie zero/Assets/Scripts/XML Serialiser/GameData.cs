using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

/// <summary>
/// Informacje dotyczące rozgrywki
/// </summary>
[System.Serializable]
public class GameData
{
    /// <summary>
    /// Nazwa gracza wierszowego
    /// </summary>
    public string rowPlayer { get; set; }
    /// <summary>
    /// Nazwa gracza kolumnowego
    /// </summary>
    public string columnPlayer { get; set; }
    /// <summary>
    /// Nazwy strategii dla gracza wierszowego
    /// </summary>
    public List<string> rowStrategies { get; set; }
    /// <summary>
    /// Nazwy strategii dla gracza kolumnowego
    /// </summary>
    public List<string> columnStrategies { get; set; }
    /// <summary>
    /// Względne częstotliwości Gracza wierszowego
    /// </summary>
    [XmlIgnore]
    public int[] rowFreq { get; set; }
    /// <summary>
    /// Względne częstotliwości Gracza kolumnowego
    /// </summary>
    [XmlIgnore]
    public int[] colFreq { get; set; }

    /// <summary>
    /// Macierz wypłat aktualnej gry
    /// </summary>
    [XmlIgnore]
    public sbyte[,] matrix { get; set; }

    private GameData() { }


    public GameData(string rowPlayer, string columnPlayer, byte rowStrategies, byte columnStrategies)
    {

        this.rowStrategies = new List<string>();
        this.columnStrategies = new List<string>();
        this.rowPlayer = rowPlayer;
        this.columnPlayer = columnPlayer;
        this.matrix = new sbyte[rowStrategies, columnStrategies];

        string strName;
        for (int i = 0; i < rowStrategies; i++)
        {
            strName = ((char)(65 + i)).ToString();
            this.rowStrategies.Add(strName);
        }

        for (int i = 0; i < columnStrategies; i++)
        {
            strName = ((char)(65 + i)).ToString();
            this.columnStrategies.Add(strName);
        }
    }

    public GameData(GameData oldgD, sbyte[,] matrix)
    {
        this.rowStrategies = oldgD.rowStrategies;
        this.columnStrategies = oldgD.columnStrategies;
        this.rowPlayer = oldgD.rowPlayer;
        this.columnPlayer = oldgD.columnPlayer;
        this.matrix = matrix;
    }

    /// <summary>
    /// XML nie serializuje tablic dwuwymiarowych, dlatego należy je rzutować na tablice rozszarpane
    /// </summary>
    [XmlElement("matrix")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sbyte[][] XmlMatrix
    {
        get
        {
            sbyte[][] m = new sbyte[matrix.GetLength(0)][];

            for (int i = 0; i < m.Length; i++)
            {
                m[i] = new sbyte[matrix.GetLength(1)];

                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    m[i][j] = matrix[i, j];
                }
            }

            return m;
        }

        private set
        {
            matrix = new sbyte[value.Length,value[0].Length];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = value[i][j];
                }
            }
        }
    }
}
