using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchManager : MonoBehaviour
{
  public static TorchManager _TorchManager = null;

  [SerializeField]
  private GameObject TorchObj = null;


  class Torch{
    public GameObject gameobj = null;
    public int id;
  }

  Dictionary<int, Torch> torch_dic = new Dictionary<int, Torch>();

  private void Awake(){
    _TorchManager = this;
  }


  //°òÂ¦·Ó«G½d³ò
  float basic_Light_Radius = 20.0f;

  public void PlaceTorch(Vector2 position, float scale){

    GameObject tmp = Instantiate(TorchObj, transform);
    tmp.transform.localPosition = position;
    tmp.transform.localScale = new Vector3(scale, scale, 1.0f);
    tmp.transform.Find("mask").localScale = new Vector3(basic_Light_Radius, basic_Light_Radius, 1.0f);
    SineScale ss = tmp.transform.Find("mask").gameObject.AddComponent<SineScale>();
  }


}
