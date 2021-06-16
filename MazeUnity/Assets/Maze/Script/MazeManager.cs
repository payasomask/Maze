using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
  public static MazeManager _MazeManager = null;
  //這邊盡量是維持在768/1024的比例
  //columns : row -> 3 : 4的
  [SerializeField]
  private int max_columns = 4;
  [SerializeField]
  private int max_rows = 4;
  [HideInInspector]
  public float cell_size = 1;

  [SerializeField]
  private GameObject player;
  [SerializeField]
  private GameObject goal;

  private Maze mMazeSpawn = null;
  private MazePlayerController playercontroller = null;
  private MazeGoalController goalcontroller = null;

  private void Awake()
  {
    _MazeManager = this;
  }

  // Start is called before the first frame update
  void Start(){
    init();
    }

  void init(){

    getCellSize();

    mMazeSpawn = new PrimsMaze(max_rows, max_columns, cell_size);
    mMazeSpawn.BuildMaze();

    //隨機開始點
    int startx = UtilityHelper.Random(0, max_rows);
    int starty = UtilityHelper.Random(0, max_columns);
    Vector2 StartPoint = mMazeSpawn.GetCellPosition(startx, starty);
    mMazeSpawn.GetCell(startx, starty).Type = CellType.Start;

    playercontroller = Instantiate(player, StartPoint, Quaternion.identity).GetComponent<MazePlayerController>();
    playercontroller.init(startx, starty, cell_size);

    //float mask_size = 3.0f;
    //CanvasMaskManager._MazeMaskManager.init(max_columns * cell_size, max_rows * cell_size, cell_size * mask_size);
    //CanvasMaskManager._MazeMaskManager.updateMaskPosion(StartPoint);

    //隨機結束點
    startx = UtilityHelper.Random(0, max_rows);
    starty = UtilityHelper.Random(0, max_columns);
    StartPoint = mMazeSpawn.GetCellPosition(startx, starty);
    mMazeSpawn.GetCell(startx, starty).Type = CellType.Goal;

    goalcontroller = Instantiate(goal, StartPoint, Quaternion.identity).GetComponent<MazeGoalController>();
    goalcontroller.init(startx, starty, cell_size);

    MaskManager._MaskManager.AddBlack("black",Vector2.zero, new Vector2(max_columns * cell_size, max_rows * cell_size));
  }

  void getCellSize(){

    cell_size = 768.0f / max_columns;
    Debug.Log("寬 : " + cell_size * max_columns);
    Debug.Log("高 : " + cell_size * max_rows);
  }


  public Maze GetMaze(){
    return mMazeSpawn;
  }
    
}
