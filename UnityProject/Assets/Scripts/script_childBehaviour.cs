﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum childState {idle, walk, run, die, checkMask, showFace};

public class script_childBehaviour : MonoBehaviour
{
    const string trigger_idle = "idle";
    const string trigger_walk = "walk";
    const string trigger_run = "run";
    const string trigger_die = "die";
    const string trigger_checkMask = "checkMask";
    const string trigger_showFace = "showFace";
    Animator anim;
    Rigidbody rb;
    public childState state = childState.idle;
    [Header("PROPERTIES")]
    public bool asthmatic = false;
    [SerializeField] float lifeTime = 2f;

    [Header("MOVEMENT")]
    [SerializeField] float speedMultiplier = 30f;
    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float pctMaxSpeedWalk = 0.5f;

    [Header("CHANGE DIR")]
    [SerializeField] float pointToDirForce = 3f;
    [SerializeField] float minTimeToChangeDir = 0f;
    [SerializeField] float rangeTimeToChangeDir = 1f;
    Vector3 direction = Vector3.zero;


    //CHECKING
    [HideInInspector] public bool checkedMask = false;
    float time = 0f;
    float positionCheckTime = 0f;
    float angleToRot = 0f;


    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("child generated");
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        if(state != childState.idle) changeState(state);
        StartCoroutine("changeDir");

        direction = Vector3.right;

        positionCheckTime = script_gameController.character.GetComponent<script_characterController>().pointAtChildTime;
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.I)) changeState(childState.idle);
        //if (Input.GetKeyDown(KeyCode.O)) changeState(childState.walk);
        //if (Input.GetKeyDown(KeyCode.P)) changeState(childState.run);
        //if (Input.GetKeyDown(KeyCode.L)) changeState(childState.checkMask);

        //behave();

    }

    private void FixedUpdate()
    {
        behave();
    }

    private void LateUpdate()
    {
        clampSpeed();
        //if(state != childState.idle && state != childState.die) transform.LookAt(transform.position + direction.normalized, Vector3.up);
    }

    public void changeState(childState s) {
        if(state != childState.die)
        {
            state = s;
            switch (s)
            {
                case childState.idle:
                    rb.isKinematic = false;
                    anim.SetTrigger(trigger_idle);
                    break;

                case childState.walk:
                    rb.isKinematic = false;
                    anim.SetTrigger(trigger_walk);
                    break;

                case childState.run:
                    rb.isKinematic = false;
                    anim.SetTrigger(trigger_run);
                    break;
                case childState.die:
                    rb.isKinematic = true;
                    anim.SetTrigger(trigger_die);
                    break;
                case childState.checkMask:
                    checkedMask = true;
                    rb.isKinematic = true;
                    time = 0f;
                    Vector3 from = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
                    Vector3 to = Vector3.ProjectOnPlane(script_gameController.character.transform.position - transform.position, Vector3.up).normalized;
                    angleToRot = Vector3.SignedAngle(from, to, Vector3.up);
                    anim.SetTrigger(trigger_checkMask);
                    break;
                case childState.showFace:
                    rb.isKinematic = true;
                    anim.SetTrigger(trigger_showFace);
                    break;
            }
            //Debug.Log(s);
        }
    }

    void behave()
    {
        switch (state)
        {
            case childState.idle:
                behaveIdle();
                break;

            case childState.walk:
                behaveWalk();
                break;

            case childState.run:
                behaveRun();
                break;
            case childState.checkMask:
                behaveCheckMask();
                break;
        }
    }

    void behaveIdle()
    {
        pointToDir();
    }

    void behaveWalk()
    {
        //Debug.Log("walking");
        rb.AddForce(direction * Time.fixedDeltaTime * speedMultiplier);
        pointToDir();
    }

    void behaveRun()
    {
        //Debug.Log("running");
        rb.AddForce(direction * Time.fixedDeltaTime * speedMultiplier);
        pointToDir();
    }

    /*void behaveDie()
    {

    }*/

    void behaveCheckMask()
    {
        //Debug.Log("character is at " + script_gameController.character.transform.position);
        time += Time.fixedDeltaTime;

        transform.RotateAround(transform.position, Vector3.up, angleToRot * Time.fixedDeltaTime / positionCheckTime);

        if (time > positionCheckTime) changeState(childState.showFace);
        Debug.Log("checking mask");
    }

    

    IEnumerator changeDir()
    {
        float time = minTimeToChangeDir + Random.Range(0, rangeTimeToChangeDir);
        yield return new WaitForSeconds(time);
        direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        StartCoroutine("changeDir");
    }

    void clampSpeed()
    {
        Vector3 vel = Vector3.ProjectOnPlane(rb.velocity, Vector3.up);
        float max = state == childState.walk ? max = maxSpeed * pctMaxSpeedWalk : maxSpeed;
        vel = vel.magnitude > max ? vel.normalized * max : vel;

        rb.velocity = new Vector3(vel.x, rb.velocity.y, vel.z);
    }

    public void setAsthmatic()
    {
        asthmatic = true;
        if (asthmatic) transform.name = "child asthmatic";
        StartCoroutine("dieCountDown");
    }

    IEnumerator dieCountDown()
    {
        yield return new WaitForSeconds(lifeTime);
        changeState(childState.die);
        Debug.Log("dead");
    }

    void pointToDir()
    {
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        Debug.Log(angle);
        //if (Mathf.Abs(angle) < 5f) transform.LookAt(transform.position + direction);
        //else transform.RotateAround(transform.position, Vector3.up, angle * Time.fixedDeltaTime * pointToDirForce);
        rb.AddTorque(new Vector3(0f, angle * Time.fixedDeltaTime * pointToDirForce, 0f));
    }

}
