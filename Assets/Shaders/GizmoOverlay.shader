Shader "Editor/GizmoOverlay"
{
    Properties
    {
        _Color ("Gizmo Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
            "RenderType" = "Overlay"
        }

        ZTest Always
        ZWrite Off
        Cull Off
        Lighting Off
        Fog { Mode Off }

        Pass
        {
            Color [_Color]
        }
    }
}
