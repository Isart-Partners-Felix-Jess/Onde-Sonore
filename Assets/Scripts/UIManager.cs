using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Singleton management

    private static UIManager _instance;

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

    [Header("InputFields")]
    [SerializeField] private TMP_InputField FrequenceInputField = null;

    [SerializeField] private TMP_InputField TruckSpeedInputField = null;

    [SerializeField] private TMP_InputField ImpedanceInputField = null;

    [SerializeField] private TMP_InputField PowerInputField = null;

    [Header("Buttons")]
    [SerializeField] private Button StartButton;

    [SerializeField] private Button PauseButton;

    [SerializeField] private Button ResetButton;

    private bool _start = false;
    private bool _paused = false;

    private void Start()
    {
        if (!FrequenceInputField || !TruckSpeedInputField || !ImpedanceInputField || !PowerInputField ||
            !StartButton || !PauseButton || !ResetButton)
        {
            Debug.LogError("One or multiple field unset in UIManager");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        StartButton.GetComponent<Button>().onClick.AddListener(OnStartTask);
        PauseButton.GetComponent<Button>().onClick.AddListener(OnPauseTask);
        ResetButton.GetComponent<Button>().onClick.AddListener(OnResetTask);

        PauseButton.gameObject.SetActive(false);
        ResetButton.gameObject.SetActive(false);
    }

    private void OnStartTask()
    {
        if (FrequenceInputField.text.Length == 0 || TruckSpeedInputField.text.Length == 0 ||
            ImpedanceInputField.text.Length == 0 || PowerInputField.text.Length == 0)
        {
            Debug.Log("Cannot start with values empty");
            return;
        }

        _start = true;

        SwitchButtons();

        FrequenceInputField.interactable = TruckSpeedInputField.interactable =
            ImpedanceInputField.interactable = PowerInputField.interactable = false;
    }

    private void OnPauseTask()
    {
        if (!_paused)
        {
            _paused = true;
            PauseButton.transform.Find("Text (TMP)").GetComponentInChildren<TMP_Text>().text = "Unpause";
        }
        else
        {
            _paused = false;
            PauseButton.transform.Find("Text (TMP)").GetComponentInChildren<TMP_Text>().text = "Pause";
        }
    }

    private void OnResetTask()
    {
        FindFirstObjectByType<PhysicSimulation>().Reset();
        Reset();
    }

    public void Reset()
    {
        _start = _paused = false;
        PauseButton.transform.Find("Text (TMP)").GetComponentInChildren<TMP_Text>().text = "Pause";

        SwitchButtons();

        FrequenceInputField.interactable = TruckSpeedInputField.interactable =
            ImpedanceInputField.interactable = PowerInputField.interactable = true;
    }

    private void SwitchButtons()
    {
        StartButton.gameObject.SetActive(!StartButton.gameObject.activeSelf);
        PauseButton.gameObject.SetActive(!PauseButton.gameObject.activeSelf);
        ResetButton.gameObject.SetActive(!ResetButton.gameObject.activeSelf);
    }

    public float Frequence
    {
        get
        {
            string input = FrequenceInputField.text.ToString();
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

    public bool IsStarted
    {
        get => _start;
    }

    public bool IsPaused
    {
        get => _paused;
    }
}