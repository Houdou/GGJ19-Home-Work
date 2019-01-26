using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleBar : MonoBehaviour
{
    private RectTransform holder; //slot holder
    private RectTransform mask;
    private RectTransform background;
    private Transform parent; //the parent object to generate the new slots
    private Vector2 nextSlot; //indicate the x position to generate the next slot (in pixels)
    private float maskPosition;
    private bool progressing; //indicate if the player is doing something
    private GameObject initializedSlot;

    public int currentLength;
    public GameObject slot;
    public float space;

    void Start()
    {
        parent = GameObject.Find("Generator").GetComponent<Transform>();
        holder = GameObject.Find("Generator").GetComponent<RectTransform>();
        mask = GameObject.Find("Mask").GetComponent<RectTransform>();
        background = GameObject.Find("Background").GetComponent<RectTransform>();

        mask.sizeDelta = background.sizeDelta;
        maskPosition = background.sizeDelta.x - mask.sizeDelta.x;
        nextSlot = new Vector2(0, 0);
        progressing = false;
    }


    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GenerateNextSlot(currentLength);
        }
        Debug.Log(maskPosition.ToString() + " - " + (background.sizeDelta.x - mask.sizeDelta.x).ToString());
        if(background.sizeDelta.x - mask.sizeDelta.x<=maskPosition)
        {
            BarProgress();
        }
        
    }

    void NextSlotPosition(int length)
    {
        nextSlot = new Vector2(nextSlot.x + length, 0);

    }

    void GenerateNextSlot(int length)
    {
        slot.GetComponent<RectTransform>().sizeDelta = new Vector2(length,20);
        initializedSlot = Instantiate(slot, parent, false);
        initializedSlot.GetComponent<RectTransform>().localPosition = nextSlot;
        NextSlotPosition(length);
        maskPosition += length;
    }

    void BarProgress()
    {
        while(background.sizeDelta.x - mask.sizeDelta.x < maskPosition)
        {
            mask.sizeDelta = new Vector2(mask.sizeDelta.x - space * Time.deltaTime, 20);
        }
        progressing = false;
    }
}
