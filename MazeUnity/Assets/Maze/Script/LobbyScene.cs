using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : MonoBehaviour,IScene
{
  string secneName;
  SceneDisposeHandler pDisposeHandler = null;
  bool mInited = false;

  enum State
  {
    NULL = 0,


    DONE,
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
    //...

    GameObject dynamicObj = gameObject;

    //
    // Intro prefab
    //
    GameObject cc = GameObject.Find("Lobby");
    if (cc == null){
      cc = instantiateObject(dynamicObj, "Lobby");
    }

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
    if(type == UIEventType.BUTTON){
      if(name == "Play_bt"){
        Debug.Log("Play_bt");
      }
    }
  }

  GameObject instantiateObject(GameObject parent, string name)
  {
    GameObject g = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab(name);
    g.transform.SetParent(parent.transform, true);

    return g;
  }
}
