using UnityEngine;
using UnityEngine.Events;

namespace EKTemplate
{
    public class LevelManager : MonoBehaviour
    {
        [HideInInspector] public UnityEvent startEvent = new UnityEvent();
        [HideInInspector] public EndGameEvent endGameEvent = new EndGameEvent();

        #region Singleton
        public static LevelManager instance = null;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        #endregion

        private void Start() => ConstructLevel();

        private void ConstructLevel()
        {
            int _level = GameManager.instance.level;
            while (_level > GameManager.instance.levelCount)
                _level = _level - GameManager.instance.levelCount + (GameManager.instance.levelLoopFrom - 1);

            // Save your levels as prefab into "Resources/levels" folder. (i.e. level-1, level-2, etc..)
            // And spawn your level into the scene
            Instantiate(Resources.Load<GameObject>("levels/level-" + _level));
        }

        // this function calling when pressing "Tap To Continue" on main panel
        public void StartGame()
        {
            startEvent.Invoke();
        }

        // call this function when user pass level as success
        public void Success()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) GameManager.instance.LevelUp();
            endGameEvent.Invoke(true);
        }

        // call this function when user failed
        public void Fail()
        {
            endGameEvent.Invoke(false);
        }
    }

    public class EndGameEvent : UnityEvent<bool> { }
}