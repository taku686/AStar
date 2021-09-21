using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStar : MonoBehaviour
{
    private class AStarInfo
    {
        public AStarGrid.Cell cell;
        public AStarInfo previous;
        public float step;
        public float distance;
        public float Weight { get { return step + distance; } }
    }

    private AStarGrid _grid;

    public AStar(AStarGrid grid)
    {
        _grid = grid;
    }

    /// <summary>
    /// 最短距離を取得する
    /// StartとGoalのセルを含む
    /// </summary>
    public List<AStarGrid.Cell> GetShortestWay(AStarGrid.Cell StartCell, AStarGrid.Cell GoalCell)
    {
        var res = new List<AStarGrid.Cell>();

        var passedCells = new List<AStarGrid.Cell>();
        var recentTargets = new List<AStarInfo>();
        passedCells.Add(StartCell);
        recentTargets.Add(GetAStarInfo(StartCell, GoalCell, null));
        AStarInfo goalInfo = null;

        while (true)
        {

            // recentTargetsのうちweightが最も低いものを計算対象とする
            var currentTarget = recentTargets
                .OrderBy(info => info.Weight)
                .FirstOrDefault();

            // ターゲットの隣接セルのAStarInfoを取得する
            var adjacentInfos = _grid.GetAdjacences(currentTarget.cell.coord.x, currentTarget.cell.coord.y)
                .Where(cell => {
                    // タイプが道でもなくゴールのセルでもない場合は対象外
                    if (cell.groundType != AStarGrid.GroundType.Road && cell != GoalCell)
                    {
                        return false;
                    }
                    // 計算済みのセルは対象外
                    if (passedCells.Contains(cell))
                    {
                        return false;
                    }
                    return true;
                })
                .Select(cell => GetAStarInfo(cell, GoalCell, currentTarget))
                .ToList();

            // recentTargetsとpassedCellsを更新
            recentTargets.Remove(currentTarget);
            recentTargets.AddRange(adjacentInfos);
            passedCells.Add(currentTarget.cell);

            // ゴールが含まれていたらそこで終了
            goalInfo = adjacentInfos.FirstOrDefault(info => info.cell == GoalCell);
            if (goalInfo != null)
            {
                break;
            }
            // recentTargetsがゼロだったら行き止まりなので終了
            if (recentTargets.Count == 0)
            {
                break;
            }
        }

        // ゴールが結果に含まれていない場合は最短経路が見つからなかった
        if (goalInfo == null)
        {
            return res;
        }

        // Previousを辿ってセルのリストを作成する
        res.Add(goalInfo.cell);
        AStarInfo current = goalInfo;
        while (true)
        {
            if (current.previous != null)
            {
                res.Add(current.previous.cell);
                current = current.previous;
            }
            else
            {
                break;
            }
        }
        return res;
    }

    private AStarInfo GetAStarInfo(AStarGrid.Cell targetCell, AStarGrid.Cell goalCell, AStarInfo previousInfo)
    {
        var result = new AStarInfo();
        result.cell = targetCell;
        result.previous = previousInfo;
        result.step = previousInfo == null ? 0 : previousInfo.step + 1;
        result.distance = (goalCell.coord - targetCell.coord).Magnitude;
        return result;
    }
}
