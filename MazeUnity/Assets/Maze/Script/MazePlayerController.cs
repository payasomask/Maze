using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePlayerController : MonoBehaviour
{
  [SerializeField]
  private float basic_speed = 10.0f;
  private float maze_size;

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
    if (Input.GetKeyUp(KeyCode.UpArrow)){
      moveDir(Dir.Top);
    }
    if (Input.GetKeyUp(KeyCode.DownArrow)){
      moveDir(Dir.Bottom);
    }
    if (Input.GetKeyUp(KeyCode.LeftArrow))
    {
      moveDir(Dir.Left);
    }
    if (Input.GetKeyUp(KeyCode.RightArrow))
    {
      moveDir(Dir.Right);
    }

    if (Input.GetKeyUp(KeyCode.Space)){
      if (mcurrentState == MoveState.Moving)
        return;
      Vector2 postion =MazeManager._MazeManager.GetMaze().GetCellPosition(currentx, currenty);
      PlayerItemManager._PlayerItemManager.UseTorch(postion);
    }

    move();
    updateTrackLine();

  }

  public void init(int currentx, int currenty, float maze_size) {
    this.maze_size = maze_size;
    this.currentx = currentx;
    this.currenty = currenty;

    //感覺3倍的maze_size比較舒服
    float scale = 3.0f;
    gameObject.transform.localScale = new Vector3(scale * maze_size, scale * maze_size, 1.0f);

    LineRenderer = gameObject.GetComponent<LineRenderer>();

    Cell StartCell = MazeManager._MazeManager.GetMaze().GetCell(currentx, currenty);
    StartCell.PlayerVisitedState = CellState.Visited;
    trackpath.Add(StartCell);

    maskid = MaskManager._MaskManager.AddMask(transform, "player", scale,true);
  }

  void moveDir(Dir dir){
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

      if(mcurrentType == MoveType.New)
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

    CanvasMaskManager._MazeMaskManager.updateMaskPosion(gameObject.transform.position);
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

    if (trackpath.Count > 0){
      Vector3[] trackpositions = new Vector3[trackpath.Count+1];
      int index = 0;
      foreach (var v in trackpath){
        trackpositions[index] = v.position;
        index++;
      }
      trackpositions[trackpath.Count] = gameObject.transform.position;
      LineRenderer.SetPositions(trackpositions);
    }
  }
}
