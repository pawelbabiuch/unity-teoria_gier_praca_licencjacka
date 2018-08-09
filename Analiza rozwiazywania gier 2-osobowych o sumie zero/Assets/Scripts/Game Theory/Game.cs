using System.IO;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

namespace GameTheory
{
    /// <summary>
    /// Klasa odpowiedzialna za analizę rozgrywki
    /// </summary>
    public sealed class Game
    {
        /// <summary>
        /// Data gry
        /// </summary>
        public static GameData gameData;

        /// <summary>
        /// Początkowa macierz wypłat
        /// </summary>
        public sbyte[,] startMatrix { get; private set; }
        /// <summary>
        /// WaitForSeconds określa odstępy czasowe pomiędzy krokami w analizie gry
        /// </summary>
        public static WaitForSeconds wfs { get; set; }
        public static float speed = .5f;

        private sbyte minMax = 0, maxMin = 0;
        private bool hasSaddlePoint = false;
        private float valueOfGameSaddle = 0;

        /// <summary>
        /// Konstruktor inicjalizujący klasę na podstawie zapisanego pliku
        /// </summary>
        /// <param name="fileName">plik który ma zostać odczytany (bez foramtu)</param>
        public Game(GameData gD)
        {
            gameData = gD;
            this.startMatrix = (sbyte[,])gameData.matrix.Clone();

            LogsManager.ins.AddLog("Gra została załadowana");
            TableManager.ins.DisplayTable(gameData);
        }


        /// <summary>
        /// Metoda wykonawcza, obliczająca strategie dla graczy
        /// </summary>
        /// <param name="checkSaddlePoint">Czy sprawdzić występowanie punktu siodłowego</param>
        /// <param name="checkDomination">Czy sprawdzić występowanie strategii zdominowanych</param>
        public IEnumerator Executive(bool checkSaddlePoint = true, bool checkDomination = true)
        {
            TableManager.ins.DisplayTable(gameData);
            TableManager.ins.cG.blocksRaycasts = false;

            LogsManager.ins.AddLog("Trwa analiza gry...");
            //Debug.Log("Trwa analiza gry.");
            yield return Wait();
            byte saddlePointCount = 0;

            if (checkSaddlePoint)
            {
                yield return HasSaddlePoint();
                yield return Wait();
            }

            if (checkSaddlePoint && hasSaddlePoint)
            {
                sbyte minInRow, maxInCol;
                byte cellID = 0;

                LogsManager.ins.AddLog("Analiza wykazała istnienie punktów siodłowych.", EColors.Green);
                yield return MinMaxPanel.ins.SetUp(true);

                for (byte i = 0; i < gameData.matrix.GetLength(0); i++)
                {
                    for (byte j = 0; j < gameData.matrix.GetLength(1); j++)
                    {
                        if (gameData.matrix[i, j] == (sbyte)valueOfGameSaddle)
                        {
                            minInRow = GetRow(i).Min();
                            maxInCol = GetColumn(j).Max();

                            if (minInRow == maxInCol)
                            {
                                LogsManager.ins.AddLog("Zaznaczono punkt siodłowy.");
                                TableManager.ins.SelectCell(cellID, EColors.Green);
                                saddlePointCount++;

                                yield return Wait();
                            }
                        }

                        cellID++;
                    }
                }

                LogsManager.ins.AddLog(string.Format("Znaleziono punkty siodłowe w liczbie: {0}.", saddlePointCount), EColors.Green);
                LogsManager.ins.AddLog("Proces analizy gry został zakończony.", EColors.Red);
                //Debug.LogErrorFormat("Wykryto punkt siodłowy ({0}). Value: {1}", saddlePointCount,valueOfGame);
            }
            else
            {
                MiniMaksManager.ins.Disable();
                if (checkSaddlePoint)
                {
                    LogsManager.ins.AddLog("Analiza nie wykazała istnienia punktu siodłowego.", EColors.Yellow);
                    yield return MinMaxPanel.ins.SetUp(false);
                    //Debug.LogWarning("Brak punktu siodłowego.");

                }

                if (checkDomination)
                {
                    LogsManager.ins.AddLog("Wyszukiwanie strategii zdominowanych.", EColors.Yellow);
                    yield return Wait();
                    //Debug.Log("Trwa analiza strategii zdominowanych");
                    yield return RemoveDominatedStrategies();
                }

                LogsManager.ins.AddLog("Analiza gry wymaga zastosowania strategii mieszanych.", EColors.Yellow);
                yield return Wait();

                byte rows = (byte)gameData.matrix.GetLength(0);
                byte columns = (byte)gameData.matrix.GetLength(1);

                if (rows == columns && rows == 2)
                {
                    FrequencyManager.ins.SetUp(gameData);
                    yield return Wait();
                    LogsManager.ins.AddLog("Przyblizona metoda rozwiązywania gry: 2x2.", EColors.Yellow);
                    // Debug.Log("Przyblizona metoda rozwiązywania gry: 2x2");
                    yield return Solve_2x2();
                    LogsManager.ins.AddLog("Wyznaczone zostały względne częstotliwości jako rozwiązanie gry.", EColors.Red);
                }
                else if ((rows == 2 && columns > 0) || (columns == 2 && rows > 2))
                {
                    LogsManager.ins.AddLog("Graficzna metoda wyszukiwania podgry 2x2.", EColors.Yellow);
                    // Debug.Log("Graficzna metoda wyszukiwania podgry 2x2");
                    yield return GraphicalManager.ins.SetUp(gameData.matrix);
                    yield return Wait();

                    yield return GraphicalMethod.Limits.FindSubGame();
                    
                    yield return Wait();

                    LogsManager.ins.AddLog("Znaleziona została podgra 2x2, odpowiednimi strategiami.", EColors.Yellow);
                    LogsManager.ins.AddLog(string.Format("Dla {0}, strategie, to: {1} oraz {2}", gameData.rowPlayer, gameData.rowStrategies[0], gameData.rowStrategies[1]), EColors.Yellow);
                    LogsManager.ins.AddLog(string.Format("Dla {0}, strategie, to: {1} oraz {2}", gameData.columnPlayer, gameData.columnStrategies[0], gameData.columnStrategies[1]), EColors.Yellow);

                    yield return Executive(true, false);
                }
                else if (rows > 2 && columns > 2)
                {
                    FrequencyManager.ins.SetUp(gameData);
                    yield return Wait();
                    LogsManager.ins.AddLog("Przyblizona metoda rozwiązywania dla dużych gier.", EColors.Yellow);
                    //Debug.Log("Przyblizona metoda rozwiązywania dla dużych gier");

                    SumManager.ins.SetUp(gameData);
                    yield return Solve_mxn();
                    LogsManager.ins.AddLog("Wyznaczone zostały względne częstotliwości jako rozwiązanie gry.", EColors.Red);
                }
            }

            HudManager.StopAnalysing();
        }

        /// <summary>
        /// Funkcja rozwiżuje grę 2-osobową o sumie zero o wymiarach mxn
        /// </summary>
        private IEnumerator Solve_mxn()
        {
            int repeatCount = 0;
            byte id = 0;
            sbyte[] data;

            int minVal = 0;
            int maxVal = 0;

            gameData.rowFreq = new int[gameData.matrix.GetLength(0)];
            gameData.colFreq = new int[gameData.matrix.GetLength(1)];

            int[] lastRow = new int[gameData.matrix.GetLength(0)];
            int[] lastCol = new int[gameData.matrix.GetLength(1)];

            int stopCount = gameData.matrix.GetLength(0) * gameData.matrix.GetLength(1);
            LogsManager.ins.AddLog(string.Format("Liczba powtórzeń liczona na podstawie ilocznu liczby strategii {0} oraz {1} i wynosi: {2}",
                                                    gameData.rowPlayer, gameData.columnPlayer, stopCount), EColors.Default);
            while (repeatCount < stopCount)
            {
                data = GetRow(id);
                lastRow = AddToLast(lastRow, data);
                gameData.rowFreq[id]++;

                LogsManager.ins.AddLog(string.Format("Pobieram strategię {0} gracza {1} i dodaję do dodatkowej tablicy", gameData.rowStrategies[id], gameData.rowPlayer));
                SumManager.ins.ChangeColSum(lastRow);
                TableManager.ins.SelectRow((byte)gameData.matrix.GetLength(0), id, EColors.Green, true);
                yield return Wait();
                LogsManager.ins.AddLog("Dla wybranej strategii zwiększam częstotliwość o 1");
                FrequencyManager.ins.ChangeRowFreq((sbyte)id, gameData.rowFreq[id]);
                yield return Wait();

                minVal = lastRow.Min();
                id = GetElementID(lastRow, minVal);

                LogsManager.ins.AddLog("Znajduję najmniejszą wartość w wybranym wierszu.");
                SumManager.ins.SelectRow(id);
                // TableManager.ins.SelectInSelected(id, EColors.Red);
                yield return Wait();

                data = GetColumn(id);
                lastCol = AddToLast(lastCol, data);
                gameData.colFreq[id]++;

                LogsManager.ins.AddLog(string.Format("Pobieram strategię {0} gracza {1} i dodaję do dodatkowej tablicy", gameData.columnStrategies[id], gameData.columnPlayer));
                SumManager.ins.ChangeRowSm(lastCol);
                TableManager.ins.SelectColumn((byte)gameData.matrix.GetLength(1), id, EColors.Green, true);
                yield return Wait();
                LogsManager.ins.AddLog("Dla wybranej strategii zwiększam częstotliwość o 1");
                FrequencyManager.ins.ChangeColFreq((sbyte)id, gameData.colFreq[id]);
                yield return Wait();

                maxVal = lastCol.Max();
                id = GetElementID(lastCol, maxVal);

                LogsManager.ins.AddLog("Znajduję najmniejszą wartość w wybranej kolumnie.");
                SumManager.ins.SelectColumn(id);
                //TableManager.ins.SelectInSelected(id, EColors.Red);
                yield return Wait();

                repeatCount++;
                LogsManager.ins.AddLog(string.Format("Powtórzenie: {0}/{1}.", repeatCount, stopCount), EColors.Yellow);
            }

            SumManager.ins.UnselectAll();
            TableManager.ins.SelectCell(0, EColors.Default, true);
            //float subError = (maxVal - minVal) / (float)repeatCount;



            LogsManager.ins.AddLog("Optymalne skracanie wyników.", EColors.Default);
            yield return Wait();

            gameData.rowFreq = DivisorRelativeRequences(gameData.rowFreq);
            gameData.colFreq = DivisorRelativeRequences(gameData.colFreq);

            for (sbyte i = 0; i < gameData.rowFreq.Length; i++)
            {
                FrequencyManager.ins.ChangeRowFreq(i, gameData.rowFreq[i]);
            }

            for (sbyte i = 0; i < gameData.rowFreq.Length; i++)
            {
                FrequencyManager.ins.ChangeColFreq(i, gameData.colFreq[i]);
            }
        }

        /// <summary>
        /// Metoda zwraca index elementu w tablicy
        /// </summary>
        /// <param name="data">Tablica elementów</param>
        /// <param name="element">Szukany element</param>
        /// <returns>index elementu w tablicy</returns>

        private byte GetElementID(int[] data, int element)
        {
            byte el = 0;

            for (byte i = 0; i < data.Length; i++)
            {
                if (data[i].Equals(element))
                {
                    el = i;
                    break;
                }
            }

            return el;
        }

        /// <summary>
        /// Metoda zwraca zsumowaną tablicę przy metodzie przybliżonej
        /// </summary>
        /// <param name="last">Tablica do której mają być dodane wartości</param>
        /// <param name="data">Wartości dodawane</param>
        /// <returns>Tablica zwiększona o wartości</returns>
        private int[] AddToLast(int[] last, sbyte[] data)
        {
            for (int i = 0; i < last.Length; i++) last[i] += data[i];
            return last;
        }

        /// <summary>
        /// Funkcja rozwiązuje grę 2-osobową o sumie zero o wymiarach 2x2
        /// </summary>
        private IEnumerator Solve_2x2()
        {
            yield return null;
            gameData.rowFreq = new int[2];
            gameData.colFreq = new int[2];

            sbyte[] row;
            for (byte i = 0; i < 2; i++)
            {
                row = GetRow(i);
                TableManager.ins.SelectRow(2, i, EColors.Green, true);
                LogsManager.ins.AddLog("Wybieram wiersz " + gameData.rowStrategies[i], EColors.Default);
                LogsManager.ins.AddLog(string.Format("Wykonując obliczenia: |{0}-{1}|=|{2}|={3}. Zapisuje w {4} wierszu dla częstotliwości.",
                                                     row[0], row[1], row[0] - row[1], Math.Abs(row[0] - row[1]), 1 - i), EColors.Default);
                yield return Wait();
                gameData.rowFreq[1 - i] = Math.Abs(row[0] - row[1]);
                FrequencyManager.ins.ChangeRowFreq((sbyte)(1 - i), gameData.rowFreq[1 - i]);

            }

            LogsManager.ins.AddLog("Optymalne skracanie wyników.", EColors.Default);
            yield return Wait();
            gameData.rowFreq = DivisorRelativeRequences(gameData.rowFreq);

            for (sbyte i = 0; i < gameData.rowFreq.Length; i++)
            {
                FrequencyManager.ins.ChangeRowFreq(i, gameData.rowFreq[i]);
            }

            sbyte[] column;
            for (byte i = 0; i < 2; i++)
            {
                column = GetColumn(i);
                TableManager.ins.SelectColumn(2, i, EColors.Green, true);
                LogsManager.ins.AddLog("Wybieram kolumnę " + gameData.columnStrategies[i], EColors.Default);
                LogsManager.ins.AddLog(string.Format("Wykonując obliczenia: |{0}-{1}|=|{2}|={3}. Zapisuje w {4} kolumnie dla częstotliwości.",
                                        column[0], column[1], column[0] - column[1], Math.Abs(column[0] - column[1]), 1 - i), EColors.Default);
                yield return Wait();
                gameData.colFreq[1 - i] = Math.Abs(column[0] - column[1]);
                FrequencyManager.ins.ChangeColFreq((sbyte)(1 - i), gameData.colFreq[1 - i]);
            }

            TableManager.ins.SelectCell(0, EColors.Default, true);

            LogsManager.ins.AddLog("Optymalne skracanie wyników.", EColors.Default);
            yield return Wait();
            gameData.colFreq = DivisorRelativeRequences(gameData.colFreq);

            for (sbyte i = 0; i < gameData.colFreq.Length; i++)
            {
                FrequencyManager.ins.ChangeColFreq(i, gameData.colFreq[i]);
            }

        }

        /// <summary>
        /// Funkcja usuwa z macierzy wypłat wszystkie strategie zdominowane
        /// </summary>
        private IEnumerator RemoveDominatedStrategies()
        {
            byte oldCols; // ile wierszy i kolumn posiada macierz przed dominacją

            do
            {
                oldCols = (byte)gameData.matrix.GetLength(1);

                yield return RowsDomination();
                yield return ColumnsDomination();

            } while (oldCols != gameData.matrix.GetLength(1));

            LogsManager.ins.AddLog("Strategie zdominowane zostały usunięte.", EColors.Yellow);
            //Debug.LogWarning("Strategie zdominowane zostały usunięte");
        }

        /// <summary>
        /// Funkcja realizowana na rzecz funkcji RemoveDominatedStrtegies,
        /// która usuwa wszystkie zdominowane strategie Gracza Kolumnowego
        /// </summary>
        private IEnumerator ColumnsDomination()
        {
            sbyte[] colA, colB;
            yield return Wait();


            for (byte i = 0; i < gameData.matrix.GetLength(1) - 1; i++)
            {

                colA = GetColumn(i);

                for (byte j = (byte)(i + 1); j < gameData.matrix.GetLength(1); j++)
                {
                    colB = GetColumn(j);
                    LogsManager.ins.AddLog(string.Format("Porównuję strategię gracza: {2} - {0} z {1}", gameData.columnStrategies[i], gameData.columnStrategies[j], gameData.columnPlayer));

                    TableManager.ins.SelectColumn((byte)gameData.matrix.GetLength(1), i, EColors.Green, true);
                    yield return Wait();
                    TableManager.ins.SelectColumn((byte)gameData.matrix.GetLength(1), j, EColors.Yellow);
                    yield return Wait();

                    // 1 = colA dominuje colB; 1 = colB dominuje colA; 0 = brak dominacji
                    sbyte domination = Compare(colA, colB);

                    if (domination == 1)
                    {
                        LogsManager.ins.AddLog(string.Format("Strategia {0} jest dominowana przez {1}", gameData.columnStrategies[j], gameData.columnStrategies[i]));
                        //Debug.LogFormat("Strategia {0} jest dominowana przez {1}", gameData.columnStrategies[j], gameData.columnStrategies[i]);

                        TableManager.ins.SelectColumn((byte)gameData.matrix.GetLength(1), j, EColors.Red, true);
                        yield return Wait(new WaitForSeconds(2f * speed));

                        gameData.matrix = RemoveColumnFromMatrix(j--);
                        TableManager.ins.DisplayTable(gameData);
                        yield return Wait();
                    }
                    else if (domination == -1)
                    {
                        LogsManager.ins.AddLog(string.Format("Strategia {0} dominuje strategię {1}", gameData.columnStrategies[j], gameData.columnStrategies[i]));
                        // Debug.LogFormat("Strategia {0} dominuje strategię {1}", gameData.columnStrategies[j], gameData.columnStrategies[i]);

                        TableManager.ins.SelectColumn((byte)gameData.matrix.GetLength(1), i, EColors.Red, true);
                        yield return Wait(new WaitForSeconds(2f * speed));

                        gameData.matrix = RemoveColumnFromMatrix(i--);
                        TableManager.ins.DisplayTable(gameData);
                        yield return Wait();
                        break;
                    }
                    else
                    {
                        TableManager.ins.SelectCell(0, EColors.Default, true);
                        LogsManager.ins.AddLog(string.Format("Brak dominacji pomiędzy strategiami {0} - {1}", gameData.columnStrategies[j], gameData.columnStrategies[i]));
                        yield return Wait();
                    }
                }
            }
        }

        /// <summary>
        /// Funkcja realizowana na rzecz funkcji RemoveDominatedStrategies,
        /// która usuwa wszystkie zdominowane strategie Gracza wierszowego
        /// </summary>
        private IEnumerator RowsDomination()
        {
            sbyte[] rowA, rowB;
            yield return Wait();

            for (byte i = 0; i < gameData.matrix.GetLength(0) - 1; i++)
            {
                rowA = GetRow(i);

                for (byte j = (byte)(i + 1); j < gameData.matrix.GetLength(0); j++)
                {
                    LogsManager.ins.AddLog(string.Format("Porównuję strategię gracza: {2} - {0} z {1}", gameData.rowStrategies[i], gameData.rowStrategies[j], gameData.rowPlayer));
                    rowB = GetRow(j);

                    TableManager.ins.SelectRow((byte)gameData.matrix.GetLength(0), i, EColors.Green, true);
                    yield return Wait();
                    TableManager.ins.SelectRow((byte)gameData.matrix.GetLength(0), j, EColors.Yellow);
                    yield return Wait();

                    // -1 = rowA  dominuje rowB; 1 = rowB dominuje rowA; 0 = brak dominacji
                    sbyte domination = Compare(rowA, rowB);

                    if (domination == -1)
                    {
                        LogsManager.ins.AddLog(string.Format("Strategia {0} jest dominowana przez {1}", gameData.rowStrategies[j], gameData.rowStrategies[i]));
                        // Debug.LogFormat("Strategia {0} jest dominowana przez {1}", gameData.rowStrategies[j], gameData.rowStrategies[i]);

                        TableManager.ins.SelectRow((byte)gameData.matrix.GetLength(0), j, EColors.Red, true);
                        yield return Wait(new WaitForSeconds(2f * speed));

                        gameData.matrix = RemoveRowFromMatrix(j--);
                        TableManager.ins.DisplayTable(gameData);
                        yield return Wait();
                    }
                    else if (domination == 1)
                    {
                        LogsManager.ins.AddLog(string.Format("Strategia {0} dominuje strategię {1}", gameData.rowStrategies[j], gameData.rowStrategies[i]));
                        // Debug.LogFormat("Strategia {0} dominuje strategię {1}", gameData.rowStrategies[j], gameData.rowStrategies[i]);

                        TableManager.ins.SelectRow((byte)gameData.matrix.GetLength(0), i, EColors.Red, true);
                        yield return Wait(new WaitForSeconds(2f * speed));

                        gameData.matrix = RemoveRowFromMatrix(i--);
                        TableManager.ins.DisplayTable(gameData);
                        yield return Wait();
                        break;
                    }
                    else
                    {
                        TableManager.ins.SelectCell(0, EColors.Default, true);
                        LogsManager.ins.AddLog(string.Format("Brak dominacji pomiędzy strategiami {0} - {1}", gameData.rowStrategies[j], gameData.rowStrategies[i]));
                        yield return Wait();
                    }
                }
            }
        }

        /// <summary>
        /// Funkcja usuwa wybrany wiersz z macierzy
        /// </summary>
        /// <param name="rowID">id wiersza</param>
        /// <returns>nowa macierz wypłat, bez jednego wiersza</returns>
        private sbyte[,] RemoveRowFromMatrix(byte rowID)
        {
            sbyte[,] newMatrix = new sbyte[gameData.matrix.GetLength(0) - 1, gameData.matrix.GetLength(1)];

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

        /// <summary>
        /// Funkcja usuwa wybraną kolumnę z macierzy
        /// </summary>
        /// <param name="columnID">id kolumny</param>
        /// <returns>nowa macierz wypłat, bez jednej kolumny</returns>
        private sbyte[,] RemoveColumnFromMatrix(byte columnID)
        {
            sbyte[,] newMatrix = new sbyte[gameData.matrix.GetLength(0), gameData.matrix.GetLength(1) - 1];

            for (byte y = 0, ny = 0; y < gameData.matrix.GetLength(1); y++, ny++)
            {
                if (columnID == y)
                {
                    ny--;
                    continue;
                }

                for (byte x = 0; x < gameData.matrix.GetLength(0); x++) newMatrix[x, ny] = gameData.matrix[x, y];
            }

            gameData.columnStrategies.RemoveAt(columnID);
            return newMatrix;
        }

        /// <summary>
        /// Porównuje dwa wiersze lub dwie kolumny.
        /// </summary>
        /// <param name="A">Wiersz pierwszy</param>
        /// <param name="B">Wiersz drugi</param>
        /// <returns>Zwraca 1 jeżeli wiersz/kolumna drugi/a dominuje, zwraca -1 jeżeli wiersz/kolumna pierwszy/a dominuje, zwraca 0, jeżeli nie ma dominacji</returns>
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

        /// <summary>
        /// Funkcja zwraca informację, czy macierz wypłat posiada punkt siodłowy
        /// </summary>
        /// <returns>true, jeżeli ma punkt siodłowy; false, jeżeli nie ma punktu siodłowego</returns>
        private IEnumerator HasSaddlePoint()
        {
            LogsManager.ins.AddLog("Sprawdzam punkty siodłowe", EColors.Yellow);
            MiniMaksManager.ins.SetUp(gameData);
            yield return Wait();
            bool has = false;

            yield return MinMax();
            yield return MaxMin();
            TableManager.ins.SelectCell(0, EColors.Default, true);

            if (minMax == maxMin)
            {
                has = true;
                valueOfGameSaddle = minMax;
            }

            hasSaddlePoint = has;
        }


        /// <summary>
        /// Funkcja zwraca największą wartość ze wszystkich najmnijeszych w każdym wierszu
        /// </summary>
        /// <returns></returns>
        private IEnumerator MinMax()
        {
            yield return null;
            sbyte max = sbyte.MinValue;
            sbyte minInRow;
            sbyte[] row;

            for (byte i = 0; i < gameData.matrix.GetLength(0); i++)
            {
                row = GetRow(i);
                TableManager.ins.SelectRow((byte)gameData.matrix.GetLength(0), i, EColors.Green, true);
                yield return Wait();
                minInRow = row.Min();

                MiniMaksManager.ins.ChangeMinVal(i, minInRow);
                LogsManager.ins.AddLog(string.Format("Znaleziono najmniejszą wartość w strategii {0} gracza: {1}", gameData.rowStrategies[i], gameData.rowPlayer));

                if (minInRow > max)
                {
                    max = minInRow;

                    LogsManager.ins.AddLog("Znaleziony min wiersza jest największy.");
                    MiniMaksManager.ins.SelectMin(i, EColors.Red, true);
                }
                else if (minInRow == max)
                {
                    LogsManager.ins.AddLog("Znaleziono kolejny min wiersza");
                    MiniMaksManager.ins.SelectMin(i, EColors.Red);
                }
                else
                {
                    LogsManager.ins.AddLog("Znaleziona strategia nie jest największa spośród minimów");
                }

                yield return Wait();

            }

            minMax = max;
        }

        /// <summary>
        /// Funkcja zwraca majmniejszą wartość ze wszystkich największych w każdej kolumnie
        /// </summary>
        /// <returns></returns>
        private IEnumerator MaxMin()
        {
            yield return null;
            sbyte min = sbyte.MaxValue;
            sbyte maxInColumn;
            sbyte[] column;

            for (byte i = 0; i < gameData.matrix.GetLength(1); i++)
            {
                column = GetColumn(i);
                TableManager.ins.SelectColumn((byte)gameData.matrix.GetLength(1), i, EColors.Green, true);
                yield return Wait();
                maxInColumn = column.Max();

                MiniMaksManager.ins.ChangeMaxVal(i, maxInColumn);
                LogsManager.ins.AddLog(string.Format("Znaleziono największą wartość w strategii {0} gracza: {1}", gameData.columnStrategies[i], gameData.columnPlayer));

                if (maxInColumn < min)
                {
                    min = maxInColumn;
                    LogsManager.ins.AddLog("Znaleziony maks kolumny jest najmniejszy.");
                    MiniMaksManager.ins.SelectMaks(i, EColors.Red, true);
                }
                else if (maxInColumn == min)
                {
                    LogsManager.ins.AddLog("Znaleziono kolejny maks kolumny");
                    MiniMaksManager.ins.SelectMaks(i, EColors.Red);
                }
                else
                {
                    LogsManager.ins.AddLog("Znaleziona strategia nie jest najmniejsza spośród maksimów");
                }

                yield return Wait();

            }
            maxMin = min;
        }

        /// <summary>
        /// Funkcja zwraca wiersz z wypłatami
        /// </summary>
        /// <param name="rowID">id wiersza</param>
        /// <returns>wiersz z wypłatami</returns>
        private sbyte[] GetRow(byte rowID)
        {
            sbyte[] row = new sbyte[gameData.matrix.GetLength(1)];

            for (byte i = 0; i < row.Length; i++)
                row[i] = gameData.matrix[rowID, i];

            return row;
        }

        /// <summary>
        /// Funkcja zwraca kolumnę z wypłatami
        /// </summary>
        /// <param name="columnID">id kolumny</param>
        /// <returns>kolumna z wypłatami</returns>
        private sbyte[] GetColumn(byte columnID)
        {
            sbyte[] column = new sbyte[gameData.matrix.GetLength(0)];

            for (byte i = 0; i < column.Length; i++)
                column[i] = gameData.matrix[i, columnID];

            return column;
        }

        /// <summary>
        /// Funckja zwraca maciarz wypłat z pliku
        /// </summary>
        /// <param name="fileName">Nazwa pliku</param>
        /// <returns>Macierz wypłat</returns>
        private sbyte[,] GetMatrixFromFile(string fileName)
        {
            string path = string.Format(@"{0}\Games", Application.dataPath);
            sbyte[,] loadedMatrix;

            using (StreamReader sR = new StreamReader(string.Format(@"{0}\{1}", path, fileName)))
            {

                string line = sR.ReadLine();        // Pierwsza linia określna liczbę kolumn i wierszy macierzy.
                string[] sub = line.Split(' ');

                loadedMatrix = new sbyte[byte.Parse(sub[0]), byte.Parse(sub[1])];

                byte row = 0;
                while ((line = sR.ReadLine()) != null)
                {
                    sub = line.Split(' ');

                    for (int column = 0; column < loadedMatrix.GetLength(1); column++)
                        loadedMatrix[row, column] = sbyte.Parse(sub[column]);

                    row++;
                }

                if (sR != null) sR.Close();

            }

            return loadedMatrix;
        }

        /// <summary>
        /// Funkcja dzieli częstotliwości przez najwiekszy wspolny dzielnik
        /// </summary>
        /// <param name="freq">Względne częstotliwości</param>
        /// <returns>Podzielone przez NWD względne częstotliwości</returns>
        private int[] DivisorRelativeRequences(int[] freq)
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

        public static IEnumerator Wait(WaitForSeconds waitForSec = null)
        {
            if (waitForSec == null)
                yield return wfs;
            else
                yield return waitForSec;
        }
    }
}

