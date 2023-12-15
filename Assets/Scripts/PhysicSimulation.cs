using UnityEditor;
using UnityEngine;

public class PhysicSimulation : MonoBehaviour
{
    private UIManager UIref;

    [Header("Assets")]
    [SerializeField] private SpriteRenderer FireTruck = null;

    [SerializeField] private Transform Human = null;

    [SerializeField] private Transform House = null;

    [SerializeField] private Transform Window = null;

    [Header("Variables")]
    [SerializeField] private int EndOfSimulationOnX;

    private Vector3 FireTruckAtStart; // Start pos is set in Editor

    private void Start()
    {
        if (!FireTruck || !Human || !House || !Window || EndOfSimulationOnX == 0)
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

            if (FireTruck.transform.position.x >= EndOfSimulationOnX)
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

    private float GetDistance(Transform obj1, Transform obj2)
    {
        return (obj1.position - obj2.position).magnitude;
    }

    public void Reset()
    {
        FireTruck.transform.position = FireTruckAtStart;
    }
}