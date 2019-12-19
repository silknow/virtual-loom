using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homography
{

	public static float[] CalcHomographyMatrix(ref Vector3[] src)
    {
        var p00 = src[0];
        var p01 = src[1];
        var p10 = src[2];
        var p11 = src[3];

		var x00 = p00.x;
        var y00 = p00.y;
        var x01 = p01.x;
        var y01 = p01.y;
        var x10 = p10.x;
        var y10 = p10.y;
        var x11 = p11.x;
        var y11 = p11.y;

        var a = x10 - x11;
        var b = x01 - x11;
        var c = x00 - x01 - x10 + x11;
        var d = y10 - y11;
        var e = y01 - y11;
        var f = y00 - y01 - y10 + y11;

        var h13 = x00;
        var h23 = y00;
        var h32 = (c * d - a * f) / (b * d - a * e);
        var h31 = (c * e - b * f) / (a * e - b * d);
        var h11 = x10 - x00 + h31 * x10;
        var h12 = x01 - x00 + h32 * x01;
        var h21 = y10 - y00 + h31 * y10;
        var h22 = y01 - y00 + h32 * y01;

        return new float[] { h11, h12, h13, h21, h22, h23, h31, h32, 1f };		
    }

	public static float[] CalcInverseMatrix(float[] mat)
    {
        var i11 = mat[0];
        var i12 = mat[1];
        var i13 = mat[2];
        var i21 = mat[3];
        var i22 = mat[4];
        var i23 = mat[5];
        var i31 = mat[6];
        var i32 = mat[7];
        var i33 = 1f;
        var a = 1f / (
            + (i11 * i22 * i33)
            + (i12 * i23 * i31)
            + (i13 * i21 * i32)
            - (i13 * i22 * i31)
            - (i12 * i21 * i33)
            - (i11 * i23 * i32)
        );

        var o11 = ( i22 * i33 - i23 * i32) / a;
        var o12 = (-i12 * i33 + i13 * i32) / a;
        var o13 = ( i12 * i23 - i13 * i22) / a;
        var o21 = (-i21 * i33 + i23 * i31) / a;
        var o22 = ( i11 * i33 - i13 * i31) / a;
        var o23 = (-i11 * i23 + i13 * i21) / a;
        var o31 = ( i21 * i32 - i22 * i31) / a;
        var o32 = (-i11 * i32 + i12 * i31) / a;
        var o33 = ( i11 * i22 - i12 * i21) / a;

        return new float[] { o11, o12, o13, o21, o22, o23, o31, o32, o33 };
    }
}
