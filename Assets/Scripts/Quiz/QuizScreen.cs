using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class QuizScreen : MonoBehaviour
{
    private const string SavePath = "quizes";
    
    [SerializeField] private List<QuizPlane> _planes;
    [SerializeField] private OpenQuizScreen _openQuizScreen;
    [SerializeField] private MainScreen _mainScreen;

    private List<QuizData> _quizDatas = new List<QuizData>();
    private ScreenVisabilityHandler _screenVisabilityHandler;
    
    public event Action<QuizPlane> QuizOpened;
    public event Action BackClicked;

    public List<QuizPlane> Planes => _planes;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        LoadQuizes();
        Disable();
    }

    private void OnEnable()
    {
        foreach (var plane in _planes)
        {
            plane.Opened += OpenQuiz;
            plane.CompletionChanged += SaveQuizData;
        }
        
        _openQuizScreen.BackClicked += Enable;
        _mainScreen.QuizClicked += Enable;
    }

    private void OnDisable()
    {
        foreach (var plane in _planes)
        {
            plane.Opened -= OpenQuiz;
            plane.CompletionChanged -= SaveQuizData;
        }
        
        _openQuizScreen.BackClicked -= Enable;
        _mainScreen.QuizClicked -= Enable;
    }

    private void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    private void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnBackClicked()
    {
        BackClicked?.Invoke();
        Disable();
    }
    
    private void SaveQuizData(QuizData data)
    {
        foreach (var dta in _quizDatas)
        {
            if (dta.Type == data.Type)
            {
                dta.IsComplete = data.IsComplete;
            }
        }

        QuizDataWrapper scoresList = new QuizDataWrapper(_quizDatas);
        string json = JsonUtility.ToJson(scoresList, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, SavePath), json);
    }

    private void LoadQuizes()
    {
        string path = Path.Combine(Application.persistentDataPath, SavePath);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("Scores loaded: " + json);

            QuizDataWrapper scoresList = JsonUtility.FromJson<QuizDataWrapper>(json);

            foreach (var score in scoresList.Datas)
            {
                _quizDatas.Add(score);
            }
            
            UpdateQuizData();
        }
        else
        {
            foreach (var plane in _planes)
            {
                _quizDatas.Add(plane.Data);
            }
        }
    }

    private void UpdateQuizData()
    {
        for (int i = 0; i < _planes.Count; i++)
        {
            _planes[i].UpdateCompletion(_quizDatas[i].IsComplete);
        }
    }

    private void OpenQuiz(QuizPlane plane)
    {
        QuizOpened?.Invoke(plane);
        Disable();
    }
}

[Serializable]
internal class QuizDataWrapper
{
    public List<QuizData> Datas;

    public QuizDataWrapper(List<QuizData> datas)
    {
        Datas = datas;
    }
}
