using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : Singleton<GameSceneManager>
{

    public Dictionary<int, SceneConfig> sceneConfigDic = new Dictionary<int, SceneConfig>();
    SceneConfig mainCurScene;
    public SceneConfig curScene { get { return mainCurScene; } }


    #region 外部方法

    public void DoInit()
    {
        SwitchMainScene(0);//デフォルトシーンを設定する（今操作してるシーン
    }

    public void SwitchMainScene(int sceneId)
    {
        if (sceneConfigDic == null)
        {
            Debug.LogError("[GameSceneManager]Switch main scene failed, sceneConfigDic is null");
            return;
        }

        if (sceneConfigDic.ContainsKey(sceneId) == false || sceneConfigDic[sceneId] == null)
        {
            Debug.LogError("[GameSceneManager]Switch main scene failed, sceneId = " + sceneId);
            return;
        }
        if (mainCurScene == sceneConfigDic[sceneId])
        {
            Debug.LogError("[GameSceneManager]cur main scene already active, sceneId = " + sceneId);
            return;
        }

        if (mainCurScene != null)
        {
            mainCurScene.ActiveScene(false);
        }

        mainCurScene = sceneConfigDic[sceneId];
        mainCurScene.ActiveScene(true);
    }

    public void RegisterScene(SceneConfig config)
    {
        if (config == null) return;

        if (sceneConfigDic == null) sceneConfigDic = new Dictionary<int, SceneConfig>();

        if (sceneConfigDic.ContainsKey(config.SceneID))
        {
            Debug.LogError("[GameSceneManager]sceneConfigDic has the same key, sceneId = " + config.SceneID);
            return;
        }

        sceneConfigDic.Add(config.SceneID, config);

    }

    public void UnregisterScene(SceneConfig config)
    {
        if (config == null) return;

        UnregisterScene(config.SceneID);
    }

    public void UnregisterScene(int sceneID)
    {
        if (sceneConfigDic == null) return;

        if (sceneConfigDic.ContainsKey(sceneID) == false)
        {
            return;
        }

        sceneConfigDic.Remove(sceneID);
    }


    #endregion 外部方法


}
