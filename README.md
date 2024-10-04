# 1. Description
This repository contains a base class and interface for the Command pattern. It also includes classes for sequential and parallel execution of commands. The Command pattern simplifies request management by encapsulating them as objects. Using a command queue allows you to build a sequence of requests, helping to avoid callback hell. This is particularly useful for creating complex sequences of actions, such as effects and animations, ensuring cleaner and more manageable code.

# 2. How to use
### 2.1 Creating a Command
To create a new command, you need to inherit from the Command class.
- The OnExecute method initiates the command’s working logic.
- The OnStop method handles the cancellation of the command and releases resources.
- The Continue method is called after the working logic is completed, which finalizes the command and changes its state to finished.
Here is an example of a command that waits for a button click:
``` csharp
 public sealed class WaitButtonClick : Command
 {
     private readonly Button _button;

     public WaitButtonClick(Button button)
     {
         _button = button;
     }

     protected override void OnExecute()
     {
         // Start listening for button clicks when the command is executed.
         _button.onClick.AddListener(OnButtonClick);
     }
     
     protected override void OnStop()
     {
         // Unsubscribe from the event when the command is stopped.
         _button.onClick.RemoveListener(OnButtonClick);
     }

     private void OnButtonClick()
     {
         // Unsubscribe from the button click event to prevent further processing.
         _button.onClick.RemoveListener(OnButtonClick);
         // Finish the command.
         Continue();
     }
 }
```
### 2.2 Using the Command
``` csharp
// Create the command and subscribe to events
var cmd = new WaitButtonClick(_button)
  .AddOnStarted(() => Debug.Log("Command started"))
  .AddOnStopped(() => Debug.Log("Command stopped"))
  .AddOnFinished(() => Debug.Log("Command finished"));
// Execute the command
cmd.Execute();	    
```
### 2.3 Command Queue
The command queue executes commands sequentially, waiting for each to complete before starting the next one.
- ExecuteOnAdd allows commands to be dynamically added to the queue and executed as they are added.
- The CommandAddMode parameter specifies whether a command should be added to the beginning (Next) or the end (Last) of the queue. By default, commands are added to the end of the queue.
``` csharp
var queue = new CommandQueue()
  .SetExecuteOnAdd(false)
  .Add(cmd, CommandAddMode.Next)
  .Add(WaitCommand.FromSeconds(1))
  // add other commands
  // ...
  .AddOnStarted(() => Debug.Log("Command queue started"))
  .AddOnFinished(() => Debug.Log("Command queue finished"));
queue.Execute();
```

### 2.4 CommandGroup

A command group executes all specified commands simultaneously and waits for their completion.
``` csharp
var group = new CommandGroup()
  .Add(cmd)
  .Add(WaitCommand.FromSeconds(1))
  // add other commands
  // ...
  .AddOnFinished(() => Debug.Log("All commands completed"));
group.Execute();
```
         
### 2.5 Комбинирование
Both the command queue and command group implement the ICommand interface, allowing them to be used as commands within other groups and queues.
``` csharp
var queue= new CommandQueue()
  .Add(new CommandGroup()
  .Add(new CommandQueue())
  .Add(new CommandQueue()))
  .Add(new CommandGroup());
queue.Execute();
```



# 1. Описание 
Этот репозиторий содержит базовый класс и интерфейс для паттерна “Команда”. В нем также представлены классы для последовательного и одновременного выполнения команд. Паттерн “Команда” упрощает управление запросами, инкапсулируя их в виде объектов. Использование очереди команд позволяет набирать последовательность запросов, помогая избежать “callback hell”. Это особенно полезно для создания сложных последовательностей действий, таких как эффекты и анимации, обеспечивая более чистый и управляемый код.

# 2. Использование
### 2.1 Создание команды
Для новой команды нужно наследоваться от класса Command.
- Метод **OnExecute** запускает рабочую логику команды.
- Метод **OnStop** обрабатывает отмену выполнения команды и освобождает ресурсы.
- Метод **Continue** вызывается после завершения рабочей логики, что приводит к завершению выполнения команды и изменению её состояния на завершённое.
Пример команды, ожидающий нажатия на кнопку
``` csharp
 public sealed class WaitButtonClick : Command
 {
     private readonly Button _button;

     public WaitButtonClick(Button button)
     {
         _button = button;
     }

     protected override void OnExecute()
     {
         // При запуске команды начинаем отслеживать нажатие на кнопку.
         _button.onClick.AddListener(OnButtonClick);
     }
     
     protected override void OnStop()
     {
         // При отмене запущенной команды, необходимо отписаться от события            
         _button.onClick.RemoveListener(OnButtonClick);
     }

     private void OnButtonClick()
     {
         // Отписываемся от события нажатия на кнопку,
         // чтобы предотвратить дальнейшую обработку нажатий.
         _button.onClick.RemoveListener(OnButtonClick);
         // Завершаем выполнение команды.
         Continue();
     }
 }
```
### 2.2 Использование команды
``` csharp
// создание команды и подписка на события
var cmd = new WaitButtonClick(_button)
  .AddOnStarted(() => Debug.Log("Команда запущена"))
  .AddOnStopped(() => Debug.Log("Команда прервана"))
  .AddOnFinished(() => Debug.Log("Команда завершена"));
// запуск выполнения команды
cmd.Execute();	    
```

### 2.3 Очередь команд
Очередь команда запускает команды последовательно друг за другом, дожидаясь завершения предыдущей. 
- **ExecuteOnAdd** позволяет добавлять команды в очередь динамически и выполнять их по мере добавления.
- Параметр **CommandAddMode** определяет,будет ли команда добавляться в начало (Next) или конец (Last) очереди. По умолчанию команда всегда добавляется к конец очереди
``` csharp
var queue = new CommandQueue()
  .SetExecuteOnAdd(false)
  .Add(cmd, CommandAddMode.Next)
  .Add(WaitCommand.FromSeconds(1))
  // добавление других команд
  // ...
  .AddOnStarted(() => Debug.Log("Очередь команд запущена"))
  .AddOnFinished(() => Debug.Log("Очередь команд завершена"));
queue.Execute();
```

### 2.4 Группа команд

Группа команда одновременно запускает все указанные команды и ожидает их завершения.
``` csharp
var group = new CommandGroup()
  .Add(cmd)
  .Add(WaitCommand.FromSeconds(1))
  // добавление других команд
  // ...
  .AddOnFinished(() => Debug.Log("Все команды завершены"));
group.Execute();
```
         
### 2.5 Комбинирование
Очередь команда и группа команд реализуют интерфейс ICommand, что позволяет их самих использовать как команды в группах и очередях
``` csharp
var queue= new CommandQueue()
  .Add(new CommandGroup()
  .Add(new CommandQueue())
  .Add(new CommandQueue()))
  .Add(new CommandGroup());
queue.Execute();
```
