using GraphicalMethod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripts.States.GraphicalMethod
{
    public interface IStateGraphicalMethod
    {
        Cross2x2 point { get; set; }
        int[,] subGame { get; set; }
        byte id { get; set; }

        IEnumerator SolveGame(Game game);
    }
}
