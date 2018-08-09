using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scripts.States.GraphicalMethod;
using System.Collections;

namespace Scripts.Strategies.Solver
{
    public class MixedGraphical : StrategySolver
    {
        private IStateGraphicalMethod stan;

        public MixedGraphical(IStateGraphicalMethod stan)
        {
            this.stan = stan;
        }

        public override IEnumerator SolveGame(Game game)
        {
            LogsManager.ins.AddLog("Rozwiązywanie gry metodą graficzną.", EColors.Blue);
            yield return GraphicalManager.ins.SetUp(game.gameData.matrix);
            yield return stan.SolveGame(game);
            TableManager.ins.DisplayTable(game.gameData);
            yield return game.SolveGame();
        }
    }
}
