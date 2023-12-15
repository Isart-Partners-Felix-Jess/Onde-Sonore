using UnityEditor;
using UnityEngine;

public class PhysicSimulation : MonoBehaviour
{
    private UIManager UIref;
    private SineVisualisation SinVRef;

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
    public float SinusoidalXCoef { get; private set; }
    public float SinusoidalTimeCoef { get; private set; }
    public float SinusoidalAmplitude { get; private set; }

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
        SinVRef = FindObjectOfType<SineVisualisation>();
        FireTruckAtStart = FireTruck.transform.position;
    }

    private void Update()
    {
        if (UIref.IsStarted && !UIref.IsPaused)
        {
            float deltaTime = Time.deltaTime;
            Movements(deltaTime);

            DistanceOutside = GetDistance(HumanOut, FireTruck);
            DistanceInside = GetDistance(HumanIn, FireTruck);

            float IntensityInside = LevelHumanInside(DistanceInside);
            float IntensityOutside = LevelHumanInside(DistanceOutside);
            //Pressure gives the Amplitude
            //Here we Update it in real time
            SinusoidalAmplitude = Mathf.Sqrt(IntensityInside * AirImpedance);
            //Amplitude and frequency downscaled for visualisation
            SinVRef.Draw(SinusoidalAmplitude * 0.01f, SinusoidalXCoef/* for precision here should be * 0.01f*/, SinusoidalTimeCoef * 0.01f);

            UIref.SetIntensityText(IntensityOutside, true);
            UIref.SetIntensityText(IntensityInside, false);
            UIref.SetFrequencyText(FrequencyHuman(HumanOut.position.x, DopplerDifference), true);
            UIref.SetFrequencyText(FrequencyHuman(HumanIn.position.x, DopplerDifference), false);

            UIref.SetWaveRev(PhaseState());

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
        const float TOLERANCE = 0.5f;
        if (humanPositionX < FireTruck.position.x - TOLERANCE)
        {
            //Plus aigu
            return UIref.Frequency + DopplerDifference;
        }
        else if (humanPositionX > FireTruck.position.x + TOLERANCE)
        {
            //Plus grave
            return UIref.Frequency - DopplerDifference;
        }
        //RARE CASE
        else
        {
            //Comme la source
            return UIref.Frequency;
        }
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
        DopplerDifference = ComputeDopplerDifference();
        float sqrtReflexionCoef = (AirImpedance - UIref.Impedance) / (AirImpedance + UIref.Impedance);
        TransmissionCoef = 1f - (sqrtReflexionCoef * sqrtReflexionCoef); //Squared
        InvertedWaveLength = UIref.Frequency / SoundSpeed;
        SinusoidalXCoef = TAU * InvertedWaveLength;
        SinusoidalTimeCoef = TAU * UIref.Frequency;
    }

    private UIManager.WaveReverbState PhaseState()
    {
        const float TOLERANCE = 0.1f; //10% to be easy to see
        float totaDistance = GetDistance(FireTruck, Window) + GetDistance(FireTruck, Window);
        float ratioDistanceWavelength = totaDistance * InvertedWaveLength;
        float floatingPoint = ratioDistanceWavelength - Mathf.FloorToInt(ratioDistanceWavelength);

        if (Mathf.Abs(floatingPoint) < TOLERANCE)
        {
            return UIManager.WaveReverbState.Phase;
        }
        if (Mathf.Abs(floatingPoint - 0.5f) < TOLERANCE)
        {
            return UIManager.WaveReverbState.Opposition;
        }
        else
        {
            return UIManager.WaveReverbState.None;
        }
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

        DistanceOutside = DistanceInside = DopplerDifference =
        TransmissionCoef = InvertedWaveLength = SinusoidalXCoef =
        SinusoidalTimeCoef = SinusoidalAmplitude = 0f;
    }
}