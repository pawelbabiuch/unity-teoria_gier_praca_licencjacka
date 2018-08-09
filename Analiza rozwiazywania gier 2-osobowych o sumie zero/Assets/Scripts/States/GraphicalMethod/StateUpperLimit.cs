using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphicalMethod;
using UnityEngine;

namespace Scripts.States.GraphicalMethod
{
    public class StateUpperLimit : IStateGraphicalMethod
    {

        private Cross2x2 _point = null;
        public int[,] _subGame = new int[2,2];
        private byte _id;

        public byte id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Cross2x2 point
        {
            get { return _point; }
            set { _point = value; }
        }

        public int[,] subGame
        {
            get { return _subGame; }
            set { _subGame = value; }
        }

        public IEnumerator SolveGame(Game game)
        {
            yield return null;

            LogsManager.ins.AddLog(string.Format("{0} ma dokładnie dwie strategie.", game.gameData.playerB));
            LogsManager.ins.AddLog("Wyszukiwanie strategii dającej największą przegraną dla gracza");
            yield return LogsManager.Wait();

            id = IDTheLargestVal(game.gameData);
            yield return CrossOverMax(id, game.gameData);

            subGame[1, 0] = game.gameData.matrix[point.strategyA_ID, 0];
            subGame[1, 1] = game.gameData.matrix[point.strategyA_ID, 1];

            subGame[0, 0] = game.gameData.matrix[point.strategyB_ID, 0];
            subGame[0, 1] = game.gameData.matrix[point.strategyB_ID, 1];

            char[] useingStrategies = new char[2] { game.gameData.rowStrategies[point.strategyA_ID],
                                                            game.gameData.rowStrategies[point.strategyB_ID]};
            game.gameData.rowStrategies = useingStrategies.ToList();

            game.gameData.matrix = subGame;
        }

        private IEnumerator CrossOverMax(byte idLargest, GameData gameData, double max = double.MaxValue)
        {
            LogsManager.ins.AddLog("Strategia znaleziona: " + gameData.rowStrategies[idLargest]);
            GraphicalManager.ins.SelectLine(idLargest, EColors.Red, true);
            yield return LogsManager.Wait();
            byte newID = 0;

            Vector strategyA_start = new Vector(gameData.matrix[idLargest, 0], 0);
            Vector strategyA_end = new Vector(gameData.matrix[idLargest, 1], 1);

            if (GraphicalManager.ins.highLine.Points.Length == 0)
                GraphicalManager.ins.SetHighLinePoint(new Vector2((float)strategyA_start.X, (float)strategyA_start.Y));

            Vector strategyB_start, strategyB_end;

            Vector maxIntersaction = new Vector(double.MinValue, 0);
            Vector intersection;

            for (byte i = 0; i < gameData.matrix.GetLength(0); i++)
            {
                if (strategyA_start.X < strategyA_end.X) break;

                strategyB_start = new Vector(gameData.matrix[i, 0], 0);
                strategyB_end = new Vector(gameData.matrix[i, 1], 1);

                if (strategyA_start.Equals(strategyB_start) && strategyA_end.Equals(strategyB_end)) continue;

                bool actual = Vector.Intersect(strategyA_start, strategyA_end, strategyB_start, strategyB_end, out intersection);

                if (actual && intersection.X > maxIntersaction.X && Math.Round(intersection.X, 2) < Math.Round(max, 2))
                {
                    maxIntersaction = intersection;
                    newID = i;
                }
            }

            if (maxIntersaction.X > double.MinValue)
            {
                Cross2x2 c = new Cross2x2(idLargest, newID, maxIntersaction);

                if (point == null || c.point.X < point.point.X)
                    point = c;

                GraphicalManager.ins.SetHighLinePoint(new Vector2((float)point.point.X, (float)point.point.Y));
                yield return CrossOverMax(newID, gameData, maxIntersaction.X);
            }
            else
            {
                GraphicalManager.ins.SetHighLinePoint(new Vector2((float)strategyA_end.X, (float)strategyA_end.Y));
                GraphicalManager.ins.SelectLine(0, EColors.Default, true);
            }
        }

        private static byte IDTheLargestVal(GameData gameData)
        {
            byte id = 0;
            int theLargest = int.MinValue;
            int curVal;

            for (byte i = 0; i < gameData.matrix.GetLength(0); i++)
            {
                curVal = gameData.matrix[i, 0];

                if (curVal > theLargest)
                {
                    theLargest = curVal;
                    id = i;
                }
            }

            return id;
        }
    }
}
