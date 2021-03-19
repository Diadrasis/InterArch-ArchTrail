/*     INFINITY CODE 2013-2018      */
/*   http://www.infinity-code.com   */

using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsDemos
{
    [AddComponentMenu("Infinity Code/Online Maps/Demos/Demo")]
    public class Demo : MonoBehaviour
    {
        public Text twoThreeDText;
        public Toggle labelsToggle;
        public Toggle trafficToggle;
        public Toggle elevationsToggle;

        public Transform camera2D;
        public Transform camera3D;

        public Shader tileShader;

        public float CameraChangeTime = 1;

        private GUIStyle activeRowStyle;
        private float animValue;
        private OnlineMaps map;
        private OnlineMapsTileSetControl control;
        private bool is2D = true;
        private bool isCameraModeChange;
        private GUIStyle rowStyle;
        private string search = "";
        private OnlineMapsMarker searchMarker;

        private Transform fromTransform;
        private Transform toTransform;
        private bool preventDoubleWarning;

        public void ChangeMode()
        {
            if (isCameraModeChange) return;
            is2D = !is2D;

            twoThreeDText.text = is2D ? "3D" : "2D";
            elevationsToggle.gameObject.SetActive(!is2D);

            animValue = 0;
            isCameraModeChange = true;

            Camera c = Camera.main;
            fromTransform = is2D ? camera3D : camera2D;
            toTransform = is2D ? camera2D : camera3D;

            c.orthographic = false;
            if (!is2D) c.fieldOfView = 28;
        }

        public void SetLabels()
        {
            map.labels = labelsToggle.isOn;
            map.Redraw();
        }

        public void SetTraffic()
        {
            map.traffic = trafficToggle.isOn;
            map.Redraw();
        }

        public void SetElevations()
        {
            if (preventDoubleWarning)
            {
                preventDoubleWarning = false;
                return;
            }
            if (!OnlineMapsKeyManager.hasBingMaps || string.IsNullOrEmpty(control.bingAPI))
            {
                Debug.LogWarning("Please enter Map / Key Manager / Bing Maps");
                preventDoubleWarning = true;
                elevationsToggle.isOn = false;
                return;
            }

            control.useElevation = elevationsToggle.isOn;
            map.Redraw();
        }

        private void OnFindLocationComplete(string result)
        {
            Vector2 position = OnlineMapsGoogleGeocoding.GetCoordinatesFromResult(result);

            if (position == Vector2.zero) return;

            if (searchMarker == null) searchMarker = map.AddMarker(position, search);
            else
            {
                searchMarker.position = position;
                searchMarker.label = search;
            }

            if (map.zoom < 13) map.zoom = 13;

            map.position = position;
            map.Redraw();
        }

        private void Start()
        {
            map = OnlineMaps.instance;
            control = OnlineMapsTileSetControl.instance;
            control.useElevation = false;
        }

        private void Update()
        {
            if (!isCameraModeChange) return;

            animValue += Time.deltaTime / CameraChangeTime;

            if (animValue > 1)
            {
                animValue = 1;
                isCameraModeChange = false;
            }

            Camera c = Camera.main;

            c.transform.position = Vector3.Lerp(fromTransform.position, toTransform.position, animValue);
            c.transform.rotation = Quaternion.Lerp(fromTransform.rotation, toTransform.rotation, animValue);

            float fromFOV = is2D ? 60 : 28;
            float toFOV = is2D ? 28 : 60;

            c.fieldOfView = Mathf.Lerp(fromFOV, toFOV, animValue);

            if (!isCameraModeChange && is2D) c.orthographic = true;
        }
    }
}