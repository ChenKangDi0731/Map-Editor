using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCrystal : MonoBehaviour
{
    public E_Group Group;

    [SerializeField] int _crystalID;
    public int CrystalID { 
        get { return _crystalID; }
        set { _crystalID = value; }
    }

    [SerializeField] int default_Hp = 100;
    [SerializeField] int _crystalHp;
    public int CrystalHp
    {
        get { return _crystalHp; }
        set
        {
            _crystalHp = Mathf.Clamp(value, 0, _crystalHp);
        }
    }

    #region 生命周期

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion 生命周期

    #region 外部方法
    public void DoInit()
    {
        if (GameSceneManager.Instance.curScene != null)
        {
            GameSceneManager.Instance.curScene.RegisterCrystal(this);//注册到SceneConfig
        }

        _crystalHp = default_Hp ;
    }

    #endregion 外部方法
}
