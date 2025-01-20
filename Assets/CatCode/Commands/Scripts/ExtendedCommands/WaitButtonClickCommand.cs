using UnityEngine.UI;

namespace CatCode.Commands
{
    public sealed class WaitButtonClickCommand : Command
    {
        private readonly Button _button;

        public WaitButtonClickCommand(Button button)
        {
            _button = button;
        }

        protected override void OnExecute()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
            Continue();
        }

        protected override void OnStop()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}