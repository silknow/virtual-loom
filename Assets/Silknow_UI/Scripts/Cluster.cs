using OpenCVForUnity.CoreModule;
using UnityEngine;

namespace Silknow_UI.Scripts
{
    public class Cluster
    {
        public enum YarnZone
        {
            Warp,
            Weft,
            Pictorial
        }
        public Color color;
        public int index;
        public Mat imageMask;
        
        public Cluster(Color color, int index)
        {
            this.color = color;
            this.index = index;
        }
    }
}