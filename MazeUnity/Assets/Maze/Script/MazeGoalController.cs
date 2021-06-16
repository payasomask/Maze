using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGoalController : MonoBehaviour
{
  private int currentx, currenty;
  private int maskid; 
  public void init(int currentx, int currenty, float maze_size)
  {
    this.currentx = currentx;
    this.currenty = currenty;
    //�Pı3����maze_size����ΪA
    float scale = 3.0f;
    gameObject.transform.localScale = new Vector3(scale * maze_size, scale * maze_size, 1.0f);
    maskid = MaskManager._MaskManager.AddMask(transform, "goal", scale);
  }
}
