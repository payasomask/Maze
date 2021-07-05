using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePlayerController : MonoBehaviour
{
  [SerializeField]
  private float basic_speed = 10.0f;
  private float maze_size;
  //玩家火光範圍
  float maskscale = 3.0f;

  private int currentx, currenty;
  private List<Cell> movepath = null;
  private List<Cell> trackpath = new List<Cell>();
  private LineRenderer LineRenderer = null;
  private int maskid;
  enum MoveState
  {
    Arrival,
    Moving,
  }
  enum MoveType
  {
    New,
    Goback,
  }
  MoveType mcurrentType = MoveType.New;
  MoveState mcurrentState = MoveState.Arrival;
    // Update is called once per frame
    void Update()
    {

    move();
    updateTrackLine();

  }

  public void init(int currentx, int currenty, float maze_size) {
    this.maze_size = maze_size;
    this.currentx = currentx;
    this.currenty = currenty;

    //感覺3倍的maze_size比較舒服

    //gameObject.transform.localScale = new Vector3(maze_size, maze_size, 1.0f);
    GameObject icon_go = transform.Find("Icon").gameObject;
    Sprite icon = icon_go.GetComponent<SpriteRenderer>().sprite;
    if (icon != null){
      float iconscale = maze_size / icon.bounds.size.x;//根據圖資重新計算scale大小
      icon_go.transform.localScale = new Vector3(iconscale, iconscale, 0.0f);
    }

    LineRenderer = gameObject.GetComponent<LineRenderer>();
    LineRenderer.startWidth = maze_size * 0.1f;
    LineRenderer.endWidth = maze_size * 0.1f;

    Cell StartCell = MazeManager._MazeManager.GetMaze().GetCell(currentx, currenty);
    StartCell.PlayerVisitedState = CellState.Visited;
    trackpath.Add(StartCell);

    maskid = MaskManager._MaskManager.AddMask(transform, "player", maskscale * maze_size, true);
  }

  public Vector2 position(){
    return  MazeManager._MazeManager.GetMaze().GetCellPosition(currentx, currenty);
  }


  public bool IsMoving(){
    return mcurrentState == MoveState.Moving;
  }

  public void moveDir(Dir dir){
    if (mcurrentState == MoveState.Moving)
      return;

    //movepath是不包含現在站著的Cell
    movepath = MazeManager._MazeManager.GetMaze().movePath(currentx, currenty,dir);

    mcurrentType = MoveType.New;

    //如果可以移動的情況
    if(movepath.Count > 0){
      Cell currentCell = MazeManager._MazeManager.GetMaze().GetCell(currentx, currenty);
      //如果我現在的點跟movepath[0]個點都是visited的話表示我正在往回走
      if (movepath[0].PlayerVisitedState == CellState.Visited && currentCell.PlayerVisitedState == CellState.Visited){
        //所以我移除我現在站著的Cell
        RemoveTrackPath(currentCell);
        mcurrentType = MoveType.Goback;
      }
    }

    if (movepath.Count == 0)
      return;

    //Debug.Log("MOVESTART : " + mcurrentType);
    //Debug.Log("移動格數 : " + movepath.Count);

    mcurrentState = MoveState.Moving;
  }

  public float maskScale(){
    return maskscale;
  }

  void move(){
    if (mcurrentState == MoveState.Arrival)
      return;

    if(movepath.Count == 0){
      mcurrentState = MoveState.Arrival;
      return;
    }

    Vector2 currentposi = gameObject.transform.position;
    Cell TargetCell = movepath[0];
    Vector2 dir = (TargetCell.position - currentposi).normalized;
    float dis = basic_speed * Time.deltaTime * maze_size;

    if ((TargetCell.position - currentposi).magnitude <= dis){
      gameObject.transform.position = TargetCell.position;
      mcurrentState = MoveState.Arrival;
      //Debug.Log("到達位置[" + TargetCell.position.x + "，" + TargetCell.position.x + "]， CellState : " + TargetCell.PlayerVisitedState);
      currentx = movepath[0].X;
      currenty = movepath[0].Y;

      MazeManager._MazeManager.ArrivalCell("player", TargetCell);

      if (mcurrentType == MoveType.New)
      AddTrackPath(TargetCell);
      else{
        if (movepath.Count == 1){
          //往回走的時候最後一個targetCell不要刪除
        }
        else
          RemoveTrackPath(TargetCell);
      }
      
      movepath.RemoveAt(0);
      if(movepath.Count > 0)
        mcurrentState = MoveState.Moving;
      return;
    }

    gameObject.transform.position += (Vector3)(dir * dis);

    //CanvasMaskManager._MazeMaskManager.updateMaskPosion(gameObject.transform.position);
  }

  void AddTrackPath(Cell targetCell){
    if (targetCell.PlayerVisitedState == CellState.Visited)
      return;
    //開始的點不會被移除
    if (targetCell.Type == CellType.Start)
      return;
    //Debug.Log("位置[" + targetCell.position.x + "，" + targetCell.position.y + "]， 被加入");
    trackpath.Add(targetCell);
    targetCell.PlayerVisitedState = CellState.Visited;
  }

  void RemoveTrackPath(Cell targetCell){
    if (targetCell.PlayerVisitedState == CellState.NotVisited)
      return;
    //開始的點不會被移除
    if (targetCell.Type == CellType.Start)
      return;
    //Debug.Log("位置[" + targetCell.position.x + "，" + targetCell.position.y + "]， 被移除");
    //int index = trackpath.LastIndexOf(targetCell);
    //trackpath.RemoveAt(index);
    trackpath.Remove(targetCell);
    targetCell.PlayerVisitedState = CellState.NotVisited;
  }

  void updateTrackLine(){
    if (LineRenderer == null)
      return;

    LineRenderer.positionCount = trackpath.Count +1;
    float linedepth = 1.0f;

    if (trackpath.Count > 0){
      Vector3[] trackpositions = new Vector3[trackpath.Count+1];
      int index = 0;
      foreach (var v in trackpath){
        trackpositions[index] = new Vector3(v.position.x, v.position.y, linedepth);
        index++;
      }
      trackpositions[trackpath.Count] = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, linedepth);
      LineRenderer.SetPositions(trackpositions);
    }
  }
}
