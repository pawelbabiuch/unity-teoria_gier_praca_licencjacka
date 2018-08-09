using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripts.Strategies.Solver
{
    public class MixedApproximate : StrategySolver
    {
        public override IEnumerator SolveGame(Game game)
        {
             LogsManager.ins.AddLog("Rozwiązywanie gry metodą przybliżoną.", EColors.Blue);
            GameData gameData = game.gameData;
            FrequencyManager.ins.SetUp(gameData);
            SumManager.ins.SetUp(gameData);

            yield return LogsManager.Wait();

            int repeatCount = 0;
            byte id = 0;
            sbyte[] data;

            int minVal = 0;
            int maxVal = 0;

            gameData.rowFreq = new int[gameData.matrix.GetLength(0)];
            gameData.colFreq = new int[gameData.matrix.GetLength(1)];

            int[] lastRow = new int[gameData.matrix.GetLength(1)];
            int[] lastCol = new int[gameData.matrix.GetLength(0)];

            int stopCount = gameData.matrix.GetLength(0) * gameData.matrix.GetLength(1);
            LogsManager.ins.AddLog(string.Format("Liczba powtórzeń liczona na podstawie ilocznu liczby strategii {0} oraz {1} i wynosi: {2}",
            gameData.playerA, gameData.playerB, stopCount), EColors.Default);
            yield return LogsManager.Wait();
            while (repeatCount < stopCount)
            {
                data = game.GetRow(id);
                lastRow = AddToLast(lastRow, data);
                gameData.rowFreq[id]++;

                LogsManager.ins.AddLog(string.Format("Pobieram strategię {0} gracza {1} i dodaję do dodatkowej tablicy", gameData.rowStrategies[id].ToString(), gameData.playerA));
                SumManager.ins.ChangeColSum(lastRow);
                TableManager.ins.SelectRow((byte)gameData.matrix.GetLength(0), id, EColors.Green, true);
                yield return LogsManager.Wait();

                LogsManager.ins.AddLog("Dla wybranej strategii zwiększam częstotliwość o 1");
                FrequencyManager.ins.ChangeRowFreq((sbyte)id, gameData.rowFreq[id]);
                yield return LogsManager.Wait();

                minVal = lastRow.Min();
                id = GetElementID(lastRow, minVal);

                LogsManager.ins.AddLog("Znajduję najmniejszą wartość w wybranym wierszu.");
                SumManager.ins.SelectRow(id);
                yield return LogsManager.Wait();

                data = game.GetColumn(id);
                lastCol = AddToLast(lastCol, data);
                gameData.colFreq[id]++;

                LogsManager.ins.AddLog(string.Format("Pobieram strategię {0} gracza {1} i dodaję do dodatkowej tablicy", gameData.colStrategies[id].ToString(), gameData.playerB));
                SumManager.ins.ChangeRowSm(lastCol);
                TableManager.ins.SelectColumn((byte)gameData.matrix.GetLength(1), id, EColors.Green, true);
                yield return LogsManager.Wait();

                LogsManager.ins.AddLog("Dla wybranej strategii zwiększam częstotliwość o 1");
                FrequencyManager.ins.ChangeColFreq((sbyte)id, gameData.colFreq[id]);
                yield return LogsManager.Wait();

                maxVal = lastCol.Max();
                id = GetElementID(lastCol, maxVal);

                LogsManager.ins.AddLog("Znajduję najmniejszą wartość w wybranej kolumnie.");
                SumManager.ins.SelectColumn(id);
                yield return LogsManager.Wait();

                repeatCount++;
                LogsManager.ins.AddLog(string.Format("Powtórzenie: {0}/{1}.", repeatCount, stopCount), EColors.Yellow);
            }

            SumManager.ins.UnselectAll();
            TableManager.ins.SelectCell(0, EColors.Default, true);

            LogsManager.ins.AddLog("Optymalne skracanie wyników.", EColors.Blue);

            gameData.rowFreq = Game.DivisorRelativeRequences(gameData.rowFreq);
            gameData.colFreq = Game.DivisorRelativeRequences(gameData.colFreq);

            for (sbyte i = 0; i < gameData.rowFreq.Length; i++)
            {
                FrequencyManager.ins.ChangeRowFreq(i, gameData.rowFreq[i]);
            }

            for (sbyte i = 0; i < gameData.colFreq.Length; i++)
            {
                FrequencyManager.ins.ChangeColFreq(i, gameData.colFreq[i]);
            }
        }

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

        private int[] AddToLast(int[] last, sbyte[] data)
        {
            for (int i = 0; i < last.Length; i++) last[i] += data[i];
            return last;
        }
    }
}
