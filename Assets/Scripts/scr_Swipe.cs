using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class scr_Swipe : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Instantiator commander;
    private Vector3 PosInit;

    private float fDistanceMoved;
    private bool SwipeLeft;
    private int speed = 4;

    public event Action aCardMoved;
    public string audioReference;
    private void Start()
    {
        commander = FindObjectOfType<Instantiator>();
        if (audioReference == "")
            noAudio();
        else
            hasAudio();
    }
    public void noAudio()
    {
        commander.audioButton.SetActive(false);
    }
    public void hasAudio()
    {
        commander.RefToAudio(audioReference);
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = new Vector2(transform.localPosition.x + eventData.delta.x, transform.localPosition.y);

        if (transform.localPosition.x - PosInit.x > 0)
        {

            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, -30, (PosInit.x + transform.localPosition.x)/(Screen.width/2 )));
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0,  30, (PosInit.x - transform.localPosition.x) / (Screen.width / 2)));
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        PosInit = transform.localPosition;

    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        fDistanceMoved = Mathf.Abs(transform.localPosition.x - PosInit.x);
        if (fDistanceMoved < 0.4 * Screen.width)
        {
            transform.localPosition = PosInit;
            transform.localEulerAngles = Vector3.zero; 

        }
        else
        {
            if (transform.localPosition.x > PosInit.x)
            {
                SwipeLeft = false;
            }
            else
            {
                SwipeLeft = true;
            }
            Next();
             
        }
        
    }
    private IEnumerator MoveCard()
    {
        float time = 0;
        while (GetComponent<Image>().color != new Color(1, 1, 1, 0))
        {
            time += Time.deltaTime;
            if (SwipeLeft)
            {
                transform.localPosition = new Vector3(Mathf.SmoothStep(transform.localPosition.x, transform.localPosition.x - Screen.width, speed * time), transform.localPosition.y, 0);
            }
            else
            {
                transform.localPosition = new Vector3(Mathf.SmoothStep(transform.localPosition.x, transform.localPosition.x +  Screen.width, speed * time), transform.localPosition.y, 0);
            }
            GetComponent<Image>().color = new Color (1 ,1, 1, Mathf.SmoothStep(1, 0, speed*time));
            yield return null;
        }
        Destroy(gameObject);
    }

    void Next()
    {
        aCardMoved?.Invoke();
        StartCoroutine(MoveCard());
    }

    private void Update()
    {

        if (commander.Debugging)
        {
            if (Input.anyKeyDown)
            {
                Next();
            }
        }
        

    }
} 

