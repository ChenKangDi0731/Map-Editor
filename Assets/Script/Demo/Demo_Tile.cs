using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_Tile : MonoBehaviour
{

    #region component
    private Transform _transform;
    public Transform tileTrans { get
        {
            if (_transform == null)
            {
                _transform = tileGo.transform;
            }
            return _transform;
        }
    }

    private GameObject _gameObject;
    public GameObject tileGo
    {
        get
        {
            if (_gameObject == null) _gameObject = this.gameObject;
            return _gameObject;
        }
    }

    public Vector3 tilePos { get { return tileTrans.position+offsetVector; } }
    #endregion component

    public float upOffsetValue = 1f;

    #region tile_param
    public bool isStartTile = false;
    public bool isDestination=false;
    public Demo_Tile nextTile;//改成寻路配置

    #endregion tile_param

    Vector3 offsetVector = Vector3.zero;

    #region lifeCycle

    // Start is called before the first frame update
    void Start()
    {
        offsetVector.y = upOffsetValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region 外部方法

    public bool TryGetNextTile(out Demo_Tile tile)
    {
        if (nextTile == null || isDestination)
        {
            tile = null;
            return false;
        }
        tile = nextTile;
        return true;
    }

    #endregion 外部方法
}
