using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Scripts
{
    public sealed class GameAnalysis
    {
        public Game game;

        public GameAnalysis(GameData gameData)
        {
            this.game = new Game(gameData);
            TableManager.ins.DisplayTable(this.game.gameData);
        }

        public IEnumerator StartGameAnalysis()
        {

            if (game == null)
            {
                LogsManager.ins.AddLog("Brak utworzonej gry do analizy.", EColors.Yellow);
            }
            else
            {
                LogsManager.ins.AddLog("Zaczeto analize gry.", EColors.Blue);
                TableManager.ins.UpdateTable(game.gameData);
                TableManager.ins.cG.blocksRaycasts = false;

                yield return game.CheckSaddlePoint();
                yield return game.CheckDominations();
                yield return game.SolveGame();
                LogsManager.ins.AddLog("Analiza została zakończona.", EColors.Green);
                HUDManager2.StopAnalysing();
            }
        }
    }
}
