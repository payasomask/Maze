using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
  public MazeManager(int rows,int columns) {
    max_columns = columns;
    max_rows = rows;
  }
  public static MazeManager _MazeManager = null;
  //這邊盡量是維持在768/1024的比例
  //columns : row -> 3 : 4的
  private int max_columns = 4;
  private int max_rows = 4;
  [HideInInspector]
  public float cell_size = 1;

  private Maze mMazeSpawn = null;
  private MazePlayerController playercontroller = null;
  private MazeGoalController goalcontroller = null;

  private void Awake()
  {
    
  }

  // Start is called before the first frame update
  void Start(){
    }

  private void Update()
  {
    if (Input.GetKeyUp(KeyCode.R)){
      resetMaze();
    }
  }

  public void init(){

    getCellSize();

    if(mMazeSpawn == null)
      mMazeSpawn = new PrimsMaze(max_rows, max_columns, cell_size);

    mMazeSpawn.BuildMaze();

    UtilityHelper.MazeCorner StartLocation = (UtilityHelper.MazeCorner)UtilityHelper.Random(0, (int)UtilityHelper.MazeCorner.SZ);
    Vector2 PlayerStartLocation = UtilityHelper.GetMazeCorner(StartLocation, max_rows, max_columns);
    Vector2 StartPoint = mMazeSpawn.GetCellPosition((int)PlayerStartLocation.x, (int)PlayerStartLocation.y);
    mMazeSpawn.GetCell((int)PlayerStartLocation.x, (int)PlayerStartLocation.y).Type = CellType.Start;

    playercontroller = Instantiate(AssetbundleLoader._AssetbundleLoader.InstantiatePrefab("MazePlayer"), StartPoint, Quaternion.identity).GetComponent<MazePlayerController>();
    playercontroller.init((int)PlayerStartLocation.x, (int)PlayerStartLocation.y, cell_size);

    //float mask_size = 3.0f;
    //CanvasMaskManager._MazeMaskManager.init(max_columns * cell_size, max_rows * cell_size, cell_size * mask_size);
    //CanvasMaskManager._MazeMaskManager.updateMaskPosion(StartPoint);

    //隨機結束點
    Vector2 GoalStartLoaction = UtilityHelper.GetDiagonalLocation((int)PlayerStartLocation.x, (int)PlayerStartLocation.y, max_rows, max_columns);
    Vector2 GoalPoint = mMazeSpawn.GetCellPosition((int)GoalStartLoaction.x, (int)GoalStartLoaction.y);
    mMazeSpawn.GetCell((int)GoalStartLoaction.x, (int)GoalStartLoaction.y).Type = CellType.Goal;

    goalcontroller = Instantiate(AssetbundleLoader._AssetbundleLoader.InstantiatePrefab("MazeGoal"), GoalPoint, Quaternion.identity).GetComponent<MazeGoalController>();
    goalcontroller.init((int)GoalStartLoaction.x, (int)GoalStartLoaction.y, cell_size);

    MaskManager._MaskManager.AddBlack("black",Vector2.zero, new Vector2(max_columns * cell_size, max_rows * cell_size));
  }

  void getCellSize(){

    cell_size = 768.0f / max_columns;
    Debug.Log("寬 : " + cell_size * max_columns);
    Debug.Log("高 : " + cell_size * max_rows);
    Debug.Log("cell_size : " + cell_size);

  }


  public Maze GetMaze(){
    return mMazeSpawn;
  }

  public void resetMaze(){

    if (mMazeSpawn == null)
      return;

    GameObject.Destroy(playercontroller.gameObject);
    Destroy(goalcontroller.gameObject);
    mMazeSpawn.ResetMaze();
    TorchManager._TorchManager.ClearAllTorch();
    MaskManager._MaskManager.ClearAllMask();
    init();
  }

    
}
