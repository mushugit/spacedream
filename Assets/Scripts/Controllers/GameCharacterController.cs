using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Interfaces;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.Utilities.Unity.Interface;
using Assets.Scripts.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using static Assets.Scripts.Model.Data.Job;

public class GameCharacterController : MonoBehaviour, IUpdatableCharacterController
{
    [Header("Movement parameters")]
    [SerializeField, Range(0.01f, 1000f)]
    private float speedMetersPerSecond = 25f;
    [SerializeField]
    private float wearAndTearAmountRatio = 100f;

    [Header("Energy parameters")]
    [SerializeField]
    private float energyRestorationSpeed = .3f;
    [SerializeField]
    private float moveEnergyCost = 1f;
    [SerializeField]
    private float jobEnergyCost = 10f;

    [SerializeField]
    private TextMeshProUGUI statusText = default;

    [SerializeField]
    private VerticalHealthBar energyBar = default;
    [SerializeField]
    private VerticalHealthBar conditionBar = default;

    private IGameController _game;
    private ICharacterView _view;

    private IVerticalHealthBar _energyBar;
    private IVerticalHealthBar _conditionBar;

    //TODO:move to Interface 
    public bool Ready { get; private set; }

    #region Character data
    //TODO move data to model
    public Point WorldPosition { get; private set; }
    public int EnergyLevel { get; private set; }
    public int Condition { get; private set; }
    private Dictionary<JobCategory, int> jobPriorityList;
    private bool _moving;
    private JobCategory? _currentJobCategory;
    private IAssignableJob _currentJob;
    private Action _moveEndedCallback;
    private float _energyRestorationTime;
    #endregion


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

        _energyBar = energyBar;
        _conditionBar = conditionBar;
        _energyRestorationTime = 0f;

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
                if (HaveEnergyToDoJob())
                {
                    UpdateStatusText($"Moving to {_currentJobCategory} job");
                    Move(job.Parameter.Coord, DoJob);
                }
                else
                {
                    UpdateStatusText($"Resting");
                    JobFinished();
                    Idling();
                }
                
            }
            else
            {
                Idling();
            }
        }
    }

    private bool HaveEnergyToDoJob()
    {
        var moveCost = _currentJob != null ? WorldPosition.EuclidianDistance(_currentJob.Parameter.Coord) * moveEnergyCost : 1f;
        return _energyBar.Value > (jobEnergyCost + moveCost);
    }

    private void Idling()
    {
        UpdateStatusText($"Idling");
        _energyRestorationTime += Time.deltaTime;
        if (_energyRestorationTime >= energyRestorationSpeed)
        {
            RestoreEnergy(1f);
            _energyRestorationTime = 0f;
        }
    }

    private void DoJob()
    {
        Debug.Log($"Job {jobEnergyCost}");
        if (_currentJobCategory.HasValue && DepleteEnergy(jobEnergyCost)
            && _game.DoJob(_currentJobCategory.Value, _currentJob, JobFinished))
        {
            UpdateStatusText($"{_currentJobCategory}ing");
        }
        else
        {
            //Cancel job
            JobFinished();
        }
    }

    private void JobFinished()
    {
        _currentJob = null;
        _currentJobCategory = null;
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
        var distance = WorldPosition.EuclidianDistance(destination);
        if (DepleteEnergy(distance * moveEnergyCost))
        {
            UpdateStatusText("Moving");
            _moving = true;
            _moveEndedCallback = moveEndedCallback;
            _view.Move(destination.ToVector2(), MoveEnded);
        }
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

    private void UpdateStatusText(string text)
    {
        statusText.text = text;
        statusText.enableAutoSizing = true;
    }

    private float RestoreEnergy(float restoredEnergy)
    {
        var misingValue = _energyBar.MaxValue - _energyBar.Value;
        if (restoredEnergy > misingValue)
        {
            restoredEnergy = misingValue;
        }
        _energyBar.Value += restoredEnergy;
        return restoredEnergy;
    }
    private bool DepleteEnergy(float depletedEnergy)
    {
        Debug.Log($"Depleting {depletedEnergy} energy");
        if (_energyBar.Value >= depletedEnergy)
        {
            if (AlterCondition(depletedEnergy / wearAndTearAmountRatio))
            {
                _energyBar.Value -= depletedEnergy;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool AlterCondition(float conditionLost)
    {
        if (_conditionBar.Value >= conditionLost)
        {
            _conditionBar.Value -= conditionLost;
            return true;
        }
        else
        {
            return false;
        }
    }
}
