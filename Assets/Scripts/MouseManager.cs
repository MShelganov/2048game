using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private static MouseManager instance;
    public int swipeRadius = 100;
    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool isDraging;
    private Vector2 startTouch, swipeDelta;
    protected Ray ray;
    protected RaycastHit raycastHit;

    #region GETs
    public bool Tap { get => tap; }
    public Vector2 TapPosition { get => startTouch; }
    public bool SwipeLeft { get => swipeLeft; }
    public bool SwipeRight { get => swipeRight; }
    public bool SwipeUp { get => swipeUp; }
    public bool SwipeDown { get => swipeDown; }
    public bool IsDraging { get => isDraging; }
    public static MouseManager Instance { get => instance; }
    #endregion GETs

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;
    }

    void Update()
    {
        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

        // PC input
        if (Input.GetMouseButtonDown(0))
        {
            tap = isDraging = true;
            startTouch = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if (swipeDelta.magnitude > swipeRadius)
            {
                float x = swipeDelta.x;
                float y = swipeDelta.y;
                if (Mathf.Abs(x) >= Mathf.Abs(y))
                {
                    if (x < 0)
                        swipeLeft = true;
                    else
                        swipeRight = true;
                }
                else
                {
                    if (y < 0)
                        swipeDown = true;
                    else
                        swipeUp = true;
                }
            }
            Reset();
        }

        // Mobile input
        if (Input.touches.Length > 0)
        {
            if(Input.touches[0].phase == TouchPhase.Began)
            {
                tap = isDraging = true;
                startTouch = Input.touches[0].position;
            }
            else if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                Reset();
            }
        }

        // Calculate the distance
        swipeDelta = Vector2.zero;
        if (isDraging)
        {
            if (Input.touches.Length > 0)
                swipeDelta = Input.touches[0].position - startTouch;
            else if (Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
        }


        // Keyboard controls
        if (Input.GetKey(KeyCode.W))
            swipeUp = true;
        else if (Input.GetKey(KeyCode.S))
            swipeDown = true;

        if (Input.GetKey(KeyCode.A))
            swipeLeft = true;
        else if (Input.GetKey(KeyCode.D))
            swipeRight = true;
    }

    private void Reset()
    {
        isDraging = false;
        startTouch = swipeDelta = Vector2.zero;
    }
}