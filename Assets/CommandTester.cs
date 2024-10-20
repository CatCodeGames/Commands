using CatCode.Commands;
using UnityEngine;

public class CommandTester : MonoBehaviour
{
    [SerializeField] private WaitCommand _command;

    private void Awake()
    {
        _command = new WaitCommand(5)
            .AddOnStarted(() => Debug.Log("Started"))
            .AddOnFinished(() => Debug.Log("Finished"))
            .AddOnStopped(() => Debug.Log("Stopped"));
    }
    private void OnEnable()
    {
        _command.Execute();
    }

    private void OnDisable()
    {
        _command.Stop();
    }
}
