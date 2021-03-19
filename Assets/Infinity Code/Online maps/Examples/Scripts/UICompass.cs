/*     INFINITY CODE 2013-2018      */
/*   http://www.infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsDemos
{
    public class UICompass : MonoBehaviour
    {
        public bool animated = true;
        public float duration = 0.3f;
        public RectTransform arrow;

        private OnlineMapsControlBase3D control;
        private float targetPan;
        private float startPan;
        private float progress;
        private bool isAnim;

        private float rotation
        {
            get { return Mathf.Repeat(control.cameraRotation.y, 360); }
        }

        private void OnCameraControl()
        {
            arrow.rotation = Quaternion.Euler(0, 0, control.cameraRotation.y);
        }

        public void RotateLeft()
        {
            SetPan(((int)(rotation / 90) + 1) * 90);
        }

        public void RotateRight()
        {
            SetPan((Mathf.CeilToInt(rotation / 90) - 1) * 90);
        }

        public void SetNorth()
        {
            SetPan(rotation > 180 ? 360 : 0);
        }

        private void SetPan(float pan)
        {
            if (!animated)
            {
                control.cameraRotation.y = pan;
                return;
            }

            targetPan = pan;
            startPan = rotation;
            progress = 0;
            isAnim = true;
        }

        private void Start()
        {
            control = OnlineMapsControlBase3D.instance;
            if (control == null)
            {
                Debug.LogWarning("UI Compass requires 3D control.");
                Destroy(gameObject);
                return;
            }

            control.OnCameraControl += OnCameraControl;
        }

        private void Update()
        {
            if (!isAnim) return;

            progress += Time.deltaTime / duration;
            if (progress >= 1)
            {
                progress = 1;
                isAnim = false;
            }

            control.cameraRotation.y = Mathf.Lerp(startPan, targetPan, progress);
        }
    }
}