using UnityEditor;
using UnityEngine;

public class SineVisualisation : MonoBehaviour
{
    [Header("Sine Wave")]
    [SerializeField] private LineRenderer LineRender = null;

    [SerializeField] private int Points = 10;

    [SerializeField] private float Amplitude = 1;

    [SerializeField] private float Frequency = 1;

    [SerializeField] private Vector2 Limits = new(0, 1);

    [SerializeField] private float MovementSpeed = 1;

    [SerializeField, Range(0, 2 * Mathf.PI)] private float Radians = 0;

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

    private void Update()
    {
        Draw();
    }

    private void Draw()
    {
        float xStart = Limits.x;
        float Tau = 2 * Mathf.PI;
        float xFinish = Limits.y;

        LineRender.positionCount = Points;
        for (int currentPoint = 0; currentPoint < Points; currentPoint++)
        {
            float progress = (float)currentPoint / (Points - 1);
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = Amplitude * Mathf.Sin((Tau * Frequency * x) + (Time.timeSinceLevelLoad * MovementSpeed));
            LineRender.SetPosition(currentPoint, new Vector3(x, y, 0));
        }
    }
}