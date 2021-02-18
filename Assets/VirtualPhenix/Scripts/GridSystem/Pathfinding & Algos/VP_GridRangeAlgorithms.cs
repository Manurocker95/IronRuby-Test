#if USE_GRID_SYSTEM
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace VirtualPhenix.GridEngine
{
    public static class VP_GridRangeAlgorithms
    {
        public static List<VP_GridTile> SearchByParameters(VP_GridTile start, VP_GridRangeParameters rangeParameters)
        {
            switch (rangeParameters.m_RangeSearchType)
            {
                case RangeSearchType.RectangleByGridPosition:
                default:
                    return VP_GridRangeAlgorithms.SearchByGridPosition(start, rangeParameters.m_MaxReach, rangeParameters.m_WalkableTilesOnly, rangeParameters.m_UnOccupiedTilesOnly, rangeParameters.m_SquareRange, rangeParameters.m_IgnoreTilesHeight, rangeParameters.m_IncludeStartingTile, rangeParameters.m_MinimunReach);
                case RangeSearchType.RectangleByMovement:
                    return VP_GridRangeAlgorithms.SearchByMovement(start, rangeParameters.m_MaxReach, rangeParameters.m_IgnoreTilesHeight, rangeParameters.m_IncludeStartingTile, rangeParameters.m_MinimunReach);
                case RangeSearchType.HexagonByGridPosition:
                    return VP_GridRangeAlgorithms.HexagonSearchByGridPosition(start, rangeParameters.m_MaxReach, rangeParameters.m_WalkableTilesOnly, rangeParameters.m_UnOccupiedTilesOnly, rangeParameters.m_IgnoreTilesHeight, rangeParameters.m_IncludeStartingTile, rangeParameters.m_MinimunReach);
                case RangeSearchType.HexagonByMovement:
                    return VP_GridRangeAlgorithms.HexagonSearchByMovement(start, rangeParameters.m_MaxReach, rangeParameters.m_IgnoreTilesHeight, rangeParameters.m_IncludeStartingTile, rangeParameters.m_MinimunReach);
            }
        }

        public static List<VP_GridTile> SearchByGridPosition(VP_GridTile start, int maxReach, bool WalkableTilesOnly = true, bool unoccupiedTilesOnly = true, bool square = true, bool ignoreHeight = false, bool includeStartingTile = false, int MinReach = 1)
        {
            List<VP_GridTile> range = new List<VP_GridTile>();

            // Start is goal
            if (maxReach <= 0)
            {
                range.Add(start);
                return range;
            }
            if (maxReach < MinReach)
            {
                return range;
            }

            Dictionary<VP_GridTile, float> cost_so_far = new Dictionary<VP_GridTile, float>();
            SimplePriorityQueue<VP_GridTile> frontier = new SimplePriorityQueue<VP_GridTile>();
            frontier.Enqueue(start, 0);
            cost_so_far.Add(start, 0);

            VP_GridTile current = start;
            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                if (cost_so_far[current] <= maxReach)
                {
                    var neighbors = WalkableTilesOnly == true ? VP_GridManager.Instance.WalkableNeighbors(current, ignoreHeight, unoccupiedTilesOnly, null, VP_GridManager.defaultRectangle8Directions) : VP_GridManager.Instance.Neighbors(current, ignoreHeight, VP_GridManager.defaultRectangle8Directions);
                    foreach (VP_GridTile next in neighbors)
                    {
                        float new_cost = cost_so_far[current] + (square == true ? 1 : VP_GridUtilities.Heuristic(current, next));
                        if (!cost_so_far.ContainsKey(next))
                        {
                            cost_so_far[next] = new_cost;
                            float priority = new_cost;
                            frontier.Enqueue(next, priority);

                            if (!range.Contains(next) && new_cost >= MinReach && new_cost <= maxReach)
                            {
                                range.Add(next);
                            }
                        }
                    }
                }
            }

            // remove the starting tile if required
            if (!includeStartingTile)
            {
                if (range.Contains(start))
                {
                    range.Remove(start);
                }
            }

            return range;
        }


        public static List<VP_GridTile> SearchByMovement(VP_GridTile start, int maxReach, bool ignoreHeight = false, bool includeStartingTile = false, int MinReach = 1)
        {
            List<VP_GridTile> range = new List<VP_GridTile>();

            // Start is goal
            if (maxReach == 0)
            {
                range.Add(start);
                return range;
            }
            if (maxReach < MinReach)
            {
                return range;
            }

            Dictionary<VP_GridTile, float> cost_so_far = new Dictionary<VP_GridTile, float>();
            SimplePriorityQueue<VP_GridTile> frontier = new SimplePriorityQueue<VP_GridTile>();
            frontier.Enqueue(start, 0);
            cost_so_far.Add(start, 0);

            VP_GridTile current = start;
            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                if (cost_so_far[current] <= maxReach)
                {
                    foreach (VP_GridTile next in VP_GridManager.Instance.WalkableNeighbors(current, ignoreHeight))
                    {
                        float new_cost = cost_so_far[current] + next.Cost();
                        if (!cost_so_far.ContainsKey(next))
                        {
                            cost_so_far[next] = new_cost;
                            float priority = new_cost;
                            frontier.Enqueue(next, priority);

                            if (!range.Contains(next) && new_cost >= MinReach && new_cost <= maxReach)
                            {
                                range.Add(next);
                            }
                        }
                    }
                }
            }

            // remove the starting tile if required
            if (!includeStartingTile)
            {
                if (range.Contains(start))
                {
                    range.Remove(start);
                }
            }

            return range;
        }

        public static List<VP_GridTile> HexagonSearchByGridPosition(VP_GridTile start, int maxReach, bool WalkableTilesOnly = true, bool unoccupiedTilesOnly = true, bool ignoreHeight = false, bool includeStartingTile = false, int MinReach = 1)
        {
            List<VP_GridTile> range = new List<VP_GridTile>();

            // Start is goal
            if (maxReach == 0)
            {
                range.Add(start);
                return range;
            }
            if (maxReach < MinReach)
            {
                return range;
            }

            Dictionary<VP_GridTile, float> cost_so_far = new Dictionary<VP_GridTile, float>();
            SimplePriorityQueue<VP_GridTile> frontier = new SimplePriorityQueue<VP_GridTile>();
            frontier.Enqueue(start, 0);
            cost_so_far.Add(start, 0);

            VP_GridTile current = start;
            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                if (cost_so_far[current] <= maxReach)
                {
                    var neighbors = WalkableTilesOnly == true ? VP_GridManager.Instance.WalkableNeighbors(current, ignoreHeight, unoccupiedTilesOnly, null) : VP_GridManager.Instance.Neighbors(current, ignoreHeight);
                    foreach (VP_GridTile next in neighbors)
                    {
                        float new_cost = VP_GridUtilities.HexDistance(next, start);
                        if (!cost_so_far.ContainsKey(next))
                        {
                            cost_so_far[next] = new_cost;
                            float priority = new_cost;
                            frontier.Enqueue(next, priority);

                            if (!range.Contains(next) && new_cost >= MinReach && new_cost <= maxReach)
                            {
                                range.Add(next);
                            }
                        }
                    }
                }
            }

            // remove the starting tile if required
            if (!includeStartingTile)
            {
                if (range.Contains(start))
                {
                    range.Remove(start);
                }
            }

            return range;
        }

        public static List<VP_GridTile> HexagonSearchByMovement(VP_GridTile start, int maxReach, bool ignoreHeight = false, bool includeStartingTile = false, int MinReach = 1)
        {
            List<VP_GridTile> range = new List<VP_GridTile>();

            // Start is goal
            if (maxReach == 0)
            {
                range.Add(start);
                return range;
            }
            if (maxReach < MinReach)
            {
                return range;
            }

            Dictionary<VP_GridTile, float> cost_so_far = new Dictionary<VP_GridTile, float>();
            SimplePriorityQueue<VP_GridTile> frontier = new SimplePriorityQueue<VP_GridTile>();
            frontier.Enqueue(start, 0);
            cost_so_far.Add(start, 0);

            VP_GridTile current = start;
            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();
                if (cost_so_far[current] <= maxReach)
                {
                    foreach (VP_GridTile next in VP_GridManager.Instance.WalkableNeighbors(current, ignoreHeight, true, null))
                    {
                        float new_cost = cost_so_far[current] + next.Cost();
                        if (!cost_so_far.ContainsKey(next))
                        {
                            cost_so_far[next] = new_cost;
                            float priority = new_cost;
                            frontier.Enqueue(next, priority);

                            if (!range.Contains(next) && new_cost >= MinReach && new_cost <= maxReach)
                            {
                                range.Add(next);
                            }
                        }
                    }
                }
            }

            // remove the starting tile if required
            if (!includeStartingTile)
            {
                if (range.Contains(start))
                {
                    range.Remove(start);
                }
            }

            return range;
        }
    }
}
#endif