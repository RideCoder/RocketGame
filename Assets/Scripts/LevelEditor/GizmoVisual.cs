using UnityEngine;

public class GizmoVisual : MonoBehaviour
{
    public GameObject[] positionGizmos;
    public GameObject[] rotationGizmos;
    public GameObject[] scaleGizmos;

    public void Awake()
    {
        GizmoHandler.OnModeSwitch += VisualMode;
    }
    public void VisualMode(int mode)
    {
     
        foreach (GameObject go in positionGizmos)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in rotationGizmos)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in scaleGizmos)
        {
            go.SetActive(false);
        }
        if (mode == 0)
        {
            foreach (GameObject obj in positionGizmos)
            {
                obj.SetActive(true);
            }
        }
        if (mode == 2)
        {
            foreach (GameObject obj in rotationGizmos)
            {
                obj.SetActive(true);
            }

        }
        if (mode == 1)
        {
            foreach (GameObject obj in scaleGizmos)
            {
                obj.SetActive(true);
            }
        }
    }
}
