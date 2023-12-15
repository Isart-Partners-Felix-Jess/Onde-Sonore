using UnityEditor;
using UnityEngine;

public class PhysicSimulation : MonoBehaviour
{
    private UIManager UIref;

    [Header("Assets")]
    [SerializeField] private SpriteRenderer FireTruck = null;

    [SerializeField] private SpriteRenderer Human = null;

    [SerializeField] private SpriteRenderer House = null;

    [Header("Variables")]
    [SerializeField] private int EndOfSimulationOnX;

    private Vector3 FireTruckAtStart; // Start pos is set in Editor

    private void Start()
    {
        if (!FireTruck || !Human || !House || EndOfSimulationOnX == 0)
        {
            Debug.LogError("One or multiple field unset in PhysicSimulation");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        UIref = UIManager.Instance;
        FireTruckAtStart = FireTruck.transform.position;
    }

    private void Update()
    {
        if (UIref.IsStarted && !UIref.IsPaused)
        {
            Movements();

            if (FireTruck.transform.localPosition.x >= EndOfSimulationOnX)
            {
                UIref.Reset();
                Reset();
            }
        }
    }

    private void Movements()
    {
        FireTruck.transform.position += new Vector3(Time.deltaTime * UIref.TruckSpeed, 0f, 0f);
    }

    private float GetDistance(SpriteRenderer obj1, SpriteRenderer obj2)
    {
        return (obj1.transform.position - obj2.transform.position).magnitude;
    }

    public void Reset()
    {
        FireTruck.transform.position = FireTruckAtStart;
    }
}