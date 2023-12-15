using UnityEditor;
using UnityEngine;

public class PhysicSimulation : MonoBehaviour
{
    private UIManager UIref;

    [Header("Assets")]
    [SerializeField] private Transform FireTruck = null;

    [SerializeField] private Transform HumanOut = null;

    [SerializeField] private Transform HumanIn = null;

    [SerializeField] private Transform Window = null;

    [Header("Variables")]
    [SerializeField] private int EndOfSimulationOnX;

    [SerializeField] private float AirImpedance = 440f;
    [SerializeField] private float SoundSpeed = 343f;

    private Vector3 FireTruckAtStart; // Start pos is set in Editor

    private float DistanceOutside;
    private float DistanceInside;
    private float DopplerDifference;

    //Precalculated
    private float TransmissionCoef;

    private float InvertedWaveLength;
    private float SinusoidalXCoef;

    private const float TAU = 2 * Mathf.PI;

    private void Start()
    {
        if (!FireTruck || !HumanOut || !HumanIn || !Window || EndOfSimulationOnX == 0)
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

            //DistanceOutside = GetDistance(HumanOut, FireTruck);
            //DistanceInside = GetDistance(HumanIn, FireTruck);

            UIref.SetIntensityText(LevelHumanOutside(DistanceOutside), true);
            UIref.SetIntensityText(LevelHumanInside(DistanceInside), false);

            UIref.SetFrequencyText(FrequencyHuman(HumanOut.position.x, DopplerDifference), true);
            UIref.SetFrequencyText(FrequencyHuman(HumanIn.position.x, DopplerDifference), false);

            if (FireTruck.transform.position.x >= EndOfSimulationOnX)
            {
                UIref.Reset();
                Reset();
            }
        }
    }

    private float LevelHumanOutside(float distance)
    {
        return IntensityToDbLevel(ReceveivedIntensity(distance));
    }

    private float LevelHumanInside(float distance)
    {
        return IntensityToDbLevel(TransmissionCoef * ReceveivedIntensity(distance));
    }

    private float FrequencyHuman(float humanPositionX, float DopplerDifference)
    {
        if (humanPositionX < FireTruck.position.x)
            return UIref.Frequency + DopplerDifference;
        else if (humanPositionX > FireTruck.position.x)
            return UIref.Frequency - DopplerDifference;
        //RARE CASE ==
        else
            return UIref.Frequency;
    }

    private double ReceveivedIntensity(float distance)
    {
        return UIref.Power / (4f * Mathf.PI * distance * distance);
    }

    private float IntensityToDbLevel(double intensity)
    {
        //base intensity is 10e-12 so we invert it
        double iBaseIntensity = 1e12;
        float result = 10f * Mathf.Log10((float)(intensity * iBaseIntensity));
        return result;
    }

    public void PreCalculus()
    {
        DistanceOutside = DistanceInside = 100f;
        DopplerDifference = ComputeDopplerDifference();
        float sqrtReflexionCoef = (AirImpedance - UIref.Impedance) / (AirImpedance + UIref.Impedance);
        TransmissionCoef = 1f - (sqrtReflexionCoef * sqrtReflexionCoef); //Squared
        InvertedWaveLength = UIref.Frequency / SoundSpeed;
        SinusoidalXCoef = TAU * InvertedWaveLength;
    }

    private float ComputeDopplerDifference()
    {
        //We won't neglect the difference speed between the sound and the firetruck
        return UIref.Frequency * UIref.TruckSpeed / (SoundSpeed - UIref.TruckSpeed);
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