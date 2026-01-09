# Sunbase Unity Assessment

A Unity project containing two assessment tasks demonstrating UI development, API integration, and 2D gameplay mechanics.

**Unity Version:** 2022.3.17f1

---

## Table of Contents
- [Task 1 - Client List with API Integration](#task-1---client-list-with-api-integration)
- [Task 2 - Line Drawing Game](#task-2---line-drawing-game)
- [Project Structure](#project-structure)
- [Dependencies](#dependencies)
- [How to Run](#how-to-run)

---

# Task 1 - Client List with API Integration

## Demo


[https://github.com/user-attachments/assets/demo_T2.mp4](https://github.com/user-attachments/assets/9dd2f849-b76c-49e9-bdc4-dbdfac0a62a3)
> 📹 If the video doesn't play inline, [click here to watch demo_T1.mp4](demo_T1.mp4)

**Scene:** `Assets/Project/Scenes/Task1.unity`

## Overview
A Unity UI application that fetches client data from a REST API and displays it in a scrollable list with filtering options. Clicking on a client row opens a popup with detailed information.

## Features
- **API Integration**: Fetches client data from a remote JSON endpoint
- **Scrollable List**: Displays clients in a vertical scroll view
- **Filtering**: Dropdown to filter by All/Managers/Non-Managers
- **Detail Popup**: Shows full client info (name, address, points) on row click
- **Animations**: DOTween-powered entrance animations and popup transitions

## Architecture

```
Scripts/
├── Data/
│   └── ClientModels.cs      # Data models matching the API response
├── Services/
│   └── ClientService.cs     # Handles API calls and JSON parsing
└── UI/
    ├── ClientListManager.cs # Main controller for the list view
    ├── ClientRow.cs         # Individual row item in the list
    └── ClientPopup.cs       # Detail popup window
```

## How It Works

### Data Flow
```
API → ClientService (fetch + parse) → ClientListManager (display) → ClientRow (item) → ClientPopup (details)
```

### API Response Structure
The API returns a nested JSON with two main parts:
- `clients[]` - Array of basic client info (id, label, isManager)
- `data{}` - Dictionary with detailed info keyed by id (name, address, points)

The `ClientService` merges these into a clean `ClientProfile` object.

### Filtering Logic
- **All**: Show all clients
- **Managers**: Filter where `isManager == true`
- **Non-Managers**: Filter where `isManager == false`

### API Endpoint
```
GET https://qa.sunbasedata.com/sunbase/portal/api/assignment.jsp?cmd=client_data
```

---

# Task 2 - Line Drawing Game

## Demo
[https://github.com/user-attachments/assets/demo_T1.mp4](https://github.com/user-attachments/assets/93203d23-d02b-4122-a51c-8f00e56cf3b6)


> 📹 If the video doesn't play inline, [click here to watch demo_T2.mp4](demo_T2.mp4)

**Scene:** `Assets/Project/Scenes/Task2.unity`

## Overview
A 2D game where the player draws lines with the mouse to hit randomly spawned target circles. The line acts as a physical collider that detects collision with targets.

## Features
- **Random Circle Spawning**: 5-10 target circles spawn at random positions on game start
- **Line Drawing**: Click and drag to draw a line on screen
- **Physics-Based Detection**: Line uses EdgeCollider2D to detect hits with targets
- **Animated Feedback**: Circles pop in on spawn and shrink when hit
- **Restart Functionality**: Button to restart and play again

## How It Works

### Game Flow
1. **Start**: Random number of circles (5-10) spawn with pop-in animation
2. **Draw**: Hold left mouse button and drag to draw a line
3. **Release**: On mouse release, the line checks for collisions with targets
4. **Hit Detection**: Any target touching the line shrinks and is destroyed
5. **Restart**: Click the restart button to reset and play again

### Key Components
- **LineRenderer**: Renders the drawn line visually
- **EdgeCollider2D**: Provides physics collision for the line
- **Target Circles**: Tagged as "Target" with Circle Collider 2D

### Architecture
```
Scripts/
└── LineGameManager.cs    # Handles spawning, drawing, collision detection, restart
```

### Input
- **Left Mouse Button (Hold)**: Draw line
- **Left Mouse Button (Release)**: Check for hits
- **Restart Button**: Reload the scene

---

# Project Structure

```
Assets/Project/
├── Prefabs/
│   ├── TargetCircle.prefab       # Task 2 - Target circle prefab
│   └── UI/
│       └── ClientRow_prefab.prefab   # Task 1 - List row item
├── Scenes/
│   ├── Task1.unity               # Client List scene
│   └── Task2.unity               # Line Drawing Game scene
└── Scripts/
    ├── Data/
    │   └── ClientModels.cs
    ├── Services/
    │   └── ClientService.cs
    ├── UI/
    │   ├── ClientListManager.cs
    │   ├── ClientPopup.cs
    │   └── ClientRow.cs
    └── LineGameManager.cs
```

---

# Dependencies

| Package | Purpose |
|---------|---------|
| **DOTween** | Animation library for smooth UI and gameplay transitions |
| **TextMesh Pro** | Better text rendering for UI |
| **Newtonsoft.Json** | JSON parsing for API responses (Task 1) |

---

# How to Run

1. Open the project in **Unity 2022.3.17f1** or compatible version
2. Open the desired scene:
   - **Task 1**: `Assets/Project/Scenes/Task1.unity`
   - **Task 2**: `Assets/Project/Scenes/Task2.unity`
3. Press **Play** in the Unity Editor
