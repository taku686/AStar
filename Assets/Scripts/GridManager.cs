using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridManager : MonoBehaviour
{
    [SerializeField] int _column_Count;//13
    [SerializeField] int _row_Count;//14
    public static AStarGrid _grid = null;
    private float m_f_Time;
    private void Awake()
    {
        _grid = new AStarGrid(_row_Count, _column_Count);
        for (int i = 0; i < _column_Count; i++)
        {
            for (int j = 0; j < _row_Count; j++)
            {
                _grid.GetCell(j, i).groundType = AStarGrid.GroundType.Road;
            }
        }
        foreach (var item in GameObject.FindGameObjectsWithTag("Wall").Select(item => item.transform.localPosition))
        {
            _grid.GetCell((int)item.x, (int)item.z).groundType = AStarGrid.GroundType.Wall;
        }
        
    }
  
    // Update is called once per frame
    void Update()
    {
        m_f_Time += Time.deltaTime;
        if(m_f_Time > 1)
        {
            m_f_Time = 0;
            UpdateGrid();
        }
    }

    private void UpdateGrid()
    {
        foreach (var item in _grid.Cells.Where(item => item.groundType != AStarGrid.GroundType.Wall))
        {
            item.groundType = AStarGrid.GroundType.Road;
        }

        foreach (var item in GameObject.FindGameObjectsWithTag("Bomb").Select(item => item.transform.localPosition))
        {
            _grid.GetCell((int)item.x, (int)item.z).groundType = AStarGrid.GroundType.Bomb;
            for (int i = 1; i < 3; i++)
            {
                if (_grid.GetCell((int)item.x + i, (int)item.z).groundType != AStarGrid.GroundType.Wall)
                {
                    _grid.GetCell((int)item.x + i, (int)item.z).groundType = AStarGrid.GroundType.DangerZone;
                }
                if (_grid.GetCell((int)item.x - i, (int)item.z).groundType != AStarGrid.GroundType.Wall)
                {
                    _grid.GetCell((int)item.x - i, (int)item.z).groundType = AStarGrid.GroundType.DangerZone;
                }
                if (_grid.GetCell((int)item.x, (int)item.z + i).groundType != AStarGrid.GroundType.Wall)
                {
                    _grid.GetCell((int)item.x, (int)item.z + i).groundType = AStarGrid.GroundType.DangerZone;
                }
                if (_grid.GetCell((int)item.x, (int)item.z - i).groundType != AStarGrid.GroundType.Wall)
                {
                    _grid.GetCell((int)item.x, (int)item.z - i).groundType = AStarGrid.GroundType.DangerZone;
                }
            }
        }
    }
}
