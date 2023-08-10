using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_UnitMovement : MonoBehaviour
{
    #region Component
    public GameObject Target;
    public Collider c;
    public Rigidbody r;

    public Animator animator;

    #endregion Component

    #region state_param

    public bool isStartMovement = false;
    public bool isTriggerAnim = false;//is dead

    #endregion state_param

    #region movement_param
    public float moveSpeed;

    Vector3 startPos = Vector3.zero;
    Vector3 targetPos = Vector3.zero;
    #endregion movement_param

    #region exploreAnim_param

    public float min_upForce;
    public float max_upForce;
    public float forceFactor;

    #endregion exploreAnim_param

    #region other_param

    Coroutine cr_delayStartMovement;

    #endregion other_param

    #region mono_lifeCycle

    // Start is called before the first frame update
    void Start()
    {
        if (c == null)
        {
            c = this.GetComponent<Collider>();
            if (c == null)
            {
                Debug.LogError("[--- Get Obj collider failed, gameobject name = " + this.gameObject.name);
            }
        }


        if (r == null)
        {
            r = this.GetComponent<Rigidbody>();
            if (r == null)
            {
                Debug.LogError("[--- Get Obj rigidbody failed, gameObject name = " + this.gameObject.name);
            }
        }

        if (animator == null)
        {
            animator = this.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[--- Get Obj animator failed, gameObject name = " + this.gameObject.name);
            }
        }


        StopMovement();

        cr_delayStartMovement = this.Delay(3f, () =>
        {
            //StartMovement();
            ActiveCollider(true);
            ActiveRigidbodyGravity(false);
        });


    }

    // Update is called once per frame
    void Update()
    {
        return;
        //hide obj
        if (isTriggerAnim)
        {
            if (transform.position.y < -10)
            {
                ActiveRigidbodyGravity(false);
                isTriggerAnim = false;
                this.gameObject.SetActive(false);

            }
        }

        if (isStartMovement == false) return;

        if (r == null || Target == null)
        {
            isStartMovement = false;
            Debug.LogError("[--- Target is null ,Start movement failed , gameObject name = " + this.gameObject.name);
            return;
        }

        Vector3 dir = (Target.transform.position - this.transform.position).normalized;
        Vector3 newPosition = transform.position + dir * moveSpeed * Time.deltaTime;

        r.MovePosition(newPosition);
        //transform.position = transform.position + dir * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Demo_GearTarget gearTarget;
        if(other.gameObject.TryGetComponent(out gearTarget)==false)
        {
            return;
        }

        StopMovement();
        ActiveCollider(false);
        TriggerForceAnim();
    }



    #endregion mono_lifeCycle

    public void StartMovement()
    {
        if (Target == null)
        {
            Debug.LogError("[--- Start movement failed, target is null, gameObject name = " + this.gameObject.name);
            return;
        }

        startPos = this.transform.position;
        targetPos = Target.transform.position;
        isStartMovement = true;

        ActiveCollider(true);
    }

    public void StopMovement()
    {
        isStartMovement = false;
    }

    public void TriggerForceAnim()
    {
        isTriggerAnim = true;

        //calc force
        Vector3 dir = Vector3.one;
        if (Target == null)
        {
            dir = -(Target.transform.position - this.transform.position).normalized;
        }
        else
        {
            dir = -this.transform.forward;
        }

        float upForce = UnityEngine.Random.Range(min_upForce, max_upForce);
        dir.y += upForce;
        dir *= forceFactor;

        ActiveRigidbodyGravity(true);

        //SetForce(dir);
        SetForcePosition(dir, new Vector3(0, -0.5f, 0.5f));
    }

   public void ActiveCollider(bool active)
    {
        if (c == null) return;

        if (c.enabled != active)
        {
            c.enabled = active;
        }
    }

    public void ActiveRigidbodyGravity( bool activeGravity = true)
    {
        if (r == null) return;
        if (r.useGravity != activeGravity)
        {
            r.useGravity = activeGravity;
        }
    }

    public void SetForce(Vector3 force, ForceMode mode=ForceMode.Force)
    {
        if (r == null) return;

        Debug.Log("<color=#ff0000> trigger force anim,force = "+force +" </color>");


        r.AddForce(force * 10, mode);
    }

    public void SetForcePosition(Vector3 force,Vector3 forcePosition, ForceMode mode = ForceMode.Force)
    {
        if (r == null) return;

        Debug.Log("<color=#ff0000> trigger force anim,force = " + force + " </color>");

        r.AddForceAtPosition(force * 10f, forcePosition, mode);

    }
}
