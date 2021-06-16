using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//玩家使用道具管理
public class PlayerItemManager : MonoBehaviour
{
  public static PlayerItemManager _PlayerItemManager = null;
  private void Awake(){
    _PlayerItemManager = this;
  }

    // Update is called once per frame
    void Update(){

    }

  public void UseTorch(Vector2 position){
    float scale = MazeManager._MazeManager.cell_size;
    TorchManager._TorchManager.PlaceTorch(position, scale);
  }
}
