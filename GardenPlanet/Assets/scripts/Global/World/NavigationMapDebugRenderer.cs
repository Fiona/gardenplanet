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

        private Color ColorOfNavPoints = Color.blue;
        private Color ColorOfNavLines = Color.cyan;

        private void OnRenderObject()
        {
            if(navigationMap == null)
                return;
            UltiDraw.Begin();

            // Draw points
            foreach(var point in navigationMap.points)
            {
                var drawFrom = CalcualateDrawPos(point.Key);
                UltiDraw.DrawSphere(drawFrom, Quaternion.identity, SizeOfNavPoints, ColorOfNavPoints);

                // Draw link lines
                foreach(var link in point.Value.links)
                {
                    var drawTo = CalcualateDrawPos(link.linkTo);
                    UltiDraw.DrawLine(drawFrom, drawTo, ColorOfNavLines);
                }
            }

            UltiDraw.End();

        }

        private Vector3 CalcualateDrawPos(Pos virtualPos)
        {
            var centre = transform.position;
            var rotation = transform.rotation;

            var pos = centre + ((virtualPos.ToVector3() - offset) * scale);
            return rotation * (pos - centre) + centre;
        }

    }
}