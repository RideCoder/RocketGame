using UnityEngine;
using UnityEngine.UI;

public class RawImageScroller : MonoBehaviour
{
    public float scrollSpeedX = 0.5f; // Horizontal scroll
    public float scrollSpeedY = 0f;   // Vertical scroll

    private RawImage rawImage;
    private Vector2 uvOffset;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        uvOffset = rawImage.uvRect.position;
    }

    void Update()
    {
        // Increment the UV offset over time
        uvOffset.x += scrollSpeedX * Time.deltaTime;
        uvOffset.y += scrollSpeedY * Time.deltaTime;

        // Keep the offset looping between 0 and 1
        uvOffset.x %= 1f;
        uvOffset.y %= 1f;

        rawImage.uvRect = new Rect(uvOffset, rawImage.uvRect.size);
    }
}
