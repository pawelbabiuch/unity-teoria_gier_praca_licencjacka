using GraphicalMethod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Scripts.States.GraphicalMethod
{
    public class StateLowerLimit : IStateGraphicalMethod
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

            LogsManager.ins.AddLog(string.Format("{0} ma dokładnie dwie strategie.", game.gameData.playerA));
            yield return LogsManager.Wait();
            id = IdTheSmalestVal(game.gameData);
            yield return CrossOverMin(id, game.gameData);

            subGame[0, 1] = game.gameData.matrix[0, point.strategyA_ID];
            subGame[1, 1] = game.gameData.matrix[1, point.strategyA_ID];

            subGame[0, 0] = game.gameData.matrix[0, point.strategyB_ID];
            subGame[1, 0] = game.gameData.matrix[1, point.strategyB_ID];

            char[] useingStrategies = new char[2] { game.gameData.colStrategies[point.strategyA_ID],
                                                            game.gameData.colStrategies[point.strategyB_ID]};
            game.gameData.colStrategies = useingStrategies.ToList();

            game.gameData.matrix = subGame;
        }

        private IEnumerator CrossOverMin(byte idSmallest, GameData gameData, double min = double.MinValue)
        {
            LogsManager.ins.AddLog("Strategia znaleziona: " + gameData.colStrategies[idSmallest].ToString());
            GraphicalManager.ins.SelectLine(idSmallest, EColors.Red, true);
            yield return LogsManager.Wait();
            byte newID = 0;

            Vector strategyA_start = new Vector(0, gameData.matrix[0, idSmallest]);
            Vector strategyA_end = new Vector(1, gameData.matrix[1, idSmallest]);

            if (GraphicalManager.ins.highLine.Points.Length == 0)
                GraphicalManager.ins.SetHighLinePoint(new Vector2((float)strategyA_start.Y, (float)strategyA_start.X));

            Vector strategyB_start, strategyB_end;

            Vector minIntersaction = new Vector(0, double.MaxValue);
            Vector intersaction;


            for (byte i = 0; i < gameData.matrix.GetLength(1); i++)
            {

                if (strategyA_start.Y > strategyA_end.Y) break;

                strategyB_start = new Vector(0, gameData.matrix[0, i]);
                strategyB_end = new Vector(1, gameData.matrix[1, i]);

                if (strategyA_start.Equals(strategyB_start) && strategyA_end.Equals(strategyB_end)) continue;

                bool actual = Vector.Intersect(strategyA_start, strategyA_end, strategyB_start, strategyB_end, out intersaction);

                if (actual && intersaction.Y < minIntersaction.Y && Math.Round(intersaction.Y, 2) > Math.Round(min, 2))
                {
                    minIntersaction = intersaction;
                    newID = i;
                }
            }

            if (minIntersaction.Y < double.MaxValue)
            {
                Cross2x2 c = new Cross2x2(idSmallest, newID, minIntersaction);

                if (point == null || c.point.Y > point.point.Y)
                    point = c;

                GraphicalManager.ins.SetHighLinePoint(new Vector2((float)point.point.Y, (float)point.point.X));
                yield return CrossOverMin(newID, gameData, minIntersaction.Y);
            }
            else
            {
                GraphicalManager.ins.SetHighLinePoint(new Vector2((float)strategyA_end.Y, (float)strategyA_end.X));
                GraphicalManager.ins.SelectLine(0, EColors.Default, true);
            }
        }

        private static byte IdTheSmalestVal(GameData gameData)
        {
            byte id = 0;
            int theSmallest = int.MaxValue;
            int curVal;

            for (byte i = 0; i < gameData.matrix.GetLength(1); i++)
            {
                curVal = gameData.matrix[0, i];

                if (curVal < theSmallest)
                {
                    theSmallest = curVal;
                    id = i;
                }
            }

            return id;
        }
    }
}
