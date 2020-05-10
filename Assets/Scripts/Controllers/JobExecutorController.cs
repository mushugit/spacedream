using Assets.Scripts.Controllers.Interfaces;
using Assets.Scripts.Model.Interfaces.Data;
using System;
using System.Collections;
using UnityEngine;
using static Assets.Scripts.Model.Data.Job;

namespace Assets.Scripts.Controllers
{
    class JobExecutorController : MonoBehaviour, IJobExecutorController
    {
        [SerializeField]
        private GameObject BuildParticlesPrefab = default;
        [SerializeField]
        private GameObject DestroyParticlesPrefab = default;

        private IExecutableJob _job;
        private Action _callback;

        private const float _defaultZ = -10f;
        private readonly Vector3 _tileCenter = new Vector3(.5f, .5f, 0f);

        private void Awake()
        {
            var gameController = GetComponentInParent<GameController>() as IGameController;
            gameController.RegisterJobExecutor(this);
        }
        public void ExecuteJob(IExecutableJob job, Action callback = null)
        {
            _job = job;
            _callback = callback;
            StartCoroutine(JobExecutor());
        }

        private IEnumerator JobExecutor()
        {
            var position = new Vector3(_job.Parameter.Coord.X, _job.Parameter.Coord.Y, _defaultZ) + _tileCenter;
            var particles = Instantiate(GetParticlePrefab(_job.Category), position, Quaternion.identity);
            particles.transform.parent = gameObject.transform;
            yield return new WaitForSeconds(_job.ExecuteTime);
            _job.Execute();
            _callback?.Invoke();
        }

        private GameObject GetParticlePrefab(JobCategory jobCategory)
        {
            switch (jobCategory)
            {
                case JobCategory.Build:
                    return BuildParticlesPrefab;
                case JobCategory.Destroy:
                    return DestroyParticlesPrefab;
                default:
                    return null;
            }
        }
    }
}
