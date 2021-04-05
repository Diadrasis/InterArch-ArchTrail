/*         INFINITY CODE         */
/*   https://infinity-code.com   */

/* Special thanks to Brian Chasalow for his help in developing this script. */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnlineMapsRawImageTouchForwarder : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private static List<OnlineMapsRawImageTouchForwarder> forwarders = new List<OnlineMapsRawImageTouchForwarder>();
    private static OnlineMapsRawImageTouchForwarder lastActiveForwarder;

    public RawImage image;
    public OnlineMaps map;
    public RenderTexture targetTexture;

    private OnlineMapsTileSetControl control;

#if !UNITY_EDITOR
    private static Vector2 pointerPos = Vector2.zero;
#endif

    protected Camera worldCamera
    {
        get
        {
            if (image.canvas == null || image.canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
            return image.canvas.worldCamera;
        }
    }

    protected void OnDestroy()
    {
        forwarders.Remove(this);
        if (forwarders.Count == 0)
        {
            control.OnGetInputPosition -= OnGetInputPosition;
            control.OnGetTouchCount -= OnGetTouchCount;
            OnlineMapsGUITooltipDrawer.OnDrawTooltip -= OnDrawTooltip;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
#if !UNITY_EDITOR
        pointerPos = eventData.position;
#endif
    }

    private static void OnDrawTooltip(GUIStyle style, string text, Vector2 position)
    {
        foreach (OnlineMapsRawImageTouchForwarder forwarder in forwarders)
        {
            RectTransform t = forwarder.image.rectTransform;

            Vector2 p = position;

            if (forwarder.targetTexture == null)
            {
                p.x /= Screen.width / t.sizeDelta.x;
                p.y /= Screen.height / t.sizeDelta.y;
            }
            else
            {
                p.x /= forwarder.targetTexture.width / t.sizeDelta.x;
                p.y /= forwarder.targetTexture.height / t.sizeDelta.y;
            }

            p -= t.sizeDelta / 2;

            Vector3 pos = (Vector3)p + forwarder.image.transform.position;

            p = RectTransformUtility.WorldToScreenPoint(forwarder.worldCamera, pos);

            GUIContent tip = new GUIContent(text);
            Vector2 size = style.CalcSize(tip);
            GUI.Label(new Rect(p.x - size.x / 2 - 5, Screen.height - p.y - size.y - 20, size.x + 10, size.y + 5), text, style);
        }
    }

    private static Vector2 OnGetInputPosition()
    {
        Vector2 pos;
        for (int i = 0; i < forwarders.Count; i++)
        {
            var forwarder = forwarders[i];
#if UNITY_EDITOR
            if (forwarder.ProcessTouch(Input.mousePosition, out pos)) return pos;
#else
            if (Input.touchSupported) 
            {
                if (forwarder.ProcessTouch(pointerPos, out pos)) return pos;
            }
            else 
            {
                if (forwarder.ProcessTouch(Input.mousePosition, out pos)) return pos;
            }
#endif
        }

        return Vector2.zero;
    }

    private static Vector2[] OnGetMultitouchInputPositions()
    {
        if (lastActiveForwarder == null) return null;

        Vector2[] touches = Input.touches.Select(t => t.position).ToArray();

        Vector2 p;
        for (int i = 0; i < touches.Length; i++)
        {
            lastActiveForwarder.ProcessTouch(touches[i], out p, false);
            touches[i] = p;
        }

        return touches;
    }

    private static int OnGetTouchCount()
    {
#if UNITY_EDITOR
        return Input.GetMouseButton(0) ? 1 : 0;
#else
        if (Input.touchSupported) return Input.touchCount;
        return Input.GetMouseButton(0) ? 1 : 0;
#endif
    }

    public void OnPointerDown(PointerEventData eventData)
    {
#if !UNITY_EDITOR
        pointerPos = eventData.position;
        lastActiveForwarder = this;
#endif
    }

    private bool ProcessTouch(Vector2 inputTouch, out Vector2 localPosition, bool checkRect = true)
    {
        localPosition = Vector2.zero;
        Vector2 pos = inputTouch;

        RectTransform t = image.rectTransform;
        Vector2 sizeDelta = t.rect.size;
        if ((int)sizeDelta.x == 0 || (int)sizeDelta.y == 0) return false;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, pos, worldCamera, out pos)) return false;
        if (checkRect && !t.rect.Contains(pos)) return false;

        pos += sizeDelta / 2.0f;

        if (targetTexture == null)
        {
            pos.x *= Screen.width / sizeDelta.x;
            pos.y *= Screen.height / sizeDelta.y;
        }
        else
        {
            pos.x *= targetTexture.width / sizeDelta.x;
            pos.y *= targetTexture.height / sizeDelta.y;
        }

        localPosition = pos;

        return true;
    }

    private void Start()
    {
        if (map == null) map = OnlineMaps.instance;
        control = map.control as OnlineMapsTileSetControl;

        if (forwarders.Count == 0)
        {
            control.OnGetInputPosition += OnGetInputPosition;
            control.OnGetMultitouchInputPositions += OnGetMultitouchInputPositions;
            control.OnGetTouchCount += OnGetTouchCount;

            map.notInteractUnderGUI = false;
            control.checkScreenSizeForWheelZoom = false;

            OnlineMapsGUITooltipDrawer.OnDrawTooltip += OnDrawTooltip;
        }

        forwarders.Add(this);
    }
}
