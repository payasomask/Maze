using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonLoader : MonoBehaviour
{

  float[] time_map_arr = new float[] {
30,
30,
30,
60,
60,
60,
90,
90,
90,
120,
120
  };

  public static JsonLoader _JsonLoader = null;
  Dictionary<int, MazeConfig> mazeConfig_dic = new Dictionary<int, MazeConfig>();

  private void Awake(){
    _JsonLoader = this;
  }

  public void Init(){
    //start init


    int column_init = 6;//��lcolumn�ƶq
    int row_init = 8;//��lrow�ƶq
    int row_interval = 4;//�C�@�Ӱ϶�row�������Y��
    int column_interval =3;//�C�@�Ӱ϶�columns�������Y��
    int level_interval = 0;//���d�϶��ƶq

    int first_level_interval_max_level = 10;

    //level init
    for(int i = first_level_interval_max_level; i < 120; i += 10){
      MazeConfig tmp = new MazeConfig();
      tmp.maxlevel = i;
      tmp.Rows = row_init + row_interval * level_interval;
      tmp.Columns = column_init + column_interval * level_interval;
      float level_time;
      try
      {
        level_time = time_map_arr[level_interval];
      }
      catch(ArgumentOutOfRangeException e)
      {
        level_time = time_map_arr[time_map_arr.Length-1];
      }
      tmp.LimitTime = level_time;
      tmp.boxADReward = new ADReward(ItmeType.Torch, 2, ItmeType.Torch,1);
      tmp.CompletedReward = new ADReward(ItmeType.OilLamp, 2, ItmeType.OilLamp,1);
      tmp.GameOverReward = new ADReward(ItmeType.OilLamp, 2, ItmeType.Torch, 1);
      tmp.DownUIReward = new ADReward(ItmeType.Torch, 1, ItmeType.Torch,0);
      level_interval++;
      mazeConfig_dic.Add(i, tmp);
    }

  }


  public MazeConfig GetMazeConfig(int level){

    if (mazeConfig_dic.Count == 0){
      Debug.Log("645 - Init JsonLoader first..");
      return null;
    }

    MazeConfig lastconfig = null;
    foreach(var v in mazeConfig_dic){
      lastconfig = v.Value;
      if (level <= v.Value.maxlevel)
        return v.Value;
    }

    return lastconfig;
  }
}

public enum ItmeType{
  Torch,
  OilLamp
}

public enum GameType{
  LIGHT,
  NIGHT
}

public enum MazeType{
  Prims=0,
  HuntKill,
  SZ
}

public class ADReward{
  public ADReward(ItmeType Type, int Num, ItmeType skipType, int SkipNum)
  {
    this.Type = Type;
    this.Num = Num;
    this.SkipType = skipType;
    this.SkipNum = SkipNum;
  }
  //�ݼs�i
  public ItmeType Type;
  public int Num;
  //���ݼs�i���y
  public ItmeType SkipType;
  public int SkipNum;
}

public class MazeConfig{
  public int maxlevel;//���d�϶�����
  public int Rows, Columns;
  public float LimitTime;
  public ADReward boxADReward;
  public ADReward CompletedReward;//
  public ADReward GameOverReward;//
  public ADReward DownUIReward;//�U��s�i���y
}
