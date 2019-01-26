using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsequenceController : MonoBehaviour
{
    private Image image;
    private Color originalColor;
    private bool FadeInComplete = false;

    // Start is called before the first frame update
    void Start()
    {

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
            image.color = Color.Lerp(image.color, originalColor, 3f *Time.deltaTime);
            FadeInComplete = (image.color == originalColor);
        }
    }
}
