using System;
using UnityEngine;
[Serializable]
public class YarnEntity
{
    public int clusterId;
    public Color clusterOriginalColor;
    public int geometryIndex;
    public YarnPanel yarnPanel;
    
    public YarnEntity(Color clusterOriginalColor,int clusterId, YarnPanel yarnPanel,int geometryIndex = -1)
    {
        this.clusterOriginalColor = clusterOriginalColor;
        this.clusterId = clusterId;
        this.geometryIndex = geometryIndex;
        this.yarnPanel = yarnPanel;
    }

    public YarnEntity Clone()
    {
        var clonedEntity = new YarnEntity(this.clusterOriginalColor,this.clusterId,this.yarnPanel,this.geometryIndex);
        return clonedEntity;
    }
}
