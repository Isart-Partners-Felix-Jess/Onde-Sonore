using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Singleton management

    private static UIManager _instance = null;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("UIManager is Null");
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#endif
                Application.Quit();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private PhysicSimulation Physics;

    [Header("InputFields")]
    [SerializeField] private TMP_InputField FrequencyInputField = null;

    [SerializeField] private TMP_InputField TruckSpeedInputField = null;

    [SerializeField] private TMP_InputField ImpedanceInputField = null;

    [SerializeField] private TMP_InputField PowerInputField = null;

    [Header("Buttons")]
    [SerializeField] private Button StartButton = null;

    [SerializeField] private Button PauseButton = null;

    private TMP_Text PauseText = null;

    [SerializeField] private Button ResetButton = null;

    [Header("Outside")]
    [SerializeField] private TMP_Text IntensityOut = null;

    [SerializeField] private TMP_Text FrequencyOut = null;

    [Header("Inside")]
    [SerializeField] private TMP_Text IntensityIn = null;

    [SerializeField] private TMP_Text FrequencyIn = null;

    [SerializeField] private TMP_Text WaveReverb = null;

    private bool bStart = false;
    private bool bPaused = false;

    private void Start()
    {
        if (!FrequencyInputField || !TruckSpeedInputField || !ImpedanceInputField || !PowerInputField ||
            !StartButton || !PauseButton || !ResetButton || !IntensityOut || !FrequencyOut || !IntensityIn || !FrequencyIn)
        {
            Debug.LogError("One or multiple field unset in UIManager");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        Physics = FindFirstObjectByType<PhysicSimulation>();

        StartButton.GetComponent<Button>().onClick.AddListener(OnStartTask);
        PauseButton.GetComponent<Button>().onClick.AddListener(OnPauseTask);
        ResetButton.GetComponent<Button>().onClick.AddListener(OnResetTask);

        PauseButton.gameObject.SetActive(false);
        ResetButton.gameObject.SetActive(false);

        PauseText = PauseButton.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
    }

    private void OnStartTask()
    {
        if (FrequencyInputField.text.Length == 0 || TruckSpeedInputField.text.Length == 0 ||
            ImpedanceInputField.text.Length == 0 || PowerInputField.text.Length == 0)
        {
            Debug.Log("Cannot start with values empty");
            return;
        }

        bStart = true;

        Physics.PreCalculus();

        SwitchButtons();

        FrequencyInputField.interactable = TruckSpeedInputField.interactable =
            ImpedanceInputField.interactable = PowerInputField.interactable = false;
    }

    private void OnPauseTask()
    {
        if (!bPaused)
        {
            bPaused = true;
            PauseText.text = "Unpause";
        }
        else
        {
            bPaused = false;
            PauseText.text = "Pause";
        }
    }

    private void OnResetTask()
    {
        Physics.Reset();
        Reset();
    }

    public void Reset()
    {
        bStart = bPaused = false;
        PauseButton.transform.Find("Text (TMP)").GetComponentInChildren<TMP_Text>().text = "Pause";

        SwitchButtons();

        FrequencyInputField.interactable = TruckSpeedInputField.interactable =
            ImpedanceInputField.interactable = PowerInputField.interactable = true;
    }

    private void SwitchButtons()
    {
        StartButton.gameObject.SetActive(!StartButton.gameObject.activeSelf);
        PauseButton.gameObject.SetActive(!PauseButton.gameObject.activeSelf);
        ResetButton.gameObject.SetActive(!ResetButton.gameObject.activeSelf);
    }

    public void SetIntensityText(float newInte, bool isOutside)
    {
        string newText = "Intensité reçue: " + newInte.ToString() + " dB";

        if (isOutside)
            IntensityOut.text = newText;
        else
            IntensityIn.text = newText;
    }

    public enum FrequencyDopplerState : int
    {
        None = 0,
        Deep = 1,
        High = 2
    }

    public void SetFrequencyText(float newFreq, FrequencyDopplerState dopplerState, bool isOutside)
    {
        string newText = "Fréquence perçue: " + newFreq.ToString() + " Hz";

        newText += dopplerState switch
        {
            FrequencyDopplerState.None => "",
            FrequencyDopplerState.Deep => " (Aiguë)",
            FrequencyDopplerState.High => " (Grave)",
            _ => "Error",
        };

        if (isOutside)
            FrequencyOut.text = newText;
        else
            FrequencyIn.text = newText;
    }

    public enum WaveReverbState : int
    {
        None = 0,
        Phase = 1,
        Opposition = 2
    }

    public void SetWaveRev(WaveReverbState waveRev)
    {
        string newText = "Onde réverbérée\n depuis la maison: ";

        newText += waveRev switch
        {
            WaveReverbState.None => "Déphasée",
            WaveReverbState.Phase => "En Phase",
            WaveReverbState.Opposition => "En Opposition",
            _ => "Error",
        };

        WaveReverb.text = newText;
    }

    public float Frequency
    {
        get
        {
            string input = FrequencyInputField.text.ToString();
            return (float)(input.Length > 0 ? Convert.ToDouble(input) : 0);
        }
    }

    public float TruckSpeed
    {
        get
        {
            string input = TruckSpeedInputField.text.ToString();
            return (float)(input.Length > 0 ? Convert.ToDouble(input) : 0);
        }
    }

    public float Impedance
    {
        get
        {
            string input = ImpedanceInputField.text.ToString();
            return (float)(input.Length > 0 ? Convert.ToDouble(input) : 0);
        }
    }

    public float Power
    {
        get
        {
            string input = PowerInputField.text.ToString();
            return (float)(input.Length > 0 ? Convert.ToDouble(input) : 0);
        }
    }

    public bool IsStarted => bStart;

    public bool IsPaused => bPaused;
}