#if USE_GRID_SYSTEM
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public static class VP_GridAStar
    {
        /// <summary>
        /// Returns the best path as a List of Nodes
        /// </summary>
        public static List<VP_GridTile> Search(VP_GridTile start, VP_GridTile goal)
        {
            // Start is goal
            if (start == goal)
            {
                return null;
            }

            Dictionary<VP_GridTile, VP_GridTile> came_from = new Dictionary<VP_GridTile, VP_GridTile>();
            Dictionary<VP_GridTile, float> cost_so_far = new Dictionary<VP_GridTile, float>();

            List<VP_GridTile> path = new List<VP_GridTile>();

            SimplePriorityQueue<VP_GridTile> frontier = new SimplePriorityQueue<VP_GridTile>();
            frontier.Enqueue(start, 0);

            came_from.Add(start, start);
            cost_so_far.Add(start, 0);

            VP_GridTile current = start;
            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                if (current == goal) break; // Early exit

                foreach (VP_GridTile next in VP_GridManager.Instance.WalkableNeighbors(current, false, false, goal))
                {
                    float new_cost = cost_so_far[current] + next.Cost();
                    if (!cost_so_far.ContainsKey(next) || new_cost < cost_so_far[next])
                    {
                        cost_so_far[next] = new_cost;
                        came_from[next] = current;
                        float priority = new_cost + VP_GridUtilities.Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        next.Priority = new_cost;
                    }
                }
            }

            while (current && current != start)
            {
                path.Add(current);
                current = came_from[current];
            }
            path.Reverse();

            // If we were not able to reach the destination target tile just return an empty/null path
            //if (!path.Contains(goal))
            //    path.Clear();

            return path;
        }

        /// <summary>
        /// Returns the best path as a List of Nodes
        /// </summary>
        public static List<VP_GridTile> Search(VP_GridTile start, Vector2Int goalPosition)
        {
            // TODO
            var goalTile = VP_GridManager.Instance.GetGridTileAtPosition(goalPosition, start.TileLayer);
            return goalTile != null ? Search(start, goalTile) : null;
        }
    }
}
#endif