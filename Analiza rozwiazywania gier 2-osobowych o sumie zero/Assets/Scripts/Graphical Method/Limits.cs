using GameTheory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GraphicalMethod
{
    public static class Limits
    {
        private static Cross2x2 point { get; set; }

        public static IEnumerator FindSubGame()
        {
            point = null;
            sbyte[,] subGame = new sbyte[2, 2];
            byte id;

            LogsManager.ins.AddLog("Wyszukiwanie podgry metodą graficzną.", EColors.Yellow);
            yield return Game.Wait();

            if (Game.gameData.matrix.GetLength(0) == 2 && Game.gameData.matrix.GetLength(1) > 2) // Dla 2xm
            {
                LogsManager.ins.AddLog(string.Format("{0} ma dokładnie dwie strategie.", Game.gameData.rowPlayer));
                id = IdTheSmalestVal();
                yield return CrossOverMin(id);

                subGame[0, 1] = Game.gameData.matrix[0, point.strategyA_ID];
                subGame[1, 1] = Game.gameData.matrix[1, point.strategyA_ID];

                subGame[0, 0] = Game.gameData.matrix[0, point.strategyB_ID];
                subGame[1, 0] = Game.gameData.matrix[1, point.strategyB_ID];

                string[] useingStrategies = new string[2] { Game.gameData.columnStrategies[point.strategyA_ID],
                                                            Game.gameData.columnStrategies[point.strategyB_ID]};
                Game.gameData.columnStrategies = useingStrategies.ToList();

            }
            else if (Game.gameData.matrix.GetLength(1) == 2 && Game.gameData.matrix.GetLength(0) > 2)    // Dla nx2
            {
                LogsManager.ins.AddLog(string.Format("{0} ma dokładnie dwie strategie.", Game.gameData.columnPlayer));
                LogsManager.ins.AddLog("Wyszukiwanie strategii dającej największą przegraną dla gracza");
                yield return Game.Wait();
                id = IDTheLargestVal();

                yield return CrossOverMax(id);

                subGame[1, 0] = Game.gameData.matrix[point.strategyA_ID, 0];
                subGame[1, 1] = Game.gameData.matrix[point.strategyA_ID, 1];

                subGame[0, 0] = Game.gameData.matrix[point.strategyB_ID, 0];
                subGame[0, 1] = Game.gameData.matrix[point.strategyB_ID, 1];

                string[] useingStrategies = new string[2] { Game.gameData.rowStrategies[point.strategyA_ID],
                                                            Game.gameData.rowStrategies[point.strategyB_ID]};
                Game.gameData.rowStrategies = useingStrategies.ToList();
            }

            Game.gameData.matrix = subGame;
        }

        private static IEnumerator CrossOverMax(byte idLargest, double max = double.MaxValue)
        {
            LogsManager.ins.AddLog("Strategia znaleziona: " + Game.gameData.rowStrategies[idLargest]);
            GraphicalManager.ins.SelectLine(idLargest, EColors.Red, true);
            yield return Game.Wait();

            byte newID = 0;

            Vector strategyA_start = new Vector(Game.gameData.matrix[idLargest, 0], 0);
            Vector strategyA_end = new Vector(Game.gameData.matrix[idLargest, 1], 1);

            if (GraphicalManager.ins.highLine.Points.Length == 0)
                GraphicalManager.ins.SetHighLinePoint(new Vector2((float)strategyA_start.X, (float)strategyA_start.Y));

            Vector strategyB_start, strategyB_end;

            Vector maxIntersaction = new Vector(double.MinValue, 0);
            Vector intersection;

            for (byte i = 0; i < Game.gameData.matrix.GetLength(0); i++)
            {
                if (strategyA_start.X < strategyA_end.X) break;

                strategyB_start = new Vector(Game.gameData.matrix[i, 0], 0);
                strategyB_end = new Vector(Game.gameData.matrix[i, 1], 1);

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
                yield return CrossOverMax(newID, maxIntersaction.X);
            }
            else
            {
                GraphicalManager.ins.SetHighLinePoint(new Vector2((float)strategyA_end.X, (float)strategyA_end.Y));
                GraphicalManager.ins.SelectLine(0, EColors.Default, true);
            }
        }

        private static IEnumerator CrossOverMin(byte idSmallest, double min = double.MinValue)
        {
            LogsManager.ins.AddLog("Strategia znaleziona: " + Game.gameData.columnStrategies[idSmallest]);
            GraphicalManager.ins.SelectLine(idSmallest, EColors.Red, true);
            yield return Game.Wait();

            byte newID = 0;

            Vector strategyA_start = new Vector(0, Game.gameData.matrix[0, idSmallest]);
            Vector strategyA_end = new Vector(1, Game.gameData.matrix[1, idSmallest]);

            if (GraphicalManager.ins.highLine.Points.Length == 0)
                GraphicalManager.ins.SetHighLinePoint(new Vector2((float)strategyA_start.Y, (float)strategyA_start.X));

            Vector strategyB_start, strategyB_end;

            Vector minIntersaction = new Vector(0, double.MaxValue);
            Vector intersaction;


            for (byte i = 0; i < Game.gameData.matrix.GetLength(1); i++)
            {

                if (strategyA_start.Y > strategyA_end.Y) break;

                strategyB_start = new Vector(0, Game.gameData.matrix[0, i]);
                strategyB_end = new Vector(1, Game.gameData.matrix[1, i]);

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
                yield return CrossOverMin(newID, minIntersaction.Y);
            }
            else
            {
                GraphicalManager.ins.SetHighLinePoint(new Vector2((float)strategyA_end.Y, (float)strategyA_end.X));
                GraphicalManager.ins.SelectLine(0, EColors.Default, true);
            }
        }

        /// <summary>
        /// Funkcja zwraca ID największej wypłaty w macierzy
        /// </summary>
        /// <returns></returns>
        private static byte IDTheLargestVal()
        {
            byte id = 0;
            sbyte theLargest = sbyte.MinValue;
            sbyte curVal;

            for (byte i = 0; i < Game.gameData.matrix.GetLength(0); i++)
            {
                curVal = Game.gameData.matrix[i, 0];

                if (curVal > theLargest)
                {
                    theLargest = curVal;
                    id = i;
                }
            }

            return id;
        }

        /// <summary>
        /// Funkcja zwraca ID najmniejszej wypłaty w macierzy
        /// </summary>
        /// <returns></returns>
        private static byte IdTheSmalestVal()
        {
            byte id = 0;
            sbyte theSmallest = sbyte.MaxValue;
            sbyte curVal;

            for (byte i = 0; i < Game.gameData.matrix.GetLength(1); i++)
            {
                curVal = Game.gameData.matrix[0, i];

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


