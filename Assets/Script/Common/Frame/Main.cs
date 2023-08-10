using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    #region mono_lifeCycle

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        MainController.Instance.DoInit();
    }

    // Update is called once per frame
    void Update()
    {
        MainController.Instance.DoUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        MainController.Instance.DoFixedUpdate(Time.fixedDeltaTime);
    }

    #endregion mono_lifeCycle
}
