Shader "Lynx/Occluder" {
    SubShader {
        Tags { "Queue" = "Geometry-100" }
        ColorMask 0
        ZWrite On
        Cull Off
        Pass { }
    }
}