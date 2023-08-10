using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectControl : MonoBehaviour
{
    private int effectID;

    public E_EffectType effectType;
    public GameObject effectRoot;

    #region 生命周期

    public virtual void OnEnable()
    {
    }

    public virtual void OnDisable()
    {
        effectRoot.SetGOActive(false);
    }

    public virtual void DoUpdate(float deltatime)
    {

    }

    public virtual void DoFixedUpdate(float fixedUpdateTime)
    {

    }

    public virtual void DoLateUpdate(float deltatime)
    {

    }

    #endregion 生命周期

    #region 外部方法

    public virtual void DoInit(bool startAtAwake = true)
    {
        if (effectRoot == null)
        {
            effectRoot = this.gameObject;
        }

        if (startAtAwake)
        {
            StartEffect();//初始化时先停止
        }
        else
        {
            StopEffect();
        }
    }

    public void SetEffectID(int id)
    {
        effectID = id;
    }

    public int GetEffectID()
    {
        return effectID;
    }

    public virtual void StartEffect()
    {
        switch (effectType)
        {
            case E_EffectType.ParticleSystem:

                if (effectRoot == null)
                {
                    Debug.LogError("[EffectControl]effect root is null ,StartEffect failed ,effect id = " + effectID);
                    return;
                }

                effectRoot.SetGOActive(true);

                effectRoot.MapAllComponent<ParticleSystem>(ps =>
                {
                    if (ps != null)
                    {
                        ps.Stop();
                        ps.Play();
                    }
                });

                break;
        }
    }

    public virtual void StopEffect()
    {
        effectRoot.SetGOActive(true);

        switch (effectType)
        {
            case E_EffectType.ParticleSystem:
                if (effectRoot == null)
                {
                    Debug.LogError("[EffectControl]effect root is null ,StartEffect failed ,effect id = " + effectID);
                    return;
                }

                effectRoot.MapAllComponent<ParticleSystem>(ps =>
                {
                    if (ps != null)
                    {
                        ps.Stop();
                    }
                });

                break;
        }

        effectRoot.SetGOActive(false);
    }

    public virtual void PauseEffect()
    {
        switch (effectType)
        {
            case E_EffectType.ParticleSystem:
                if (effectRoot == null)
                {
                    Debug.LogError("[EffectControl]effect root is null ,StartEffect failed ,effect id = " + effectID);
                    return;
                }

                effectRoot.MapAllComponent<ParticleSystem>(ps =>
                {
                    if (ps != null)
                    {
                        ps.Pause();
                    }
                });

                break;
        }
    }

    public virtual void ContinueEffect()
    {
        switch (effectType)
        {
            case E_EffectType.ParticleSystem:
                if (effectRoot == null)
                {
                    Debug.LogError("[EffectControl]effect root is null ,StartEffect failed ,effect id = " + effectID);
                    return;
                }

                effectRoot.MapAllComponent<ParticleSystem>(ps =>
                {
                    if (ps != null)
                    {
                        ps.Play();
                    }
                });

                break;
        }
    }

    public virtual void ResetEffect()
    {
        
    }

    public void SetEffectType(E_EffectType type)
    {
        effectType = type;
    }

    #endregion 外部方法
}


public enum E_EffectType
{
    ParticleSystem=1,
}