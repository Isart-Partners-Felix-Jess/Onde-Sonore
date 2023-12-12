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
                Debug.LogError("UIManager is Null");
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    [Header("Frequence")]
    [SerializeField] private TextMeshProUGUI FrequenceText = null;

    [Header("Truck Speed")]
    [SerializeField] private TextMeshProUGUI TruckSpeedText = null;

    [Header("Impedence")]
    [SerializeField] private TextMeshProUGUI ImpedenceText = null;

    [Header("Power")]
    [SerializeField] private TextMeshProUGUI PowerText = null;

    [Header("Start")]
    [SerializeField] private Button GoButton;

    private void Start()
    {
        if (!FrequenceText || !TruckSpeedText || !ImpedenceText || !PowerText || !GoButton)
        {
            ErrorDetected("One or multiple field unset in UIManager");
        }

        Button btn = GoButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    private void ErrorDetected(string _error)
    {
        Debug.LogError(_error);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }

    private void TaskOnClick()
    {
        Debug.Log("You have clicked the go button!");
    }

    public double Frequence
    {
        get
        {
            return 0f;
        }
    }

    public double TruckSpeed
    {
        get
        {
            return 0f;
        }
    }

    public double Impedence
    {
        get
        {
            return 0f;
        }
    }

    public double Power
    {
        get
        {
            return 0f;
        }
    }
}