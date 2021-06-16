using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class UtilityHelper 
{

  public static int Random(int min, int max)
  {
    UnityEngine.Random.InitState(Guid.NewGuid().GetHashCode());
    return UnityEngine.Random.Range(min, max);
  }

  //畢氏定理
  public static Vector2 Pythagoreantheorem(float bottom, float hight){
    float c_length = Mathf.Pow((Mathf.Pow(bottom , 2) + Mathf.Pow(hight , 2)), 0.5f);
    return new Vector2(bottom, hight).normalized * c_length;
  }

  //迷宮矩形中心點
  public static Vector2 MazePoint(float bottom, float hight){
    return Pythagoreantheorem(bottom, hight) * 0.5f;
  }
}
