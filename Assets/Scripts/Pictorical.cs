using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARTEC.Curves;

[Serializable]
public class Pictorical
{
    public Curve curve;
    public Pattern drawing;
    public ScriptableYarn yarn;
    public bool adjusted=true;
    public int healedStep = -1;
    public int healedStepGap = -1;
    public bool doubleHealed = false;
    public int[] firstPoint { set; get; }
    public int[] lastPoint{ set; get; }


    public int CalculateBackDepth(Patch patch, int index, int column, int row)
    {
        int acum = -1;
        for (int i=0;i<index;i++)
            if (patch.pictoricals[i].IsInBack(column,row))
                acum--;
        return acum;
    }

    public bool IsInBack(int column, int row)
    {
        if (firstPoint[row]!=-1 && (lastPoint[row]-firstPoint[row])>2)
            if (firstPoint[row] < column && column < lastPoint[row])
                return (!drawing.Value(column, row));
        return false;
    }

    public void CalculateFirstAndLastPointOfRows(Vector2Int resolution)
    {

        firstPoint = new int[resolution.y];
        lastPoint = new int[resolution.y];
        for (int row = 0; row < resolution.y; row++)
        {
            if (adjusted)
            {
                lastPoint[row] = firstPoint[row] = -1;
                for (int column=0;column<resolution.x;column++)
                    if (drawing.Value(column, row))
                    {
                        firstPoint[row] = Math.Max(column-1,0);
                        break;
                    }
                for (int column=resolution.x-1;column>firstPoint[row];column--)
                    if (drawing.Value(column, row))
                    {
                        lastPoint[row] = Math.Min(column+1,resolution.x);
                        break;
                    }
            }
            else
            {
                {
                    firstPoint[row] = 0;
                    lastPoint[row] = resolution.x;
                }
            }
        }
    }
}
