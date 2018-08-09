using GameTheory;
using Scripts.States.Solver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Scripts
{
    public class Game
    {
        private StateSolver stateSolver;
        public GameData gameData;

        public Game(GameData gameData)
        {
            this.gameData = gameData;
        }

        private int minMax = 0, maxMin = 0;
        public IEnumerator CheckSaddlePoint()
        {
            LogsManager.ins.AddLog("Sprawdzanie punktów siodłowych.");
            MiniMaksManager.ins.SetUp(gameData);
            yield return LogsManager.Wait();
            yield return FindMinMax();
            yield return FindMaxMin();

            if (minMax == maxMin)
            {
                LogsManager.ins.AddLog("Znaleziono punkt siodłowy.", EColors.Green);
                stateSolver = new StateSaddlePoint(minMax);
                yield return LogsManager.Wait();
            }
            else
            {
                LogsManager.ins.AddLog("Brak punktów siodłowych.", EColors.Red);
                stateSolver = new StateNoSaddlePoint();
                yield return LogsManager.Wait();
            }
        }

        public IEnumerator CheckDominations()
        {
            if (stateSolver.GetType() == typeof(StateNoSaddlePoint))
            {
                LogsManager.ins.AddLog("Sprawdzanie strategii zdominowanych.");
                yield return RemoveDominatedStrategies();
            }
            else
            {
                yield return LogsManager.Wait();
            }
        }

        public IEnumerator SolveGame()
        {
            yield return stateSolver.SolveGame(this);
        }

        private IEnumerator FindMinMax()
        {
            yield return LogsManager.Wait();
            sbyte max = sbyte.MinValue;
            sbyte minInRow;
            sbyte[] row;

            for (byte i = 0; i < gameData.matrix.GetLength(0); i++)
            {
                row = GetRow(i);
                TableManager.ins.SelectRow((byte)gameData.matrix.GetLength(0), i, EColors.Green, true);
                minInRow = row.Min();

                MiniMaksManager.ins.ChangeMinVal(i, minInRow);
                LogsManager.ins.AddLog(string.Format("Znaleziono najmniejszą wartość w strategii {0} gracza: {1}", gameData.rowStrategies[i].ToString(), gameData.playerA));
                yield return LogsManager.Wait();

                if (minInRow > max)
                {
                    max = minInRow;

                    LogsManager.ins.AddLog("Znaleziony min wiersza jest największy.");
                    MiniMaksManager.ins.SelectMin(i, EColors.Red, true);
                    yield return LogsManager.Wait();
                }
                else if (minInRow == max)
                {
                    LogsManager.ins.AddLog("Znaleziono kolejny min wiersza");
                    MiniMaksManager.ins.SelectMin(i, EColors.Red);
                    yield return LogsManager.Wait();
                }
                else
                {
                    LogsManager.ins.AddLog("Znaleziona strategia nie jest największa spośród minimów");
                    yield return LogsManager.Wait();
                }
            }

            minMax = max;
        }

        private IEnumerator FindMaxMin()
        {
            yield return LogsManager.Wait();
            sbyte min = sbyte.MaxValue;
            sbyte maxInColumn;
            sbyte[] column;

            for (byte i = 0; i < gameData.matrix.GetLength(1); i++)
            {
                column = GetColumn(i);
                TableManager.ins.SelectColumn((byte)gameData.matrix.GetLength(1), i, EColors.Green, true);
                maxInColumn = column.Max();

                MiniMaksManager.ins.ChangeMaxVal(i, maxInColumn);
                LogsManager.ins.AddLog(string.Format("Znaleziono największą wartość w strategii {0} gracza: {1}", gameData.colStrategies[i].ToString(), gameData.playerB));
                yield return LogsManager.Wait();

                if (maxInColumn < min)
                {
                    min = maxInColumn;
                    LogsManager.ins.AddLog("Znaleziony maks kolumny jest najmniejszy.");
                    MiniMaksManager.ins.SelectMaks(i, EColors.Red, true);
                    yield return LogsManager.Wait();
                }
                else if (maxInColumn == min)
                {
                    LogsManager.ins.AddLog("Znaleziono kolejny maks kolumny");
                    MiniMaksManager.ins.SelectMaks(i, EColors.Red);
                    yield return LogsManager.Wait();
                }
                else
                {
                    LogsManager.ins.AddLog("Znaleziona strategia nie jest najmniejsza spośród maksimów");
                    yield return LogsManager.Wait();
                }
            }
            maxMin = min;
        }

        private IEnumerator RemoveDominatedStrategies()
        {
            yield return LogsManager.Wait();
            byte oldCols; // ile wierszy i kolumn posiada macierz przed dominacją

            do
            {
                oldCols = (byte)gameData.matrix.GetLength(1);

                yield return RowsDomination();
                yield return ColumnsDomination();

            } while (oldCols != gameData.matrix.GetLength(1));
        }

        private IEnumerator RowsDomination()
        {
            sbyte[] rowA, rowB;
            yield return LogsManager.Wait();

            for (byte i = 0; i < gameData.matrix.GetLength(0) - 1; i++)
            {
                rowA = GetRow(i);

                for (byte j = (byte)(i + 1); j < gameData.matrix.GetLength(0); j++)
                {
                    LogsManager.ins.AddLog(string.Format("Porównuję strategię gracza: {2} - {0} z {1}", gameData.rowStrategies[i].ToString(), gameData.rowStrategies[j].ToString(), gameData.playerA));
                    rowB = GetRow(j);

                    TableManager.ins.SelectRow((byte)gameData.matrix.GetLength(0), i, EColors.Green, true);
                    yield return LogsManager.Wait();
                    TableManager.ins.SelectRow((byte)gameData.matrix.GetLength(0), j, EColors.Blue);
                    yield return LogsManager.Wait();

                    // -1 = rowA  dominuje rowB; 1 = rowB dominuje rowA; 0 = brak dominacji
                    sbyte domination = Compare(rowA, rowB);

                    if (domination == -1)
                    {
                        LogsManager.ins.AddLog(string.Format("Strategia {0} jest dominowana przez {1}", gameData.rowStrategies[j].ToString(), gameData.rowStrategies[i].ToString()));
                        TableManager.ins.SelectRow((byte)gameData.matrix.GetLength(0), j, EColors.Red, true);
                        yield return LogsManager.Wait();

                        gameData.matrix = RemoveRowFromMatrix(j--);
                        TableManager.ins.DisplayTable(gameData);
                        yield return LogsManager.Wait();
                    }
                    else if (domination == 1)
                    {
                        LogsManager.ins.AddLog(string.Format("Strategia {0} dominuje strategię {1}", gameData.rowStrategies[j].ToString(), gameData.rowStrategies[i].ToString()));

                        TableManager.ins.SelectRow((byte)gameData.matrix.GetLength(0), i, EColors.Red, true);
                        yield return LogsManager.Wait();

                        gameData.matrix = RemoveRowFromMatrix(i--);
                        TableManager.ins.DisplayTable(gameData);
                        yield return LogsManager.Wait();
                        break;
                    }
                    else
                    {
                        LogsManager.ins.AddLog("Brak dominacji pomiędzy strategiami.");
                        TableManager.ins.SelectCell(0, EColors.Default, true);
                        yield return LogsManager.Wait();
                    }
                }
            }

            yield return null;
        }

        private IEnumerator ColumnsDomination()
        {
            sbyte[] colA, colB;
            yield return LogsManager.Wait();

            for (byte i = 0; i < gameData.matrix.GetLength(1) - 1; i++)
            {

                colA = GetColumn(i);

                for (byte j = (byte)(i + 1); j < gameData.matrix.GetLength(1); j++)
                {
                    colB = GetColumn(j);
                    LogsManager.ins.AddLog(string.Format("Porównuję strategię gracza: {2} - {0} z {1}", gameData.colStrategies[i].ToString(), gameData.colStrategies[j].ToString(), gameData.playerB));


                    TableManager.ins.SelectColumn((byte)gameData.matrix.GetLength(1), i, EColors.Green, true);
                    yield return LogsManager.Wait();
                    TableManager.ins.SelectColumn((byte)gameData.matrix.GetLength(1), j, EColors.Blue);
                    yield return LogsManager.Wait();

                    // 1 = colA dominuje colB; 1 = colB dominuje colA; 0 = brak dominacji
                    sbyte domination = Compare(colA, colB);

                    if (domination == 1)
                    {
                        LogsManager.ins.AddLog(string.Format("Strategia {0} jest dominowana przez {1}", gameData.colStrategies[j].ToString(), gameData.colStrategies[i].ToString()));
                        TableManager.ins.SelectColumn((byte)gameData.matrix.GetLength(1), j, EColors.Red, true);
                        yield return LogsManager.Wait();

                        gameData.matrix = RemoveColumnFromMatrix(j--);
                        TableManager.ins.DisplayTable(gameData);
                        yield return LogsManager.Wait();
                    }
                    else if (domination == -1)
                    {
                        LogsManager.ins.AddLog(string.Format("Strategia {0} dominuje strategię {1}", gameData.colStrategies[j].ToString(), gameData.colStrategies[i]).ToString());
                        TableManager.ins.SelectColumn((byte)gameData.matrix.GetLength(1), i, EColors.Red, true);
                        yield return LogsManager.Wait();

                        gameData.matrix = RemoveColumnFromMatrix(i--);
                        TableManager.ins.DisplayTable(gameData);
                        yield return LogsManager.Wait();

                        break;
                    }
                    else
                    {
                        TableManager.ins.SelectCell(0, EColors.Default, true);
                        LogsManager.ins.AddLog("Brak dominacji pomiędzy strategiami");
                        yield return LogsManager.Wait();
                    }
                }
            }

            yield return null;
        }

        private int[,] RemoveColumnFromMatrix(byte columnID)
        {
            int[,] newMatrix = new int[gameData.matrix.GetLength(0), gameData.matrix.GetLength(1) - 1];

            for (byte y = 0, ny = 0; y < gameData.matrix.GetLength(1); y++, ny++)
            {
                if (columnID == y)
                {
                    ny--;
                    continue;
                }

                for (byte x = 0; x < gameData.matrix.GetLength(0); x++) newMatrix[x, ny] = gameData.matrix[x, y];
            }

            gameData.colStrategies.RemoveAt(columnID);
            return newMatrix;
        }

        private int[,] RemoveRowFromMatrix(byte rowID)
        {
            int[,] newMatrix = new int[gameData.matrix.GetLength(0) - 1, gameData.matrix.GetLength(1)];

            for (byte x = 0, nx = 0; x < gameData.matrix.GetLength(0); x++, nx++)
            {
                if (rowID == x)
                {
                    nx--;
                    continue;
                }

                for (byte y = 0; y < gameData.matrix.GetLength(1); y++) newMatrix[nx, y] = gameData.matrix[x, y];
            }

            gameData.rowStrategies.RemoveAt(rowID);
            return newMatrix;
        }

        private sbyte Compare(sbyte[] A, sbyte[] B)
        {
            sbyte s = 0;

            byte less = 0, more = 0, equals = 0;

            for (byte i = 0; i < A.Length; i++)
            {
                if (A[i] < B[i]) less++;
                else if (A[i] > B[i]) more++;
                else equals++;
            }

            if (less + equals == A.Length) s = 1;
            else if (more + equals == A.Length) s = -1;

            return s;
        }

        public sbyte[] GetRow(byte rowID)
        {
            sbyte[] row = new sbyte[gameData.matrix.GetLength(1)];

            for (byte i = 0; i < row.Length; i++)
                row[i] = (sbyte)gameData.matrix[rowID, i];

            return row;
        }

        public sbyte[] GetColumn(byte columnID)
        {
            sbyte[] column = new sbyte[gameData.matrix.GetLength(0)];

            for (byte i = 0; i < column.Length; i++)
                column[i] = (sbyte)gameData.matrix[i, columnID];

            return column;
        }

        public static int[] DivisorRelativeRequences(int[] freq)
        {
            int maxDivisor = GreatestCommonDivisor.GetGreatestCommonDivisor(freq);

            if (maxDivisor > 1)
            {
                for (int i = 0; i < freq.Length; i++)
                {
                    freq[i] /= maxDivisor;
                }
            }

            return freq;
        }
    }
}
