using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ADSDialog :  IDialogContext
{

  public ADSDialog(Sprite itemicon, InteractiveDiaLogHandler[] handlers){
    micon = itemicon;
    bt_handlers = handlers;
  }
  bool binited = false;
  string message;
  Sprite micon = null;
  GameObject dlgGO = null;
  InteractiveDiaLogHandler[] bt_handlers;
  public bool dismiss()
  {
    GameObject.Destroy(dlgGO);
    return true;
  }

  public DialogEscapeType getEscapeType()
  {
    return DialogEscapeType.NOTHING;
  }

  public DialogType getType()
  {
    return DialogType.NORMAL;
  }

  public GameObject init(int dlgIdx, AssetbundleLoader abl){
      dlgGO = abl.InstantiatePrefab("ADSDialog");
      binited = true;


    SpriteRenderer icon_sr = dlgGO.transform.Find("Bg/dialog_No_bt/icon").GetComponent<SpriteRenderer>();
    icon_sr.sprite = micon;

    icon_sr = dlgGO.transform.Find("Bg/dialog_Yes_bt/icon").GetComponent<SpriteRenderer>();
    icon_sr.sprite = micon;

    int noadsamount = 4;
    TextMeshPro text = dlgGO.transform.Find("Bg/dialog_No_bt/amount").GetComponent<TextMeshPro>();
    text.text = "x" + noadsamount;

    int adsamount = 8;
    text = dlgGO.transform.Find("Bg/dialog_Yes_bt/amount").GetComponent<TextMeshPro>();
    text.text = "x" + adsamount;

    return dlgGO;
  }

  public bool inited()
  {
    return binited;
  }

  public DialogResponse setUIEvent(string name, UIEventType type, object[] extra_info)
  {
    if(type == UIEventType.BUTTON){
      if(name == "dialog_Yes_bt"){
        if(bt_handlers[0] != null)
          bt_handlers[0]();
        return DialogResponse.TAKEN_AND_DISMISS;
      }
      else if(name == "dialog_No_bt"){
        if (bt_handlers[1] != null)
          bt_handlers[1]();
        return DialogResponse.TAKEN_AND_DISMISS;
      }
    }

    return DialogResponse.PASS;
  }



  public DialogNetworkResponse setNetworkResponseEvent(string name, object payload)
  {
    return DialogNetworkResponse.PASS;
  }
}
