using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour {
	public event Action OnClick;
    //public event Action Hover;

	public Transform CenterPos;
	public Transform RefPos;
	public Vector3 PinPos;

    public GameObject ConsequencePrefab;
    private GameObject consequence = null;

    private Image image;
    private Color originalColor;
    private bool FadeInComplete = false;

    public bool isEmergency = false;

    public void HandleClick() {
		OnClick?.Invoke();
	}

    public void MouseOver()
    {
        if(!isEmergency)
            consequence = Instantiate(ConsequencePrefab, PinPos + new Vector3(350, 0, 0), Quaternion.identity, transform);
    }

    public void MouseExit()
    {
        if (consequence != null)
            Destroy(consequence);
    }

    void Awake()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!FadeInComplete)
        {
            image.color = Color.Lerp(image.color, originalColor, 10f * Time.deltaTime);
            FadeInComplete = (image.color == originalColor);
        }
    }

}