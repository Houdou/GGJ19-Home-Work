using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleBar : MonoBehaviour {
    private RectTransform mask;
    private RectTransform background;
    private Transform parent; //the parent object to generate the new slots
    private Vector2 nextSlot; //indicate the x position to generate the next slot (in pixels)
    private bool progressing; //indicate if the player is doing something
    private GameObject initializedSlot;
    
    private float maskPosition;

    public float currentLength;
    public GameObject slot;
    public float space;

    private void Awake() {
        var maskFind = GameObject.Find("Mask");
        parent = maskFind.GetComponent<Transform>();
        mask = maskFind.GetComponent<RectTransform>();
        background = GameObject.Find("Background").GetComponent<RectTransform>();
    }

    void Start() {
        mask.sizeDelta = new Vector2(0f, mask.sizeDelta.y);
        maskPosition = mask.sizeDelta.x; //background.sizeDelta.x - mask.sizeDelta.x;
        nextSlot = new Vector2(0f, 0f);
        progressing = false;
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            GenerateNextSlot(currentLength);
            progressing = true;
        }

        //Debug.Log(maskPosition.ToString() + " - " + (background.sizeDelta.x - mask.sizeDelta.x).ToString());
        //if(background.sizeDelta.x - mask.sizeDelta.x<=maskPosition)
        if (progressing) {
            BarProgress();
        }

    }

    public void NextSlotPosition(float length) {
        nextSlot = new Vector2(nextSlot.x + length, 0f);
    }

    public void GenerateNextSlot(float length) {
        slot.GetComponent<RectTransform>().sizeDelta = new Vector2(length, background.sizeDelta.y);
        initializedSlot = Instantiate(slot, parent, false);
        initializedSlot.GetComponent<RectTransform>().localPosition = nextSlot;
        NextSlotPosition(length);
        maskPosition += length;
    }

    public void BarProgress() {
        var sizeDelta = mask.sizeDelta;
        sizeDelta = new Vector2(sizeDelta.x + space * Time.deltaTime, sizeDelta.y);
        mask.sizeDelta = sizeDelta;
        if (mask.sizeDelta.x >= maskPosition)
            progressing = false;
    }
}