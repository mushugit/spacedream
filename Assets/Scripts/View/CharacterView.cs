using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Interfaces;
using Assets.Scripts.Utilities.Unity.Interface;
using Assets.Scripts.View.Interfaces;
using System;
using System.Collections;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Assets.Scripts.View
{
    public class CharacterView : MonoBehaviour, ICharacterView
    {
        private IGameController _game;
        private IUpdatableCharacterController _character;

        [SerializeField]
        private Sprite characterSprite = default;

        [SerializeField]
        private Transform characterParentTransform;

        [SerializeField]
        private string sortingLayerName = default;

        private bool renderCharacter;
        private GameObject characterGO;
        private float _totalLerpDuration;
        private Vector2 _startPosition;
        private Vector2? _destination;
        private float _elapsedLerpDuration;
        private Action _onCompleteCallback;
        private readonly Vector2 _tileCenter = new Vector2(.5f, .5f);

        void Awake()
        {
            _game = GetComponentInParent<GameController>() as IGameController;
            _character = GetComponentInParent<GameCharacterController>() as IUpdatableCharacterController;
            renderCharacter = false;
        }

        void Start()
        {
            WaitForGame(_game);
        }

        public void WaitForGame(IGameController game)
        {
            if (game == null)
            {
                Debug.LogError($"Game not initialized");
                Application.Quit();
            }
            else
            {
                StartCoroutine(WaitForGamePlaying());
            }
        }

        //TODO create helper or move to interface
        public IEnumerator WaitForGamePlaying()
        {
            yield return new WaitUntil(() => _game.Playing);
            yield return new WaitUntil(() => _character.Ready);

            InitCharacter();
            renderCharacter = true;
        }

        private void InitCharacter()
        {
            var vectorPosition = new Vector2(_character.WorldPosition.X, _character.WorldPosition.Y) + _tileCenter;
            characterGO = Instantiate(characterSprite, _character.WorldPosition, vectorPosition, Color.white);

            _character.RegisterView(this);
        }

        private void Update()
        {
            if (renderCharacter)
            {
                //Moving
                if (!_destination.HasValue)
                {
                    return;
                }

                if (_elapsedLerpDuration >= _totalLerpDuration && _totalLerpDuration > 0)
                {
                    return;
                }

                _elapsedLerpDuration += Time.deltaTime;
                var percentAdvancement = _elapsedLerpDuration / _totalLerpDuration;
                //Debug.Log($"Move character {percentAdvancement} (={_elapsedLerpDuration} / {_totalLerpDuration})");

                characterGO.transform.position = Vector2.Lerp(_startPosition, _destination.Value, percentAdvancement);
                _character.UpdatePosition(characterGO.transform.position);

                if (_elapsedLerpDuration >= _totalLerpDuration)
                {
                    //Callback
                    _onCompleteCallback?.Invoke();
                }
            }
        }

        private GameObject Instantiate(Sprite sprite, Point point, Vector2 position, Color color)
        {
            var gameObject = new GameObject($"Character_{point.X}_{point.Y}");

            //Position
            gameObject.transform.position = position;

            //Sprite
            var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingLayerName = sortingLayerName;
            if (color != Color.white)
            {
                spriteRenderer.color = color;
            }

            return gameObject;
        }

        public void Move(Vector2 destination, Action onCompleteCallback = null)
        {
            var adjutedDestination = destination + _tileCenter;

            var distance = Vector2.Distance(characterGO.transform.position, adjutedDestination);

            _totalLerpDuration = distance / _character.SpeedMetersPerSecond;
            _startPosition = characterGO.transform.position;
            _destination = adjutedDestination;
            _elapsedLerpDuration = 0f;

            _onCompleteCallback = onCompleteCallback;
        }
    }
}
