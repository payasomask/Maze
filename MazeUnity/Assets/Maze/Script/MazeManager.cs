using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
  public static MazeManager _MazeManager = null;
  private Maze mMazeSpawn = null;
  private MazePlayerController playercontroller = null;
  private MazeGoalController goalcontroller = null;
  private MazeBoxController boxcontroller = null;
  private int maze_rows, maze_columns;
  private float maze_cellsize;
  private MazeScene mMS = null;

  private void Awake()
  {
    _MazeManager = this;
  }

  // Start is called before the first frame update
  void Start(){
    }

  private void Update()
  {

  }

  public void Init(MazeScene ms){
    mMS = ms;
  }
  //這邊盡量是維持在768/1024的比例
  public void CreatMaze(int rows,int columns){

    maze_rows = rows;
    maze_columns = columns;

    maze_cellsize = getCellSize(maze_rows, maze_columns);


    Vector2 maze_pivot = UtilityHelper.MazePoint(maze_columns * maze_cellsize, maze_rows * maze_cellsize);
    //讓迷宮的上緣貼著TopUI的下緣
    //透過(下緣位置 - 整個迷宮的一半高度) = 需要調整的Y，重新計算pivot.y
    //最終迷宮位置的調整 cell - maze_pivot，這裡必須調整成這樣
    maze_pivot.y = maze_pivot.y - (mMS.GetMazeTopUIBottom() - maze_pivot.y);
    if (mMazeSpawn == null)
      mMazeSpawn = new PrimsMaze(maze_rows, maze_columns, maze_cellsize, maze_pivot);

    mMazeSpawn.BuildMaze();

    UtilityHelper.MazeCorner StartLocation = (UtilityHelper.MazeCorner)UtilityHelper.Random(0, (int)UtilityHelper.MazeCorner.SZ);
    Vector2 PlayerStartLocation = UtilityHelper.GetMazeCorner(StartLocation, maze_rows, maze_columns);
    Vector2 StartPoint = mMazeSpawn.GetCellPosition((int)PlayerStartLocation.x, (int)PlayerStartLocation.y);
    mMazeSpawn.GetCell((int)PlayerStartLocation.x, (int)PlayerStartLocation.y).Type = CellType.Start;

    GameObject player_go =instantiateObject(gameObject,"MazePlayer");
    player_go.transform.localPosition = StartPoint;
    playercontroller = player_go.GetComponent<MazePlayerController>();
    playercontroller.init((int)PlayerStartLocation.x, (int)PlayerStartLocation.y, maze_cellsize);

    //float mask_size = 3.0f;
    //CanvasMaskManager._MazeMaskManager.init(max_columns * cell_size, max_rows * cell_size, cell_size * mask_size);
    //CanvasMaskManager._MazeMaskManager.updateMaskPosion(StartPoint);

    //隨機結束點
    Vector2 GoalStartLoaction = UtilityHelper.GetDiagonalLocation((int)PlayerStartLocation.x, (int)PlayerStartLocation.y, maze_rows, maze_columns);
    Vector2 GoalPoint = mMazeSpawn.GetCellPosition((int)GoalStartLoaction.x, (int)GoalStartLoaction.y);
    mMazeSpawn.GetCell((int)GoalStartLoaction.x, (int)GoalStartLoaction.y).Type = CellType.Goal;

    GameObject goal_go = instantiateObject(gameObject, "MazeGoal");
    goal_go.transform.localPosition = GoalPoint;
    goalcontroller = goal_go.GetComponent<MazeGoalController>();
    goalcontroller.init((int)GoalStartLoaction.x, (int)GoalStartLoaction.y, maze_cellsize);

    //隨機寶箱
    //if (UtilityHelper.Random(0, 10) >= 0){
    if (UtilityHelper.Random(0, 10) < 3){

      //寶相的位置先暫定是角落好了，最不會有問題
      //起點跟終點都在角落所以我直接隨機一個小一圈的範圍就好了


      //取得角落寶相的位置
      //UtilityHelper.MazeCorner[] boxcorners = UtilityHelper.GetMazeCorners(StartLocation, maze_rows, maze_columns);
      //UtilityHelper.MazeCorner boxcorner = boxcorners[UtilityHelper.Random(0, boxcorners.Length)];
      //Vector2 boxlocation = UtilityHelper.GetMazeCorner(boxcorner, maze_rows, maze_columns);

      Vector2 boxlocation = new Vector2(UtilityHelper.Random(1, maze_rows - 1), UtilityHelper.Random(1, maze_columns - 1));


      //加入寶箱
      Vector2 BoxPoint = mMazeSpawn.GetCellPosition((int)boxlocation.x, (int)boxlocation.y);
      mMazeSpawn.GetCell((int)boxlocation.x, (int)boxlocation.y).Type = CellType.Box;

      GameObject box_go = instantiateObject(gameObject, "MazeBox");
      box_go.transform.localPosition = BoxPoint;
      boxcontroller = box_go.GetComponent<MazeBoxController>();
      boxcontroller.init((int)boxlocation.x, (int)boxlocation.y, maze_cellsize);
    }


    //MaskManager._MaskManager.AddBlack("black",Vector2.zero, new Vector2(maze_columns * maze_cellsize, maze_rows * maze_cellsize));
    MaskManager._MaskManager.HideMask("box");

    //FloorManager._FloorManager.init();
    //FloorManager._FloorManager.updateSize(maze_cellsize * maze_columns, maze_cellsize * maze_rows);
  }

 float getCellSize(int rows,int columns){

    float size = 768.0f / columns;
    Debug.Log("寬 : " + size * columns);
    Debug.Log("高 : " + size * rows);
    Debug.Log("cell_size : " + size);
    return size;
  }

  public float getCellSize(){
    return maze_cellsize;
  }



  public Maze GetMaze(){
    return mMazeSpawn;
  }

  public void ClearMaze(){

    if (mMazeSpawn == null)
      return;

    GameObject.Destroy(playercontroller.gameObject);
    if(goalcontroller != null)
      Destroy(goalcontroller.gameObject);
    if (boxcontroller != null)
      Destroy(boxcontroller.gameObject);
    mMazeSpawn.ResetMaze();
    TorchManager._TorchManager.ClearAllTorch();
    MaskManager._MaskManager.ClearAllMask();
  }

  public void movePlayer(Dir movedir) {
    if (playercontroller == null)
      return;
    playercontroller.moveDir(movedir);
  }

  public Vector2 PlayerPosition(){
    return playercontroller.position();
  }

  public float PlayerMaskScale(){
    return playercontroller.maskScale();
  }

  public bool IsPlayerMoving(){
    if (playercontroller == null)
      return false;
    return playercontroller.IsMoving();
  }

  public void ArrivalCell(string who, Cell cell) {
    if (mMS == null)
      return;
    mMS.ArrivalCell(who, cell);

    if (cell.Type == CellType.Box) {
      cell.Type = CellType.Road;
      boxcontroller.gameObject.SetActive(false);
    } else if (cell.Type == CellType.Goal){
      cell.Type = CellType.Road;
      goalcontroller.gameObject.SetActive(false);
    }
  }

  GameObject instantiateObject(GameObject parent, string name)
  {
    GameObject g = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab(name);
    g.transform.SetParent(parent.transform, true);

    return g;
  }


}
