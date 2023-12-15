using System.Drawing;
using UnityEditor;
using UnityEngine;

public class SineVisualisation : MonoBehaviour
{
    [Header("Sine Wave")]
    [SerializeField] private LineRenderer LineRender = null;

    [SerializeField] private int Points = 200;

    [SerializeField] private Vector2 Limits = new(0, 100);

    private void Start()
    {
        if (!LineRender)
        {
            Debug.LogError("One or multiple field unset in SineVisualisation");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }
    }

    public void Draw(float Amplitude, float SinusoidalXCoef,float SinusoidalTimeCoef)
    {
        float xStart = Limits.x;
        float xFinish = Limits.y;

        LineRender.positionCount = Points;
        for (int currentPoint = 0; currentPoint < Points; currentPoint++)
        {
            float progress = (float)currentPoint / (Points - 1);
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = Amplitude * Mathf.Sin((SinusoidalXCoef * x) + (Time.timeSinceLevelLoad * SinusoidalTimeCoef));
            LineRender.SetPosition(currentPoint, new Vector3(x, y, 0));
        }
    }
}