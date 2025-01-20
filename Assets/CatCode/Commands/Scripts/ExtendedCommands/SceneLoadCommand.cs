using UnityEngine;
using UnityEngine.SceneManagement;

namespace CatCode.Commands
{

    public sealed class SceneLoadCommand : Command
    {
        private AsyncOperation _operation;

        private readonly string _sceneName;
        private readonly LoadSceneMode _mode;

        public SceneLoadCommand(string sceneName, LoadSceneMode mode)
        {
            _sceneName = sceneName;
            _mode = mode;
        }

        protected override void OnExecute()
        {
            _operation = SceneManager.LoadSceneAsync(_sceneName, _mode);
            _operation.completed += OnAsyncOperationCompleted;
        }

        private void OnAsyncOperationCompleted(AsyncOperation operation)
        {
            _operation.completed -= OnAsyncOperationCompleted;
            Continue();
        }

        protected override void OnStop()
        {
            _operation.completed -= OnAsyncOperationCompleted;
        }
    }
}