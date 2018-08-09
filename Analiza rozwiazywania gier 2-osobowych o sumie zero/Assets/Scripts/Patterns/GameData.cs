using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Scripts
{
    public class GameData
    {
        public static GameData instance;

        /*Nazwy graczy*/
        public string playerA { get; private set; }
        public string playerB { get; private set; }

        /*Nazwy strategii*/
        public List<char> rowStrategies { get; set; }
        public List<char> colStrategies { get; set; }

        /*Względne częstotliwości*/
        [XmlIgnore]
        public int[] rowFreq { get; set; }
        [XmlIgnore]
        public int[] colFreq { get; set; }

        /*Macierz wypłat*/
        [XmlIgnore] public int[,] matrix { get; set; }

        private GameData()
        {
            instance = this;
        }

        public GameData(ScriptableGameData gameData)
        {
            this.playerA = gameData.rowPlayerName;
            this.playerB = gameData.columnPlayerName;
            this.matrix = gameData.getMatrix;
            this.rowStrategies = SetUpStrategies(matrix.GetLength(0));
            this.colStrategies = SetUpStrategies(matrix.GetLength(1));

            instance = this;        
        }

        public GameData(int[,] matrix, string playerA = "Gracz Wierszowy", string playerB = "Gracz Kolumnowy")
        {
            this.matrix = matrix;
            this.playerA = playerA;
            this.playerB = playerB;
            this.rowStrategies = SetUpStrategies(matrix.GetLength(0));
            this.colStrategies = SetUpStrategies(matrix.GetLength(1));

            instance = this;
        }

        [XmlElement("matrix")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int[][] XmlMatrix
        {
            get
            {
                int[][] m = new int[matrix.GetLength(0)][];

                for (int i = 0; i < m.Length; i++)
                {
                    m[i] = new int[matrix.GetLength(1)];

                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        m[i][j] = matrix[i, j];
                    }
                }

                return m;
            }

            private set
            {
                matrix = new int[value.Length, value[0].Length];

                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        matrix[i, j] = value[i][j];
                    }
                }
            }
        }

        private List<char> SetUpStrategies(int length)
        {
            const int FIRST_CHAR_LETTER_A = 65;

            List<char> chars = new List<char>();

            for (int i = 0; i < length; i++)
            {
                chars.Add((char)(FIRST_CHAR_LETTER_A + i));
            }

            return chars;
        }
    }
}
