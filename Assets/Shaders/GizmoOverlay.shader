Shader "Editor/GizmoOverlay"
{
    SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
            "RenderType" = "Overlay"
        }

        ZTest Always       // 👈 Ignore depth buffer
        ZWrite Off         // 👈 Do not write depth
        Cull Off           // 👈 Visible from both sides
        Lighting Off
        Fog { Mode Off }

        Pass
        {
            Color (1,1,1,1)
        }
    }
}
