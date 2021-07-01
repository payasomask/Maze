using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscountNum{
  Animator discount_ani = null;
  SpriteRenderer discount_num = null;
  SpriteRenderer discount_num_glow = null;
  SpriteRenderer discount_numD = null;
  SpriteRenderer discount_num_glowD = null;
  public void Init(GameObject Root){
    discount_ani = Root.GetComponent<Animator>();
    discount_num = discount_ani.transform.Find("數字_Sample").GetComponent<SpriteRenderer>();
    discount_num_glow = discount_ani.transform.Find("數字_Sample_Glow").GetComponent<SpriteRenderer>();

    discount_numD = discount_ani.transform.Find("數字_SampleD").GetComponent<SpriteRenderer>();
    discount_num_glowD = discount_ani.transform.Find("數字_Sample_GlowD").GetComponent<SpriteRenderer>();

    discount_ani.transform.Find("倒數計時_光環").GetComponent<SpriteRenderer>().sprite =AssetbundleLoader._AssetbundleLoader.InstantiateSprite("card_game3", "倒數計時_光環");
    discount_ani.transform.Find("倒數計時_亮暈").GetComponent<SpriteRenderer>().sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("card_game3", "倒數計時_亮暈");
    discount_ani.transform.Find("New Sprite Mask_01").GetComponent<SpriteMask>().sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("card_game3", "倒數計時_Mask");
    discount_ani.transform.Find("New Sprite Mask_02").GetComponent<SpriteMask>().sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("card_game3", "倒數計時_Mask");

    //動態是靠美術用動畫作掉...那十位數..可能也用動畫做嗎? 不太想再弄一個動畫出來，首先要一組倒數計時
    //不過這樣等於要動態調整數字的位置...
  }

  //倒數的時間因為美術資源只有給一位數
  public int checktime(int time){
    if (time >= 99)
      return 99;

    if (time <= 0)
      return 0;

    return (int)time;
  }


  public void updateDiscount(int currenttime){
    if (discount_ani == null || discount_num == null || discount_num_glow == null)
      return;

    int displaynum = checktime(currenttime);

    int DD = displaynum / 10;
    int D = displaynum - 10 * DD;

    discount_num.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("card_game3", "倒數計時數字_" + D);
    discount_num_glow.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("card_game3", "倒數計時數字_" + D);

    discount_numD.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("card_game3", "倒數計時數字_" + DD);
    discount_num_glowD.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("card_game3", "倒數計時數字_" + DD);

    discount_numD.gameObject.SetActive(DD > 0);
    discount_num_glowD.gameObject.SetActive(DD > 0);

    discount_numD.transform.localPosition = DD > 0 ? new Vector3(-38.0f, 0.0f, -5.0f) : new Vector3(0.0f, 0.0f, 0.0f);
    discount_num_glowD.transform.localPosition = DD> 0 ? new Vector3(-38.0f, 0.0f, -6.0f) : new Vector3(0.0f, 0.0f, 0.0f);

    discount_num.transform.localPosition = DD > 0 ? new Vector3(17.0f,0.0f,-5.0f) : new Vector3(0.0f,0.0f,-5.0f);
    discount_num_glow.transform.localPosition = DD > 0 ? new Vector3(17.0f, 0.0f, -6.0f) : new Vector3(0.0f, 0.0f, -6.0f);


    if (discount_ani.gameObject.activeSelf)
      discount_ani.SetTrigger("discount");
    else
    {
      discount_ani.gameObject.SetActive(true);
      discount_ani.SetTrigger("discount");
    }
  }
}

public class InteractiveDialog : IDialogContext
{

  public enum Type
  {
    Normal,
    Warring,
    Store
  }

  float button_interval = 150.0f;
  public string message;
  string[] buttons_name;
  public bool binited = false;
  GameObject stmgr = null;
  //SpriteRenderer bg_sr = null;
  Text t = null;
  //float minWidth = 970.0f;
  //float minHeight = 363.4f;
  //float AddHeight = 40.0f;
  //第一行的字距離BG上緣距離
  //float first_line_offsetY = 70.0f;
  //總共有幾行(大約)
  //int Lines = 0;
  //BT距離BG下緣距離
  //float Bt_offsetY = 87.28f;
  //float topofbg_offset = -16.3f;
  InteractiveDiaLogHandler[] pHandlers;
  Type type = Type.Normal;
  bool activediscount = false;
  DiscountNum discount_num = null;
  public InteractiveDialog(string msg, string[] buttons_name,InteractiveDiaLogHandler[] Handlers,Type type = Type.Normal,bool discount = false)
  {
    this.buttons_name = buttons_name;
    message = msg;
    pHandlers = Handlers;
    this.type = type;
    activediscount = discount;
    //每40個英文字元增加一個height
    //Lines = (message.Length / 40);
    //每18個中文增加一個height
    //Lines = (message.Length / 18);
  }

  public bool inited()
  {
    return binited;
  }

  void callback(object param = null)
  {
    if(param == null)
      return;
    else
    {
      int bt_index = (int)param;
      if (pHandlers != null)
        if(pHandlers[bt_index] != null)
          pHandlers[bt_index]();
    }
  }

  public bool dismiss(){

    //被外部呼叫 dismiss
    //...

    GameObject.Destroy(stmgr);

    return true;
  }

  public DialogType getType()
  {
    return DialogType.NORMAL;
  }
  int dialogindex = 0;
  public GameObject init(int dlgIdx, AssetbundleLoader abl)
  {
    ////////
    stmgr = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab("InteractiveDialog");
    stmgr.name = "InteractiveDialog";
    Timer t = stmgr.AddComponent<Timer>();

    int interval = 0;
    dialogindex = dlgIdx;
    if (activediscount)
    {
      GameObject countergo = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab("倒數計時數字");
      countergo.transform.SetParent(stmgr.transform);
      countergo.transform.position = new Vector3(287.0f, -92.0f, 0.0f);
      countergo.transform.localScale = new Vector3(0.7f, 0.7f, 1.0f);
      discount_num = new DiscountNum();
      discount_num.Init(countergo);
    }

    GameObject textgo = stmgr.transform.Find("Scaleble_text").gameObject;
    VerticalLayoutGroup VLG = textgo.transform.Find("Panel").GetComponent<VerticalLayoutGroup>();
    //在字數不夠的情況最大值的左右寬度是300
    //那何謂字數沖不充足，那要以總按鈕的數量而定，而按鈕的數量
    //所以必須先計算出總按鈕的sprite寬度 + 按鈕尖閣寬度
    float buttontotalX = 0;
    int buttonintervalcount = buttons_name.Length > 0 ? buttons_name.Length -1 : 0;
    FontManager._FontManager.getFont(textgo.transform.Find("Panel/TextMeshPro Text").gameObject.GetComponent<TextMeshProUGUI>());
    TextMeshProUGUI textmesh = textgo.transform.Find("Panel/TextMeshPro Text").GetComponent<TextMeshProUGUI>();
    textmesh.text = message;

    //每多一行bg +一個高度
    Image bg_sr = stmgr.transform.Find("Scaleble_text/Panel").GetComponent<Image>();
    if (type == Type.Warring)
      bg_sr.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("embedded_nine_slice", "dialog_warring_bg");
    else if (type == Type.Store)
    {
      //bg_sr.transform.localPosition = new Vector3(-357.0f, 101.0f, 0);
      //bg_sr.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
      bg_sr.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("embedded_nine_slice", "tip_bg");
      textmesh.color = Color.black;
      //Bt_offsetY -= 135.280f;
    }
    //float currentH = bg_sr.sprite.bounds.size.y;


    //calculate Bt posY
    //改用自定義高度基值為-41.06再根據字高調整高度
    //一個中文字36寬47.88高
    float bt_Y = -41.06f - textmesh.preferredHeight * 0.5f;

    //由於不確定字數 關鍵就是我能不能鎖住text的寬高，VerticalLayoutGroup 限制childControlWidth的情況下不能更改
    //所以我必須先判斷字數是否有超過我的最大允許範圍，最大容許範圍944.79/430.95 
    //並且去關閉childControlWidth/childControlHeight，
    //關閉有兩個作用，一，讓dialog長出來的寬高會是我設定最大容許範圍的寬高
    //二，讓textmesh的字overflow可以正常運作
    if (textmesh.preferredWidth > 944.79f)
    {
      VLG.childControlWidth = false;
    }
    if (textmesh.preferredHeight > 430.95f)
    {
      //但是這邊會讓按鈕的Y位置有落差，因為childControlHeight的情況下textmesh的width / height會完美的符合字的preferredwidth/height
      //所以這裡改用最大容許範圍的430.95f作為計算
      VLG.childControlHeight = false;
      bt_Y = -41.06f - 430.95f * 0.5f;
    }
    //最後不管怎樣，複寫設定字的寬高最大容許範圍944.79/430.95
    textmesh.rectTransform.sizeDelta = new Vector2(944.79f, 430.95f);

    //Z值由canvas物件為基準，因為此階段不知道stmgrZ值會被安排到多少
    float bt_Z = -10.0f;
    //X值目前不打算隨著panel增長
    
    //基數 偶數不同排列方式
    if (buttons_name.Length % 2 == 0)
    {
      for (int i = 0; i < buttons_name.Length; i++)
      {
        GameObject bt = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab(buttons_name[i]);
        bt.name = "Interactive_" + buttons_name[i] + "_" + dlgIdx + "_" + i;
        bt.transform.SetParent(textgo.transform);
        bt.transform.localScale = Vector3.one;
        buttontotalX += bt.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        if (i % 2 == 0)
        {
          interval++;
          bt.transform.localPosition = new Vector3(-button_interval * (interval + 1) * 0.5f, bt_Y, bt_Z);
        }
        else
        {
          bt.transform.localPosition = new Vector3(+button_interval * (interval + 1) * 0.5f, bt_Y, bt_Z);
        }
      }
    }
    else
    {
      for (int i = 0; i < buttons_name.Length; i++)
      {
        GameObject bt = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab(buttons_name[i]);
        bt.name = "Interactive_" + buttons_name[i] + "_" + dlgIdx + "_" + i;
        bt.transform.SetParent(textgo.transform);
        bt.transform.localScale = Vector3.one;
        buttontotalX += bt.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        if (i % 2 == 0)
        {
          bt.transform.localPosition = new Vector3(-button_interval * interval, bt_Y, bt_Z);
          interval++;
        }
        else
        {
          bt.transform.localPosition = new Vector3(+button_interval * interval, bt_Y, bt_Z);
        }
      }

    }



    buttontotalX += buttonintervalcount * button_interval;

    //重新計算nine slice的左右寬度
    //如果字寬是0 那就 300
    //50為反向lerp 確保字寬大於 按鈕總寬 的話 也會有50的左右寬度
    //確保字寬小於 按鈕總寬 的話 最小的時候寬度是300
    float lerp = UnityEngine.Mathf.Clamp01((buttontotalX - textmesh.preferredWidth) / buttontotalX);
    int panelwidth = (int)(lerp * 300.0f);
    VLG.padding.left = panelwidth+(int)(100 * 1.0f - lerp);
    VLG.padding.right = panelwidth+ (int)(100 * 1.0f - lerp);


    binited = true;
    AudioController._AudioController.playOverlapEffect("sfx_msg_tip");

    CameraExtensions.CanvasScreenSpaceCamera(textgo);
    //如果攝影機發生異常，不見了比如說登入可能發生的339，是因為生成dlg的當下的BootloaderCamera被刪除，嘗試找回maincamera
    t.start(() => {
      if(stmgr.transform.Find("Scaleble_text").GetComponent<Canvas>().worldCamera == null)
        CameraExtensions.CanvasScreenSpaceCamera(textgo);
    });
    return stmgr;
  }


  //InteractiveDialog舊版
  //public GameObject init(int dlgIdx, AssetbundleLoader abl)
  //{
  //  ////////
  //  stmgr = tmpCxt.mABL.InstantiatePrefab("InteractiveDialog");
  //  stmgr.name = "InteractiveDialog";
  //  int interval = 0;
  //  dialogindex = dlgIdx;
  //  if (activediscount)
  //  {
  //    GameObject countergo = tmpCxt.mABL.InstantiatePrefab("倒數計時數字");
  //    countergo.transform.SetParent(stmgr.transform);
  //    countergo.transform.position = new Vector3(287.0f, -92.0f, 0.0f);
  //    countergo.transform.localScale = new Vector3(0.7f, 0.7f, 1.0f);
  //    discount_num = new DiscountNum();
  //    discount_num.Init(tmpCxt, countergo);
  //  }

  //  GameObject textgo = stmgr.transform.Find("scrollable_text").gameObject;
  //  ScrollableTextController stc = textgo.GetComponent<ScrollableTextController>();
  //  stc.init(message);

  //  //每多一行bg +一個高度
  //  bg_sr = stmgr.transform.Find("bg").GetComponent<SpriteRenderer>();
  //  if (type == Type.Warring)
  //    bg_sr.sprite = tmpCxt.mABL.InstantiateSprite("embedded2", "dialog_warring_bg");
  //  else if(type == Type.Store){
  //    bg_sr.transform.localPosition = new Vector3(-357.0f, 101.0f, 0);
  //    bg_sr.transform.localScale= new Vector3(1.5f, 1.5f, 1.0f);
  //    bg_sr.sprite = tmpCxt.mABL.InstantiateSprite("store", "tip_bg");
  //    stc.setColor(Color.black);
  //    Bt_offsetY -= 135.280f;
  //  }
  //  float currentH = bg_sr.sprite.bounds.size.y;

  //  //calculate Bt posY
  //  float bt_Y = bg_sr.transform.position.y - currentH * 0.5f + Bt_offsetY;

  //  //先判斷有幾個buttons
  //  //基數 偶數不同排列方式
  //  if (buttons_name.Length % 2 == 0)
  //  {
  //    for (int i = 0; i < buttons_name.Length; i++)
  //    {
  //      GameObject bt = tmpCxt.mABL.InstantiatePrefab(buttons_name[i]);
  //      bt.name = "Interactive_" + buttons_name[i] + "_" + dlgIdx + "_" + i;
  //      bt.transform.SetParent(stmgr.transform);
  //      if (i % 2 == 0)
  //      {
  //        interval++;
  //        bt.transform.localPosition = new Vector3(-button_interval * (interval + 1) * 0.5f, bt_Y, -50f);
  //      }
  //      else
  //      {
  //        bt.transform.localPosition = new Vector3(+button_interval * (interval + 1) * 0.5f, bt_Y, -50f);
  //      }
  //    }
  //  }
  //  else
  //  {
  //    for (int i = 0; i < buttons_name.Length; i++)
  //    {
  //      GameObject bt = tmpCxt.mABL.InstantiatePrefab(buttons_name[i]);
  //      bt.name = "Interactive_" + buttons_name[i] + "_" + dlgIdx + "_" + i;
  //      bt.transform.SetParent(stmgr.transform);

  //      if (i % 2 == 0)
  //      {
  //        bt.transform.localPosition = new Vector3(-button_interval * interval, bt_Y, -50f);
  //        interval++;
  //      }
  //      else
  //      {
  //        bt.transform.localPosition = new Vector3(+button_interval * interval, bt_Y, -50f);
  //      }
  //    }

  //  }

  //  ////////

  //  binited = true;
  //  tmpCxt.mAC.playOverlapEffect("sfx_msg_tip");

  //  return stmgr;
  //}



  // Use this for initialization
  void Start()
  {
  }

  public void updateMessage(string msg){
    message = msg;

    if (t == null)
      return;
    t.text = msg;
  }

  public void updateDiscount(int currenttime){
    discount_num.updateDiscount(currenttime);
  }

  public DialogResponse setUIEvent(string name, UIEventType type, object[] extra_info){
    //攔截Interactive dlg 訊息
    if (name.Contains("Interactive_") && type ==UIEventType.BUTTON)
    {
      string[] token = name.Split('_');
      if (token.Length == 4)
      {
        int outDlgIdx = -1;
        int outbtindex = -1;
        int.TryParse(token[2], out outDlgIdx);
        int.TryParse(token[3], out outbtindex);

        //若是同意等的 event 則呼叫對應之 callback function
        callback(outbtindex);
      }
      AudioController._AudioController.playOverlapEffect("sfx_btn");

      // if(tmpCxt.mDLG.getCurrentDialogIdx() != dialogindex)//判斷要返回的時後是不是有新的dialog被建立出來...
      //   return DialogResponse.TAKEN_AND_DISMISS_PREVIOUS;

      //因為是 taken and dismiss, 所以外部的 dialog manager 之後會呼叫本 dialog 的 dismiss function
      return DialogResponse.TAKEN_AND_DISMISS;
    }

    //ui event 不屬於本 dialogue, pass 給其它物件
    return DialogResponse.PASS;
  }

  public DialogNetworkResponse setNetworkResponseEvent(string name, object payload){
    return DialogNetworkResponse.PASS;
  }

  public DialogEscapeType getEscapeType(){
    return DialogEscapeType.DISMISS;
  }
}
