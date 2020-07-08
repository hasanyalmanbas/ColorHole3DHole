using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DestroyCubeEvent : UnityEvent<GameObject>
{
}

public class NextArenaEvent : UnityEvent<Vector3>
{
}

public class FinishGameEvent : UnityEvent
{
}

public class GameOverEvent : UnityEvent
{
}

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public DestroyCubeEvent destroyCubeEvent = new DestroyCubeEvent();
    public NextArenaEvent nextArenaEvent = new NextArenaEvent();
    public FinishGameEvent finishGameEvent = new FinishGameEvent();
    public GameOverEvent gameOverEvent = new GameOverEvent();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<ArenaController> arenas;
    private int _currentArena;

    private PlayerControler _playerControler;
    private CameraController _cameraController;

    public Vector3 GetCurrentArenaCenterPosition()
    {
        return arenas[_currentArena].transform.position;
    }

    private void Start()
    {
        arenas = FindObjectsOfType<ArenaController>().ToList().OrderBy(arena => arena.numberOfArena).ToList();


        _currentArena = 0;

        destroyCubeEvent.AddListener(CubeDestroy);

        if (GetComponentInChildren<PlayerControler>() is null)
        {
            Debug.LogError("PlayerController not found!");
        }
        else
        {
            _playerControler = GetComponentInChildren<PlayerControler>();
        }

        if (GetComponentInChildren<CameraController>() is null)
        {
            Debug.LogError("CameraController not found!");
        }
        else
        {
            _cameraController = GetComponentInChildren<CameraController>();
        }
    }

    private void CubeDestroy(GameObject cube)
    {
        arenas[_currentArena].cubes.Remove(cube);
        Destroy(cube);

        CheckCubeCount();
    }

    private void CheckCubeCount()
    {
        if (arenas[_currentArena].cubes.Count <= 0)
        {
            if (NextArena())
            {
                nextArenaEvent.Invoke(arenas[_currentArena].transform.position);
            }
            else
            {
                FinishGame();
                finishGameEvent.Invoke();
            }
        }
    }

    private bool NextArena()
    {
        _currentArena++;

        return arenas.Count > _currentArena;
    }

    private void FinishGame()
    {
        _playerControler.enabled = false;
        _cameraController.enabled = false;
        Debug.Log("Finish Game!");
    }

    public void OpenNextLevel()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneBuildIndex = activeSceneIndex + 1;

        if (SceneManager.sceneCountInBuildSettings < nextSceneBuildIndex)
        {
            SceneManager.LoadScene(nextSceneBuildIndex);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        _playerControler.enabled = false;
        _cameraController.enabled = false;

        gameOverEvent.Invoke();
    }
}