# Kahoot Bot

A powerful and customizable Kahoot bot built with C# and .NET 8, designed to automate interaction with Kahoot quizzes. This project demonstrates efficient use of C# asynchronous programming and web protocols to interact with Kahoot's API.

---

## Features

- **Multi-bot support**: Simulate multiple players joining a Kahoot game.
- **Customizable bot names**: Set unique names or generate random ones.
- **Answer automation**: Automatically select answers based on predefined strategies.
- **Real-time interactions**: Respond to quiz updates in real-time.
- **Simple and extensible**: Easily add new features or customize behavior.

---

## Disclaimer

**Limitation on Bot Count**: The number of bots you can run simultaneously is limited. To handle a large number of bots, you will need to configure proxies. Proxy setup is not included in this project, so you will need to add it yourself if required.

**Bug in Auto Answer Feature**: The automatic answer selection feature may not work correctly because there html class element of kahoot..

---

## Prerequisites

Before running the project, ensure you have the following installed:

- [Visual Studio 2022](https://visualstudio.microsoft.com/)
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Git](https://git-scm.com/)

---

## Getting Started

Follow these steps to set up and run the Kahoot Bot locally.

### 1. Clone the Repository
```bash
git clone <repository-url>
cd kahoot-bot
```

### 2. Install Dependencies
Restore required NuGet packages:
```bash
dotnet restore
```

### 3. Build the Project
Compile the project:
```bash
dotnet build
```

### 4. Run the Project
Launch the bot:
```bash
dotnet run
```

---

## License

This project is licensed under the [MIT License](LICENSE).

---

## Acknowledgments

- Inspired by the Kahoot API and the need for automated solutions.
- Special thanks to the .NET community for their valuable libraries and tools.

---




