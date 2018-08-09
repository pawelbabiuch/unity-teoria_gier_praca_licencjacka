using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripts.Strategies.Solver
{
    public abstract class StrategySolver
    {
        public abstract IEnumerator SolveGame(Game game);
    }
}
