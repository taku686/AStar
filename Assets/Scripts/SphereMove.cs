using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class SphereMove : MonoBehaviour
{
    [SerializeField] int _column_Count;
    [SerializeField] int _row_Count;
    //    [SerializeField] AStarGrid _grid = null;
    Vector3Int _startPos;
    [SerializeField] Vector3 _goalPos;
    private AStar _aStar;
    private List<AStarGrid.Cell> _shortestWay = new List<AStarGrid.Cell>();
    List<Vector3> _path = new List<Vector3>();
    [SerializeField] GameObject m_obj_Bomb;
    private float m_f_Time;

    private void OnEnable()
    {
        _startPos = new Vector3Int((int)transform.position.x, 0, (int)transform.position.z);

    }
    private void Start()
    {
        _aStar = new AStar(GridManager._grid);

        transform.DOPath(GetShortestPass(transform.position, _goalPos), 3);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(m_obj_Bomb, new Vector3((int)transform.position.x, transform.position.y, (int)transform.position.z), Quaternion.identity);
        }
        m_f_Time += Time.deltaTime;
        if (m_f_Time > 4)
        {
            m_f_Time = 0;
            Evade();
        }

    }

    private void Evade()
    {
        if (GridManager._grid.GetCell((int)transform.position.x, (int)transform.position.z).groundType == AStarGrid.GroundType.Bomb || GridManager._grid.GetCell((int)transform.position.x, (int)transform.position.z).groundType == AStarGrid.GroundType.DangerZone)
        {
            foreach (var item in GridManager._grid.Cells.Where(item => item.groundType == AStarGrid.GroundType.Road))
            {
                float distance = 1000;
                if (distance > (Mathf.Pow((item.coord.x - transform.position.x), 2) + Mathf.Pow((item.coord.y - transform.position.z), 2)))
                {
                    distance = (Mathf.Pow((item.coord.x - transform.position.x), 2) + Mathf.Pow((item.coord.y - transform.position.z), 2));
                    GridManager._grid.GoalCell = item;
                }
            }
            transform.DOPath(GetShortestPass(transform.position, new Vector3(GridManager._grid.GoalCell.coord.x, 0, GridManager._grid.GoalCell.coord.y)), 3);
        }
    }

    private Vector3[] GetShortestPass(Vector3 startPos, Vector3 goalPos)
    {
        _shortestWay = _aStar.GetShortestWay(GridManager._grid.GetCell((int)startPos.x, (int)startPos.z), GridManager._grid.GetCell((int)goalPos.x, (int)goalPos.z));
        foreach (var item in _shortestWay)
        {
            _path.Add(new Vector3(item.coord.x, 0.5f, item.coord.y));
        }
        _path.Reverse();
        return _path.ToArray();
    }
}
