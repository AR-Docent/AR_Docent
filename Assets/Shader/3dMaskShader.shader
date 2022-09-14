Shader "Custom/3dMaskShader"
{
    SubShader {
        Tags { "Queue" = "Geometry+10" }

        ColorMask 0
	    ZWrite On

        Pass {}
    }
}
