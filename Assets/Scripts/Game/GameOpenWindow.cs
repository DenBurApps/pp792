using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class GameOpenWindow : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;

    private ScreenVisabilityHandler _screenVisabilityHandler;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _startGameButton.onClick.AddListener(ProcessGameStart);
    }

    private void OnDisable()
    {
        _startGameButton.onClick.RemoveListener(ProcessGameStart);
    }

    public void EnableWindow()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void DisableWindow()
    {
        _screenVisabilityHandler.DisableScreen();
    }
    
    private void ProcessGameStart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
