using Scripts.Strategies.Solver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Scripts.States.Solver
{
    public class StateSaddlePoint : StateSolver
    {
        private int valueOfGame;

        public StateSaddlePoint(int valueOfGame)
        {
            this.valueOfGame = valueOfGame;
        }

        public IEnumerator SolveGame(Game game)
        {
            sbyte minInRow, maxInCol;
            byte cellID = 0;
            LogsManager.ins.AddLog("Zaznaczanie punktów siodłowych.");

            yield return MinMaxPanel.ins.SetUp(true);
            yield return LogsManager.Wait();
            TableManager.ins.SelectCell(0, EColors.Default, true);

            for (byte i = 0; i < game.gameData.matrix.GetLength(0); i++)
            {
                for (byte j = 0; j < game.gameData.matrix.GetLength(1); j++)
                {
                    if (game.gameData.matrix[i, j] == (sbyte)valueOfGame)
                    {
                        minInRow = game.GetRow(i).Min();
                        maxInCol = game.GetColumn(j).Max();

                        if (minInRow == maxInCol)
                        {
                            LogsManager.ins.AddLog("Zaznaczono punkt siodłowy.");
                            yield return LogsManager.Wait();
                            TableManager.ins.SelectCell(cellID, EColors.Green);
                        }
                    }

                    cellID++;
                }
            }
        }
    }
}
