using UnityEditor;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class PhysicSimulation : MonoBehaviour
{
    private UIManager UIref;

    [Header("Assets")]
    [SerializeField] private Transform FireTruck = null;

    [SerializeField] private Transform Human = null;

    [SerializeField] private Transform House = null;

    [SerializeField] private Transform Window = null;

    [Header("Variables")]
    [SerializeField] private int EndOfSimulationOnX;

    [SerializeField] float airImpedance = 440f;
    [SerializeField] float soundSpeed = 343f;

    private Vector3 FireTruckAtStart; // Start pos is set in Editor

    float distance1;
    float distance2;
    float dopplerDifference;

    //Precalculated 
    float transmissionCoef;
    float invertedWaveLength;
    float sinusoidalXCoef;

    private const float TAU = 2 * Mathf.PI;

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
            float deltaTime = Time.deltaTime;
            Movements(deltaTime);

            distance1 = GetDistance(Human, FireTruck);
            distance2 = GetDistance(House, FireTruck);

            LevelHuman1(distance1);
            LevelHuman2(distance2);
            FrequencyHuman(Human.position.x, dopplerDifference);
            FrequencyHuman(House.position.x, dopplerDifference);


            if (FireTruck.transform.position.x >= EndOfSimulationOnX)
            {
                UIref.Reset();
                Reset();
            }
        }
    }

    private float LevelHuman1(float distance)
    {
        return IntensityToDbLevel(ReceveivedIntensity(distance));
    }
    private float LevelHuman2(float distance)
    {
        return IntensityToDbLevel(transmissionCoef *  ReceveivedIntensity(distance));
    }
    private float FrequencyHuman(float humanPositionX, float dopplerDifference)
    {
        if (humanPositionX < FireTruck.position.x)
            return UIref.Frequence + dopplerDifference;
        else if (humanPositionX > FireTruck.position.x)
            return UIref.Frequence + dopplerDifference;
        //RARE CASE ==
        else
            return UIref.Frequence;
    }
    private float ReceveivedIntensity(float distance)
    {
        return UIref.Power / (4 * TAU * distance * distance);
    }
    private float IntensityToDbLevel(float intensity)
    {
        //base intensity is 10e-12 so we invert it
        float iBaseIntensity = (float)10e12;
        return 10f * Mathf.Log(intensity * iBaseIntensity);
    }
    public void PreCalculus()
    {
        dopplerDifference = DopplerDifference();
        transmissionCoef = 1f - Mathf.Pow((airImpedance - UIref.Impedence / (airImpedance + UIref.Impedence)), 2f);
        invertedWaveLength = UIref.Frequence / soundSpeed;
        sinusoidalXCoef = TAU * invertedWaveLength;
    }

    private float DopplerDifference()
    {
        //We won't neglect the difference speed between the sound and the firetruck
        return UIref.Frequence * UIref.TruckSpeed - (soundSpeed - UIref.TruckSpeed);
    }
    private void Movements(float deltaTime)
    {
        FireTruck.position += new Vector3(deltaTime * UIref.TruckSpeed, 0f, 0f);
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