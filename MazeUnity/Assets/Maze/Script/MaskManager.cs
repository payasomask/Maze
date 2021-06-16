using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{

  public static MaskManager _MaskManager = null;

  [SerializeField]
  private Sprite Circle;
  [SerializeField]
  private Sprite Square;

  int maskid = 0;
  int blackid = 0;
  Dictionary<int, MaskData> mask_dic = new Dictionary<int, MaskData>();
  Dictionary<int, MaskData> black_dic = new Dictionary<int, MaskData>();


  public class MaskData {
    public int id;
    public string name;
    public Transform t;
  }

  private void Awake(){
    _MaskManager = this;
  }

  public void Init(){

  }

  public int AddMask(Transform parent ,string name,float scale, bool sinScale = false){
    int tmpid = maskid;
    MaskData tmp = new MaskData();
    GameObject go = new GameObject(name + "_" + tmpid);
    SpriteMask sr = go.AddComponent<SpriteMask>();
    sr.sprite = Circle;
    go.transform.SetParent(parent);
    go.transform.localPosition = Vector3.zero;
    tmp.t = go.transform;
    tmp.t.localScale = new Vector3(scale, scale, 1.0f);
    tmp.id = tmpid;
    tmp.name = name;

    if (sinScale)
      go.AddComponent<SineScale>();

    mask_dic.Add(tmpid, tmp);
    maskid++;
    return tmpid;
  }

  public void AddBlack(string name, Vector2 Position, Vector2 scale)
  {
    MaskData tmp = new MaskData();
    GameObject go = new GameObject(name + "_" + blackid);
    go.transform.SetParent(transform);
    go.transform.localPosition = new Vector3(Position.x,Position.y,-20.0f);
    SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
    sr.sprite = Square;
    sr.color = Color.black;
    sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
    tmp.t = go.transform;
    tmp.t.localScale = new Vector3(scale.x, scale.y, 1.0f);
    tmp.id = blackid;
    tmp.name = name;
    black_dic.Add(blackid, tmp);
    blackid++;
  }

  // Update is called once per frame
  void Update()
    {
        
    }
}
