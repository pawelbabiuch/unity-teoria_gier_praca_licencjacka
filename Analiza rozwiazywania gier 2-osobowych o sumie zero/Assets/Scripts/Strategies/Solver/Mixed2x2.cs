using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Scripts.Strategies.Solver
{
    public class Mixed2x2 : StrategySolver
    {
        public override IEnumerator SolveGame(Game game)
        {
            LogsManager.ins.AddLog("Rozwiazywanie gry w strategiach mieszanych 2x2");
            FrequencyManager.ins.SetUp(game.gameData);
            yield return LogsManager.Wait();

            game.gameData.rowFreq = new int[2];
            game.gameData.colFreq = new int[2];

            sbyte[] row;
            for (byte i = 0; i < 2; i++)
            {
                row = game.GetRow(i);
                TableManager.ins.SelectRow(2, i, EColors.Green, true);
                LogsManager.ins.AddLog("Wybieram wiersz " + game.gameData.rowStrategies[i].ToString(), EColors.Blue);
                yield return LogsManager.Wait();
                LogsManager.ins.AddLog(string.Format("Wykonując obliczenia: |{0}-{1}|=|{2}|={3}. Zapisuje w {4} wierszu dla częstotliwości.",
                                                     row[0], row[1], row[0] - row[1], Math.Abs(row[0] - row[1]), 1 - i), EColors.Default);
                yield return LogsManager.Wait();
                game.gameData.rowFreq[1 - i] = Math.Abs(row[0] - row[1]);
                FrequencyManager.ins.ChangeRowFreq((sbyte)(1 - i), game.gameData.rowFreq[1 - i]);
                yield return LogsManager.Wait();
            }

            LogsManager.ins.AddLog("Optymalne skracanie wyników.", EColors.Blue);
            yield return LogsManager.Wait();
            game.gameData.rowFreq = Game.DivisorRelativeRequences(game.gameData.rowFreq);

            for (sbyte i = 0; i < game.gameData.rowFreq.Length; i++)
            {
                FrequencyManager.ins.ChangeRowFreq(i, game.gameData.rowFreq[i]);
                yield return LogsManager.Wait();
            }

            sbyte[] column;
            for (byte i = 0; i < 2; i++)
            {
                column = game.GetColumn(i);
                TableManager.ins.SelectColumn(2, i, EColors.Green, true);
                LogsManager.ins.AddLog("Wybieram kolumnę " + game.gameData.colStrategies[i].ToString(), EColors.Blue);
                yield return LogsManager.Wait();
                LogsManager.ins.AddLog(string.Format("Wykonując obliczenia: |{0}-{1}|=|{2}|={3}. Zapisuje w {4} kolumnie dla częstotliwości.",
                                        column[0], column[1], column[0] - column[1], Math.Abs(column[0] - column[1]), 1 - i), EColors.Default);
                yield return LogsManager.Wait();
                game.gameData.colFreq[1 - i] = Math.Abs(column[0] - column[1]);
                FrequencyManager.ins.ChangeColFreq((sbyte)(1 - i), game.gameData.colFreq[1 - i]);
                yield return LogsManager.Wait();
            }

            TableManager.ins.SelectCell(0, EColors.Default, true);
            LogsManager.ins.AddLog("Optymalne skracanie wyników.", EColors.Blue);
            yield return LogsManager.Wait();

            game.gameData.colFreq = Game.DivisorRelativeRequences(game.gameData.colFreq);

            for (sbyte i = 0; i < game.gameData.colFreq.Length; i++)
            {
                FrequencyManager.ins.ChangeColFreq(i, game.gameData.colFreq[i]);
                yield return LogsManager.Wait();
            }
        }
    }
}
