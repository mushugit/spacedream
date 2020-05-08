using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Interfaces;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Model.Data.Job;

public class CharacterController : MonoBehaviour, IUpdatableCharacterController
{
    [SerializeField, Range(0.01f, 1000f)]
    private float speedMetersPerSecond = 25f;

    private IGameController _game;
    private ICharacterView _view;

    //TODO:move to Interface 
    public bool Ready { get; private set; }

    //TODO move data to model
    public Point WorldPosition { get; private set; }
    private Dictionary<JobCategory, int> jobPriorityList;
    private bool _moving;
    private JobCategory? _currentJobCategory;
    private IAssignableJob _currentJob;
    private Action _moveEndedCallback;

    public float SpeedMetersPerSecond
    {
        get { return speedMetersPerSecond; }
    }

    void Awake()
    {
        _game = GetComponentInParent<GameController>() as IGameController;

        //TODO:move data to model
        WorldPosition = new Point(0, 0);
        _moving = false;

        InitPriorityList();
    }
    
    //TODO utility class
    private void InitPriorityList()
    {
        jobPriorityList = new Dictionary<JobCategory, int>(Enum.GetValues(typeof(Job.JobCategory)).Length)
        {
            { JobCategory.Build, 0 },
            { JobCategory.Destroy, 1 }
        };
    }

    void Start()
    {
        WorldPosition = _game.WorldCenter;
        Ready = true;
    }

    void Update()
    {
        if (!_moving && _currentJob == null)
        {
            var job = FindJob();
            if (job != null)
            {
                _currentJob = job;
                Move(job.Parameter.Coord, DoJob);
            }
        }
    }

    private void DoJob()
    {
        if(_currentJobCategory.HasValue && _game.DoJob(_currentJobCategory.Value, _currentJob))
        {
            _currentJob = null;
            _currentJobCategory = null;
        }
        
    }

    private IAssignableJob FindJob()
    {
        for (var priority = 0; priority < jobPriorityList.Count; priority++)
        {
            var keyValuePair = jobPriorityList.First(kvp => kvp.Value == priority);
            var job = _game.PeekJob(keyValuePair.Key);
            if (job != null)
            {
                _currentJobCategory = keyValuePair.Key;
                return job;
            }
        }
        _currentJobCategory = null;
        return null;
    }

    public void ForceMove(Point destination)
    {
        Move(destination);
    }

    public void AskMove(Point destination)
    {
        if (!_moving)
        {
            Move(destination);
        }
    }

    private void Move(Point destination, Action moveEndedCallback = null)
    {
        _moving = true;
        _moveEndedCallback = moveEndedCallback;
        _view.Move(destination.ToVector2(), MoveEnded);
    }

    private void MoveEnded()
    {
        _moving = false;
        _moveEndedCallback?.Invoke();
    }

    public void UpdatePosition(Vector2 position)
    {
        WorldPosition = new Point(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    public void RegisterView(ICharacterView view)
    {
        _view = view;
    }
}
