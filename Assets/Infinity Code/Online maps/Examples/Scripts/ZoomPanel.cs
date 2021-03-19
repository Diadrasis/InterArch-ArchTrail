/*     INFINITY CODE 2013-2018      */
/*   http://www.infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsDemos
{
    [AddComponentMenu("Infinity Code/Online Maps/Demos/Zoom Panel")]
    public class ZoomPanel : MonoBehaviour
    {
        public void SetZoom(int zoom)
        {
            OnlineMaps.instance.zoom = zoom;
        }

        public void ZoomIn()
        {
            OnlineMaps.instance.zoom++;
        }

        public void ZoomOut()
        {
            OnlineMaps.instance.zoom--;
        }
    }
}