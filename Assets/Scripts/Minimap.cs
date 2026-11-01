using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// Draws the whole map in the top-right corner of the screen, with a dot marking the player.
// A second camera renders the map into a texture, and a UI image shows that texture.
// Press M to show or hide it.
public class Minimap : MonoBehaviour
{
    [SerializeField] private Tilemap mapBounds;
    [SerializeField] private Transform player;
    [SerializeField] private float height = 260f;
    [SerializeField] private float margin = 20f;
    [SerializeField] private float border = 4f;
    [SerializeField] private float dotSize = 10f;
    [SerializeField] private Color frameColor = new Color(0f, 0f, 0f, 0.85f);
    [SerializeField] private Color playerColor = new Color(1f, 0.85f, 0.2f, 1f);

    private Bounds worldBounds;
    private Camera minimapCamera;
    private GameObject canvasObject;
    private RectTransform playerDot;

    private void Start()
    {
        mapBounds.CompressBounds();
        worldBounds = mapBounds.localBounds;
        worldBounds.center = mapBounds.transform.TransformPoint(worldBounds.center);

        float aspect = worldBounds.size.x / worldBounds.size.y;
        var texture = new RenderTexture(Mathf.RoundToInt(height * aspect * 2f), Mathf.RoundToInt(height * 2f), 16);

        CreateCamera(texture);
        CreateUI(texture, aspect);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            bool visible = !canvasObject.activeSelf;
            canvasObject.SetActive(visible);
            minimapCamera.enabled = visible;
        }

        if (player != null)
        {
            Vector2 normalized = new Vector2(
                Mathf.InverseLerp(worldBounds.min.x, worldBounds.max.x, player.position.x),
                Mathf.InverseLerp(worldBounds.min.y, worldBounds.max.y, player.position.y));
            playerDot.anchorMin = normalized;
            playerDot.anchorMax = normalized;
        }
    }

    private void CreateCamera(RenderTexture texture)
    {
        var cameraObject = new GameObject("Minimap Camera");
        cameraObject.transform.SetParent(transform, false);
        cameraObject.transform.position = new Vector3(worldBounds.center.x, worldBounds.center.y, -10f);

        minimapCamera = cameraObject.AddComponent<Camera>();
        minimapCamera.orthographic = true;
        minimapCamera.orthographicSize = worldBounds.extents.y;
        minimapCamera.clearFlags = CameraClearFlags.SolidColor;
        minimapCamera.backgroundColor = Color.black;
        minimapCamera.targetTexture = texture;
    }

    private void CreateUI(RenderTexture texture, float aspect)
    {
        canvasObject = new GameObject("Minimap Canvas", typeof(Canvas), typeof(CanvasScaler));
        canvasObject.transform.SetParent(transform, false);
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 1f;

        Image frame = CreateGraphic<Image>("Frame", canvasObject.transform);
        frame.color = frameColor;
        RectTransform frameRect = frame.rectTransform;
        frameRect.anchorMin = Vector2.one;
        frameRect.anchorMax = Vector2.one;
        frameRect.pivot = Vector2.one;
        frameRect.anchoredPosition = new Vector2(-margin, -margin);
        frameRect.sizeDelta = new Vector2(height * aspect + border * 2f, height + border * 2f);

        RawImage map = CreateGraphic<RawImage>("Map", frame.transform);
        map.texture = texture;
        RectTransform mapRect = map.rectTransform;
        mapRect.anchorMin = Vector2.zero;
        mapRect.anchorMax = Vector2.one;
        mapRect.offsetMin = new Vector2(border, border);
        mapRect.offsetMax = new Vector2(-border, -border);

        Image dot = CreateGraphic<Image>("Player Dot", map.transform);
        dot.color = playerColor;
        playerDot = dot.rectTransform;
        playerDot.sizeDelta = new Vector2(dotSize, dotSize);
        playerDot.anchoredPosition = Vector2.zero;
    }

    private static T CreateGraphic<T>(string name, Transform parent) where T : Graphic
    {
        var graphicObject = new GameObject(name, typeof(RectTransform), typeof(T));
        graphicObject.transform.SetParent(parent, false);
        var graphic = graphicObject.GetComponent<T>();
        graphic.raycastTarget = false;
        return graphic;
    }
}
