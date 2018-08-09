using Scripts.Strategies.Solver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Scripts.States.Solver
{
    public class StateNoSaddlePoint : StateSolver
    {
        public IEnumerator SolveGame(Game game)
        {
            MiniMaksManager.ins.Disable();
            yield return CheckStrategy(game).SolveGame(game);
        }

        private StrategySolver CheckStrategy(Game game)
        {
            LogsManager.ins.AddLog("Szukanie rozwiązania w strategiach mieszanych.");
            byte rowStrategies = (byte)GameData.instance.rowStrategies.Count;
            byte colStrategies = (byte)GameData.instance.colStrategies.Count;

            if (rowStrategies == colStrategies && rowStrategies == 2) return new Mixed2x2();
            else if (rowStrategies == 2 && colStrategies > 2) return new MixedGraphical(new GraphicalMethod.StateLowerLimit());
            else if (rowStrategies > 2 && colStrategies == 2) return new MixedGraphical(new GraphicalMethod.StateUpperLimit());
            else return new MixedApproximate();
        }
    }
}
