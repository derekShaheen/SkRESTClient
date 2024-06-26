# Unity WickerREST

![Logo](https://github.com/derekShaheen/WickerREST/blob/main/web/resources/h192.png?raw=true)

Stable / Release|Hit Counter
| :------------: | :------------: |
![releaseworkflow](https://github.com/derekShaheen/WickerREST/actions/workflows/release.yml/badge.svg)| [![HitCount](https://hits.dwyl.com/derekShaheen/WickerREST.svg?style=flat)](http://hits.dwyl.com/derekShaheen/WickerREST)|

Unity WickerREST is a powerful framework designed for Unity developers, allowing for the dynamic creation of commands and game variable monitoring through a web interface. This mod leverages the capabilities of .NET to extend Unity games with a RESTful API, making it possible to interact with the game's internals in real-time from a web browser.

## Features

- **Dynamic Command Handlers**: Easily create and manage commands that can be called through HTTP requests.
- **Game Variable Monitoring**: Watch game variables in real-time through a web interface, with support for dynamic updates.
- **Flexible Configuration**: Use MelonPreferences to customize settings such as the listening port and debug level.
- **Developer-Friendly**: Designed with simplicity in mind, allowing quick integration into your projects.

![SampleUserInterface](https://i.imgur.com/SFzaWUX.png)
![SampleUserInterface2](https://i.imgur.com/zMIWMhI.png)

The Example Command button, Reveal Map button, and Game Fully Initialized variable monitor were generated from this code:
```csharp
[CommandHandler("ExampleCommand")]
public static void ExampleCommandHttp(string Input1, string Input2 = "Default value")
{
    WickerNetwork.Instance.LogResponse($"Returning back: {Input1} {Input2}");
}

[CommandHandler("revealMap")]
public static void RevealMapHttp()
{
    Processing.RevealMap(); // WickerNetwork.Instance.LogResponse() can be used to send a response back to the user.
}

[GameVariable("GameFullyInitialized")]
public static string GetGmeFullyInitialized()
{
    return GameManager.gameFullyInitialized.ToString();
}
```

## Getting Started (Users)

To integrate WickerREST into your Unity game, follow these steps:

1. **Installation**: Ensure you have MelonLoader installed for your game. Simply download the WickerREST.dll and drop it in the /Mods/ folder.

2. **Plugin Installation**: On it's own, this doesn't provide anything for you. You'll also need a plugin for WickerREST for your game. Download this plugin and simply place it in the /Mods/ folder.

3. **Accessing the Web Interface**: Start your game, and navigate to `http://localhost:6103/` in your web browser to access the command interface and game variable monitor. **Default is port 6103.**

## Getting Started (Developers)

**![Sample project repository is available at this link ](https://github.com/derekShaheen/WickerREST-Sample-Plugin)**

To integrate WickerREST into your Unity project, follow these steps:

1. **Installation**: Ensure you have MelonLoader installed in your Unity project. Then, add the WickerREST.dll assembly reference to your Visual Studio project.

2. **Create Command Handlers**: Decorate static methods in your code with the `[CommandHandler]` attribute to expose them as commands. For example:

    ```csharp
    [CommandHandler("sayHello")]
    public static void SayHelloHttp()
    {
        WickerNetwork.Instance.LogResponse("Hello, World!");
    }
    ```
    Parameters are automatically handled by the system and presented to the user in the web interface. Default values are passed through. As the web browser sends and receives strings, parameters after the response must all be of string data type.

    ```csharp
    [CommandHandler("sayInput")]
    public static void SayInputHttp(string input = "Hello World!")
    {
        WickerNetwork.Instance.LogResponse(input);
    }
    ```

3. **Monitor Game Variables**: Use the `[GameVariable]` attribute to mark static properties or methods for monitoring. For example:

    ```csharp
    [GameVariable("playerHealth")]
    public static int PlayerHealth => Player.Instance.Health;
    ```

4. **Accessing the Web Interface**: Start your game, and navigate to `http://localhost:<ListeningPort>/` in your web browser to access the command interface and game variable monitor. **Default is port 6103.**

## Configuration

Modify the `WickerREST.cfg` file generated in the `UserData` directory to adjust settings like the listening port:

```ini
[WickerREST]
ListeningPort=6103
DebugLevel=1 # 0: None, 1: Raised, 2: Verbose
```

## Creating Commands

Commands are created by annotating static methods with the CommandHandler attribute, specifying the path. These methods can accept HttpListenerResponse as a parameter for sending back responses:

```csharp

[CommandHandler("increaseScore")]
public static void IncreaseScoreHttp(int amount = 10)
{
    Game.Instance.IncreaseScore(amount);
    WickerNetwork.Instance.LogResponse($"Score increased by {amount}");
}
```

## Monitoring Game Variables

Game variables can be exposed for real-time monitoring through the web interface by using the GameVariable attribute:

```csharp

[GameVariable("currentTime")]
public static string CurrentTime => DateTime.Now.ToString();
```

These variables will be automatically polled and displayed on the web interface, allowing for easy observation of internal game states.

## Extending WickerServer

WickerREST is designed to be easily extendable. Developers can add custom functionality by creating additional command handlers and monitoring more game variables as needed. The framework handles the complexity of setting up a web server and routing commands, allowing you to focus on creating engaging features for your game.