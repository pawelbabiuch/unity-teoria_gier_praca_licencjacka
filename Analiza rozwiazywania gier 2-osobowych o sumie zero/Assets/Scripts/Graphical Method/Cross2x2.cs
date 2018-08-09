using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicalMethod
{
    public sealed class Cross2x2
    {
        public byte strategyA_ID { get; private set; }
        public byte strategyB_ID { get; private set; }

        public Vector point { get; private set; }

        public Cross2x2(byte strategyA_ID, byte strategyB_ID, Vector point)
        {
            this.strategyA_ID = strategyA_ID;
            this.strategyB_ID = strategyB_ID;
            this.point = point;
        }
    }
}
