using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleBar : MonoBehaviour {
    public GameObject Mask;
    public GameObject Background;
    
    private RectTransform _mask;
    private RectTransform _background;
    
    private bool _busy => StatusManager.Instance.Busy; // Indicate if the player is doing something
    private bool _shouldProgress;

    private Transform _parent; // The parent object to generate the new slots
    private Vector2 _nextSlot; // Indicate the x position to generate the next slot (in pixels)
    
    
    private float _maskPosition;

    public float CurrentLength;
    public GameObject Slot;
    public float Space;

    private void Awake() {
        _parent = Mask.GetComponent<Transform>();
        _mask = Mask.GetComponent<RectTransform>();
        _background = Background.GetComponent<RectTransform>();
    }

    void Start() {
        var sizeDelta = _mask.sizeDelta;
        sizeDelta = new Vector2(0f, sizeDelta.y);
        
        _mask.sizeDelta = sizeDelta;
        _maskPosition = sizeDelta.x; // background.sizeDelta.x - mask.sizeDelta.x;
        _nextSlot = new Vector2(0f, 0f);
        _shouldProgress = false;
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            GenerateNextSlot(CurrentLength);
        }

        if (_busy && _shouldProgress) {
            BarProgress();
        }
    }
    
    public void GenerateNextSlot(float length) {
        Slot.GetComponent<RectTransform>().sizeDelta = new Vector2(length, _background.sizeDelta.y);
        
        var initializedSlot = Instantiate(Slot, _parent, false);
        initializedSlot.GetComponent<RectTransform>().localPosition = _nextSlot;
        
        _nextSlot = new Vector2(_nextSlot.x + length, 0f);
        _maskPosition += length;
    }

    public void BarProgress() {
        var sizeDelta = _mask.sizeDelta;
        
        sizeDelta = new Vector2(sizeDelta.x + Space * Time.deltaTime, sizeDelta.y);
        _mask.sizeDelta = sizeDelta;
        if (_mask.sizeDelta.x >= _maskPosition) {
            _shouldProgress = false;
        }
    }
}