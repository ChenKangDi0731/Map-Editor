using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    public GameObject effectRoot;

    int default_effectID = 0;
    int effectID;
    int maxSetIDTimes = int.MaxValue;
    Dictionary<int, EffectControl> effectDic = new Dictionary<int, EffectControl>();

    bool totalEffectState = false;

    #region 生命周期

    public void DoUpdate(float deltatime)
    {
        if (totalEffectState)
        {
            /*
            foreach(var temp in effectDic)
            {
                if (temp.Value != null)
                {
                    temp.Value.DoUpdate(deltatime);
                }
            }
            */
            Dictionary<int, EffectControl>.ValueCollection.Enumerator tempEnumerator = effectDic.Values.GetEnumerator();
            while (tempEnumerator.MoveNext())
            {
                var cur = tempEnumerator.Current;
                if (cur != null)
                {
                    cur.DoUpdate(deltatime);
                }
            }
        }
    }

    public void DoFixedUpdate(float fixedUpdateTime)
    {
        if (totalEffectState)
        {
            /*
            foreach(var temp in effectDic)
            {
                if (temp.Value != null)
                {
                    temp.Value.DoFixedUpdate(fixedUpdateTime);
                }
            }
            */

            Dictionary<int, EffectControl>.ValueCollection.Enumerator tempEnumerator = effectDic.Values.GetEnumerator();
            while (tempEnumerator.MoveNext())
            {
                var cur = tempEnumerator.Current;
                if (cur != null)
                {
                    cur.DoUpdate(fixedUpdateTime);
                }
            }
        }
    }

    #endregion 生命周期

    #region 外部方法

    #region 生成

    public EffectControl CreateEffect(int effectPrefabsId, Transform parent = null, bool playAtAwake = true)
    {
        //生成GameObject
        GameObject effect = AssetManager.Instance.Create(effectPrefabsId);
        if (effect == null)
        {
            Debug.LogError("[EffectManager]Create effect failed, effectPrefabsID = " + effectPrefabsId);
            return null;
        }

        //获取EffectControl脚本进行配置
        EffectControl control = effect.GetComponent<EffectControl>();
        if (control == null)
        {
            Debug.LogError("[EffectManager]Get EffectControl component failed , effectPrefabsId = " + effectPrefabsId);
            return null;
        }
        RegisterEffect(control);

        control.DoInit(playAtAwake);

        //执行自定义action
        effect.MapAllComponent<ICustomAction>(action =>
        {
            action.Execute();
        });

        return control;
    }

    public EffectControl CreateEffect(GameObject effectPrefab, Transform parent = null,bool playAtAwake=true)
    {
        if (effectPrefab == null)
        {
            Debug.LogError("[EffectManager]Create effect failed, effectPrefabs is null");
            return null;
        }

        //生成GameObject
        GameObject effect = GameObject.Instantiate(effectPrefab);

        if (effect == null)
        {
            Debug.LogError("[EffectManager]Create effect failed ");
            return null;
        }
        if (parent == null &&effectRoot!=null)
        {
            effect.transform.SetParent(effectRoot.transform);
        }
        else
        {
            effect.transform.SetParent(parent);
        }

        //获取EffectControl脚本进行配置
        EffectControl control = effect.GetComponent<EffectControl>();
        if (control == null)
        {
            Debug.LogError("[EffectManager]Get EffectControl component failed ");
            return null;
        }
        RegisterEffect(control);

        control.DoInit(playAtAwake);

        //执行自定义action
        effect.MapAllComponent<ICustomAction>(action =>
        {
            action.Execute();
        });

        return control;
    }

    public EffectControl CreateEffect(E_EffectType type, GameObject effectPrefab, Transform parent = null, bool playAtAwake = true)
    {
        if (effectPrefab == null)
        {
            Debug.LogError("[EffectManager]Create effect failed, effectPrefabs is null");
            return null;
        }

        //生成GameObject
        GameObject effect = GameObject.Instantiate(effectPrefab);

        if (effect == null)
        {
            Debug.LogError("[EffectManager]Create effect failed ");
            return null;
        }
        if (parent == null && effectRoot != null)
        {
            effect.transform.SetParent(effectRoot.transform);
        }
        else
        {
            effect.transform.SetParent(parent);
        }

        //获取EffectControl脚本进行配置
        EffectControl control = effect.AddComponentOnce<EffectControl>();
        if (control == null)
        {
            Debug.LogError("[EffectManager]Get EffectControl component failed ");
            return null;
        }
        RegisterEffect(control);
        control.SetEffectType(type);
        control.DoInit(playAtAwake);

        //执行自定义action
        effect.MapAllComponent<ICustomAction>(action =>
        {
            action.Execute();
        });

        return control;
    }

    #endregion

    public void DoInit()
    {
        effectID = default_effectID;
        totalEffectState = true;

        if (effectDic == null) effectDic = new Dictionary<int, EffectControl>();
        effectDic.Clear();

        totalEffectState = true;
    }

    public void SetState(bool state)
    {
        totalEffectState = state;
    }

    public void RegisterEffect(EffectControl effect)
    {
        if (effect == null) return;
        //int count = 0;
        //do
        //{
        //count++;
        effect.SetEffectID(effectID++);
        //} while (effectDic.ContainsKey(effect.GetActionID()) == false && count < maxSetIDTimes);

        effectDic.Add(effect.GetEffectID(), effect);

    }

    public void UnregisterEffect(EffectControl effect)
    {
        if (effect == null) return;
        UnregisterEffect(effect.GetEffectID());
    }

    public void UnregisterEffect(int id)
    {
        if (effectDic.ContainsKey(id) == false)
        {
            return;
        }

        effectDic.Remove(id);
    }

    public void StartEffect(int id)
    {

    }

    public void StopEffect(int id)
    {

    }

    public void PauseEffect(int id)
    {

    }

    public void ContinueEffect(int id)
    {

    }

    #endregion 外部方法

}
