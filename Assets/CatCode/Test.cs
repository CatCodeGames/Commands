using CatCode.Commands;
using UnityEngine;
using UnityEngine.UI;

public sealed class Test : MonoBehaviour
{
    [SerializeField] private Button _button;
    void Start()
    {
        var q = new CommandQueue()
            .Add(new WaitButtonClickCommand(_button))
            .Add(new DelayCommand(1f)
                .AddOnFinished(() => Debug.Log(Time.realtimeSinceStartup)))
            .Add(new DelayCommand(1f)
                .AddOnFinished(() => Debug.Log(Time.realtimeSinceStartup)));
        q.Execute();
    }

    void Update()
    {

    }
}
