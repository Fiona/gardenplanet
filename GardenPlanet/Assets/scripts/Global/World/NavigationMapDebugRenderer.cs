using UnityEngine;
using UnityEngine.AI;

namespace StompyBlondie
{
    public class NavigationMapDebugRenderer: MonoBehaviour
    {

        public NavigationMap navigationMap;
        public float SizeOfNavPoints = .01f;
        public float scale = 1f;
        public Vector3 offset;

        private void OnRenderObject()
        {
            if(navigationMap == null)
                return;
            UltiDraw.Begin();

            var centre = transform.position;
            var rotation = transform.rotation;
            foreach(var point in navigationMap.points)
            {
                var pos = centre + ((point.Key.ToVector3() - offset) * scale);
                pos =  rotation * (pos - centre) + centre;
                UltiDraw.DrawSphere(pos, Quaternion.identity, SizeOfNavPoints, ColorOfNavPoints);
            }

            //UltiDraw.DrawLine();
            UltiDraw.End();

        }

        private Color ColorOfNavPoints = Color.cyan;
    }
}