# WebGL Bridge Documentation

This document describes the communication bridge between Unity and JavaScript (WebGL).

## Unity -> JavaScript

Events sent from Unity to JavaScript.

| Event Type | Payload                                             | Description                               |
|------------|-----------------------------------------------------|-------------------------------------------|
| `SET_STEP` | `string` (number)                                   | Updates the step counter in the UI.       |
| `SET_TIME` | `json` (`{ "min": int, "sec": int }`)               | Updates the timer in the UI.              |
| `WIN_GAME` | `""`                                                | Sent when the cube is solved.             |
| `SAVE_DATA`| `json` (serialized `CubeSaveData`)                  | Sends the game state to JS to be saved.   |

## JavaScript -> Unity

Events sent from JavaScript to Unity.

| Event Type  | Payload                                             | Description                               |
|-------------|-----------------------------------------------------|-------------------------------------------|
| `SET_SPEED` | `string` (float)                                    | Sets the rotation speed of the cube.      |
| `SHUFFLE`   | `""`                                                | Shuffles the cube.                        |
| `RESET`     | `""`                                                | Resets the cube to its initial state.     |
| `LOAD_DATA` | `json` (serialized `CubeSaveData`)                  | Loads the game state from JS.             |
