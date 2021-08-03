using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
  enum SessionType{
    COUNT_DOWN,
    EACH_FRAME
  }
  class Session{
    public SessionType st =SessionType.COUNT_DOWN;
    public string session_id =null;
    public CommonAction count_down_handler =null;
    public float count_down_duration =-1f;
    public int runtime = 0;
    public float duration = 0;
  }
  Dictionary<string, Session> session_map =new Dictionary<string, Session>();
  
  public string start(float duration, CommonAction handler){
    string session_id =System.Guid.NewGuid().ToString();
    if (handler !=null){
      Debug.Log("10 - start count down (duration:"+duration+", session:"+session_id+")");
    
      Session s =new Session();
      s.session_id =session_id;
      s.count_down_duration =duration;
      s.count_down_handler =handler;
      session_map.Add(s.session_id, s);
    }

    return session_id;
  }


  //runtime����X���Aduration�C������ɶ��Ainitrun�Ұʷ�U�N������@��
  public string start(int runtime, float duration, CommonAction handler, bool initrun = true){
    string session_id = System.Guid.NewGuid().ToString();
    if (handler != null){
      Debug.Log("10 - start count down (runtime : "+ runtime + ", duration:" + duration + ", session:" + session_id + ")");

      Session s = new Session();
      s.session_id = session_id;
      s.count_down_duration = duration;
      s.count_down_handler = handler;
      s.runtime = runtime;
      s.duration = duration;
      if (initrun){
        if (s.count_down_handler != null){
          s.count_down_handler();
          s.runtime--;
        }
      }
      session_map.Add(s.session_id, s);

    }

    return session_id;
  }

  public void stop(string session_id){
    if (session_map.ContainsKey(session_id)){
      session_map.Remove(session_id);
    }
  }

  public float getSessionTime(string id){
    if (session_map.ContainsKey(id) == false)
      return 0.0f;

    return session_map[id].count_down_duration;
  }

  public void stop_all(){
    session_map.Clear();
  }

  public string start(CommonAction handler){
    string id =start(0f, handler);
    session_map[id].st =SessionType.EACH_FRAME;
    return id;
  }

  public void Update (){
    foreach(KeyValuePair<string, Session> entity in session_map){
      if (entity.Value.st ==SessionType.EACH_FRAME){
        try{
          entity.Value.count_down_handler();
        }catch(System.Exception e){
          Debug.LogError(e.ToString());
        }
      }
    }

    foreach(KeyValuePair<string, Session> entity in session_map){
      if (entity.Value.st ==SessionType.EACH_FRAME){
        continue;
      }

      if (entity.Value.count_down_duration>0f){
        entity.Value.count_down_duration-=Time.deltaTime;
        if (entity.Value.count_down_duration<=0f){
          entity.Value.count_down_duration = 0.0f;
          Debug.Log("27 - count down times up ! (session:"+entity.Value.session_id+")");
          if (entity.Value.count_down_handler !=null){
            entity.Value.runtime--;
            try{
              entity.Value.count_down_handler();
            }catch(System.Exception e){
              Debug.LogError(e.ToString());
            }


            if(entity.Value.runtime <= 0){
              entity.Value.count_down_handler =null;
              session_map.Remove(entity.Key);
              break;
            }
            else{
              entity.Value.count_down_duration = entity.Value.duration;
              continue;
            }
          }
        }
      }
    }

  }
  
}