using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MazeScene : MonoBehaviour,IScene
{
  string secneName;
  SceneDisposeHandler pDisposeHandler = null;
  bool mInited = false;

  TextMeshPro timer;
  //TextMeshPro itemtimer;

  float gametime = 0.0f;
  string oillamptimerID;

  UIButton torchbt = null;
  UIButton oillampbt = null;

  GameObject mRoot = null;

  //UIButton staffbt = null;
  //bool staff_used = false;

  enum State
  {
    NULL = 0,

    CREAT_MAZE,

    IDLE,
    STOP,

    GAME_OVER,
    COMPLETED,
    WAITPLAYER,
  }

  State currentstate = State.NULL;

  public void disposeScene(bool forceDispose)
  {
    pDisposeHandler = null;
  }

  public int getSceneInitializeProgress()
  {
    return 0;
  }

  public string getSceneName()
  {
    return secneName;
  }

  public void initLoadingScene(string name, object[] extra_param = null)
  {
    secneName = name;
  }

  public void initScene(string name, object[] extra_param = null)
  {
    //...do somthing init

    GameObject dynamicObj = gameObject;

    //
    // Intro prefab
    //
    mRoot = GameObject.Find("Maze");
    if (mRoot == null)
    {
      mRoot = instantiateObject(dynamicObj, "Maze");
    }


    gametime = 1.00f;
    timer = mRoot.transform.Find("TopUI/bg/timer").GetComponent<TextMeshPro>();
    updateGameTime(gametime);

    torchbt = mRoot.transform.Find("DownUI/bg/Torchbt").GetComponent<UIButton>();
    oillampbt = mRoot.transform.Find("DownUI/bg/oillampbt").GetComponent<UIButton>();
    //staffbt = cc.transform.Find("DownUI/staffbt").GetComponent<UIButton>();

    //itemtimer = cc.transform.Find("DownUI/itemtimer").GetComponent<TextMeshPro>();

    MaskManager._MaskManager.Init();
    MazeManager._MazeManager.Init(this);

    currentstate = State.CREAT_MAZE;
    mInited = true;

    return;
  }

  public bool isSceneDisposed()
  {
    return (pDisposeHandler == null);
  }

  public bool isSceneInitialized()
  {
    return mInited;
  }

  public void registerSceneDisposeHandler(SceneDisposeHandler pHandler)
  {
    pDisposeHandler = pHandler;
  }

  public void setUIEvent(string name, UIEventType type, object[] extra_info)
  {
    if (type == UIEventType.BUTTON)
    {
      if (name == "stopbt")
      {
        SpriteRenderer bt = transform.Find("Maze(Clone)/TopUI/bg/stopbt").GetComponent<SpriteRenderer>();
        SpriteRenderer icon = transform.Find("Maze(Clone)/TopUI/bg/stopbt/icon").GetComponent<SpriteRenderer>();
        bt.gameObject.name = "playbt";
        icon.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", "play_gray");
        currentstate = State.STOP;
        //switch sprite

      }
      else if(name == "playbt"){
        SpriteRenderer bt = transform.Find("Maze(Clone)/TopUI/bg/playbt").GetComponent<SpriteRenderer>();
        SpriteRenderer icon = transform.Find("Maze(Clone)/TopUI/bg/playbt/icon").GetComponent<SpriteRenderer>();
        bt.gameObject.name = "stopbt";
        icon.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", "pause");

        currentstate = State.IDLE;
        //switch sprite

      }
      else if (name == "Torchbt")
      {

        //...
        UIDialog._UIDialog.show(new UseItemDialog("Use a torch?", AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", "toch"), new InteractiveDiaLogHandler[] {
      ()=>{
        //No
        currentstate = State.IDLE;
        return;
      },
      ()=>{
        //YES
        PlayerItemManager._PlayerItemManager.UseTorch(MazeManager._MazeManager.PlayerPosition());
        currentstate = State.IDLE;
        return;
      }
      }));
        currentstate = State.WAITPLAYER;

      }
      else if (name == "oillampbt")
      {
        UIDialog._UIDialog.show(new UseItemDialog("Use a oillamp?", AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", "lamp"), new InteractiveDiaLogHandler[] {
      ()=>{
        currentstate = State.IDLE;
        return;
      },
      ()=>{
        PlayerItemManager._PlayerItemManager.UseOilLamp(5.0f);
        MaskManager._MaskManager.ShowMask("box");
        Timer oillamptimer = oillampbt.gameObject.AddComponent<Timer>();
        oillamptimerID = oillamptimer.start(10.0f , ()=>{
          //回復玩家mask範圍
          PlayerItemManager._PlayerItemManager.UseOilLamp(1.0f);
          MaskManager._MaskManager.HideMask("box");
          //並移除time;
          DestroyImmediate(oillamptimer);
        });
        currentstate = State.IDLE;
        return;
      }
      }));
        currentstate = State.WAITPLAYER;
      }
      //else if (name == "staffbt")
      //{
      //  UIDialog._UIDialog.show(new InteractiveDialog("Use Staff?", new string[] { "acceptbt", "cancelbt" }, new InteractiveDiaLogHandler[] {
      //()=>{
      //  //staff_used = true;
      //  PlayerItemManager._PlayerItemManager.UseStaff();
      //  currentstate = State.IDLE;
      //  return;
      //},
      //()=>{
      //  currentstate = State.IDLE;
      //  return;
      //}
      //}));
      //  currentstate = State.WAITPLAYER;

      //}

    }
  }

  GameObject instantiateObject(GameObject parent, string name)
  {
    GameObject g = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab(name);
    g.transform.SetParent(parent.transform, true);

    return g;
  }

  void Update(){

    if (Input.GetKeyUp(KeyCode.R)){
      CREAT_MAZE();
    }

    if(currentstate == State.STOP){
      return;
    }

    UpdateButton();

    if (currentstate == State.IDLE)
    {
      gametime -= Time.deltaTime;
      if (gametime <= 0.0f)
      {
        gametime = 0.0f;
        currentstate = State.GAME_OVER;
      }
      updateGameTime(gametime);
      //updateItemTimer();

      PlayerControll();
    }
    else
    if (currentstate == State.CREAT_MAZE){

      MazeManager._MazeManager.ClearMaze();
      //維持4:3
      //4可以小 : 3可以大
      MazeManager._MazeManager.CreatMaze(9, 9);
      gametime = 1000.0f;


      currentstate = State.IDLE;
      return;
    }
    else if(currentstate == State.GAME_OVER){

      //...
      UIDialog._UIDialog.show(new UseItemDialog("Times up, reset?",null, new InteractiveDiaLogHandler[] {
      ()=>{
        //返回大廳
        pDisposeHandler( SceneDisposeReason.USER_EXIT,null);
        return;
      },
      ()=>{
        //重製迷宮，或是使用原迷宮?
        CREAT_MAZE();
        return;
      }
      }));

      currentstate = State.WAITPLAYER;
    }
    else if (currentstate == State.COMPLETED){
      //...do somthing
      UIDialog._UIDialog.show(new UseItemDialog("Go Next Maze?",null, new InteractiveDiaLogHandler[] {
      ()=>{
        //取消的話就是返回大廳
        pDisposeHandler(SceneDisposeReason.USER_EXIT,null);
        return;
      },
      ()=>{
         //..更換迷宮? 原場景重製? 或是啥的不知道
        CREAT_MAZE();
        return;
      }
      }));
      currentstate = State.WAITPLAYER;
      return;
    }

  }

  void CREAT_MAZE() {
    currentstate = State.CREAT_MAZE;
    //staff_used = false;
    return;
  }

  void updateGameTime(float time){
    if (timer == null)
      return;

    TimeSpan span = TimeSpan.FromSeconds((double)(new decimal(time)));
    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    DateTime date = epoch + span;

    timer.text = "TIME\n" + date.ToString("mm:ss.ff");
  }

  void PlayerControll(){

    //..根據不同平台在這切換輸入原

    if (Input.GetKeyUp(KeyCode.UpArrow))
    {
      MazeManager._MazeManager.movePlayer(Dir.Top);
    }
    if (Input.GetKeyUp(KeyCode.DownArrow))
    {
      MazeManager._MazeManager.movePlayer(Dir.Bottom);
    }
    if (Input.GetKeyUp(KeyCode.LeftArrow))
    {
      MazeManager._MazeManager.movePlayer(Dir.Left);
    }
    if (Input.GetKeyUp(KeyCode.RightArrow))
    {
      MazeManager._MazeManager.movePlayer(Dir.Right);
    }

  }

  void UpdateButton(){
    bool playermoving = MazeManager._MazeManager.IsPlayerMoving();

    //bool canusestaff = !staff_used && !playermoving;
    //staffbt.setEnabled(canusestaff);
    //torchbt.setEnabled(!playermoving && canusestaff);
    //bool canuseoil = !playermoving && oillampbt.GetComponent<Timer>() == null;
    //oillampbt.setEnabled(canuseoil && canusestaff);

    torchbt.setEnabled(!playermoving);
    bool canuseoil = !playermoving && oillampbt.GetComponent<Timer>() == null;
    oillampbt.setEnabled(canuseoil);

  }

  //void updateItemTimer(){
  //  if (itemtimer == null)
  //  return;

  //  Timer oildtimer = oillampbt.GetComponent<Timer>();
  //  if (oildtimer == null){
  //    itemtimer.gameObject.SetActive(false);
  //    return;
  //  }

  //  itemtimer.gameObject.SetActive(true);
  //  itemtimer.text = "Oil Lamp Remain Time : " + oildtimer.getSessionTime(oillamptimerID).ToString("F02", System.Globalization.CultureInfo.InvariantCulture);

  //}

  public void ArrivalCell(string who, Cell c){

    if(c.Type == CellType.Box){
      UIDialog._UIDialog.show(new ADSDialog(AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", "toch"), new InteractiveDiaLogHandler[] {
      ()=>{
        //..看廣告之類的啥的
        currentstate = State.IDLE;
        return;
      },
      ()=>{
        currentstate = State.IDLE;
        return;
      }
      }));
      currentstate = State.WAITPLAYER;
      return;
    }else if(c.Type == CellType.Goal){
      currentstate = State.COMPLETED;
      return;
    }

  }

  //能需要根據美術的設計調整整座迷宮的中心點位置
  //這裡需要回傳TOPUI的下緣Y位置
  public float GetMazeTopUIBottom(){
    
    float topuiHight = mRoot.transform.Find("TopUI/bg").GetComponent<SpriteRenderer>().sprite.bounds.size.y;
    return MainLogic._MainLogic.getCameraHight() * 0.5f - topuiHight;
  }
}
