using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    #region data



    #endregion data

    public void DoInit()
    {

    }

    void InitSetupData()
    {

    }

    #region 外部方法

    #region 测试方法
    public int GetAtkValue(E_UnitType type)
    {
        switch (type)
        {
            case E_UnitType.TestCactust:
                return 1;
            case E_UnitType.TestGreenBat:
                return 3;
            case E_UnitType.TestRedDragon:
                return 5;
            default:
                return 0;
        }
    }


    public int GetHpValue(E_UnitType type)
    {
        switch (type)
        {
            case E_UnitType.TestCactust:
                return 20;
            case E_UnitType.TestGreenBat:
                return 15;
            case E_UnitType.TestRedDragon:
                return 100;
            default:
                return 1;
        }
    }

    #endregion 测试方法

    #endregion 外部方法

}
