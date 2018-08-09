using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripts.States.Solver
{
    interface StateSolver
    {
        IEnumerator SolveGame(Scripts.Game game);
    }
}
