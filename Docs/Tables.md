# Таблицы (Unity/WebGL интеграция)

### id в WebGLBuild/index1.html

| id                  | Роль                                          | Кто пишет/читает                                                                                                                         |
| ------------------- | --------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| TimeM               | Минуты таймера                                |                                                                                                                                          |
| Time:               | Двоеточие                                     |                                                                                                                                          |
| TimeS               | Секунды таймера                               |                                                                                                                                          |
| Step                | Шаги                                          |                                                                                                                                          |
| Speed               | Скорость                                      |                                                                                                                                          |
| solve-button        | Решить кубик (button)                         | solveCube() in js                                                                                                                        |
| stats-button        | Статистика (button)                           | showStats() in js                                                                                                                        |
| save-button         | Сохранить в файл (button)                     | sendCommand('SAVE_GAME') in js                                                                                                           |
| load-button         | Загрузить из файла (button)                   | triggerLoad() in js                                                                                                                      |
| export-stats-button | Экспорт статистики (button)                   | exportStats()                                                                                                                            |
| speed-slider        | Slider speed                                  | updateSpeed(this.value) in js                                                                                                            |
| pastel-button       | Pastel Color                                  | handleButtonClick('Pastel')                                                                                                              |
| standart-button     | Standart Color                                | handleButtonClick('Standart')                                                                                                            |
| neon-button         | Neon Color                                    | handleButtonClick('Neon')                                                                                                                |
| grey-button         | Grey Color                                    | handleButtonClick('Grey')                                                                                                                |
| warning-modal       | Всплывающее окно предупреждения о смене цвета | Вызывается в handleButtonClick()                                                                                                         |
| win-modal           | Всплывающее окно о победе                     | Вызывается в ... Доделать отображение данных. Убрать двойное скачиваение. Реализовать принцип сбрасывания. В целом есть с чем поработать |
| stats-modal         | Высплывающее окно сохранения статичтики       | Вызывается в ...                                                                                                                         |

## Методы в main.js

| Method | What doing | Instruction |
| ------ | ---------- | ----------- |

## 1) Переменные в Unity (C#)

Сводка публичных и приватных полей по скриптам.

| Скрипт / класс   | Переменная                                            | Тип               | Доступ                  | Назначение                          |
| ---------------- | ----------------------------------------------------- | ----------------- | ----------------------- | ----------------------------------- |
| CameraMovement   | localRotation                                         | Vector3           | private                 | Накопленный поворот камеры          |
| CameraMovement   | mouseStartPos / mouseEndPos                           | Vector3           | private                 | Позиции для вычисления свайпа       |
| CameraMovement   | cameraDisabled / rotateDisabled                       | bool              | private                 | Блокировки вращения камеры/куба     |
| CameraMovement   | CubeMan                                               | CubeManager       | public                  | Ссылка на менеджер куба             |
| CameraMovement   | pieces / planes                                       | List<GameObject>  | private                 | Две выбранные части и их плоскости  |
| CubeManager      | CubePrefab                                            | GameObject        | public                  | Префаб части кубика                 |
| CubeManager      | cubeRoot                                              | Transform         | private                 | Родитель для частей                 |
| CubeManager      | allPieces                                             | List<GameObject>  | public                  | Все 27 частей                       |
| CubeManager      | centerPiece                                           | GameObject        | private                 | Центр вращения                      |
| CubeManager      | canRotate / canTimer                                  | bool              | private                 | Флаги вращения и таймера            |
| CubeManager      | rotationSpeed                                         | float             | [SerializeField] public | Скорость анимации; отправка в JS    |
| CubeManager      | step                                                  | int               | [SerializeField] public | Счётчик ходов; отправка в JS        |
| CubeManager      | min / sec                                             | int               | [SerializeField] public | Таймер; отправка в JS               |
| CubeManager      | \_elapsedTime                                         | float             | [SerializeField] public | База для мин/сек                    |
| CubeManager      | \_cubes                                               | List<float[]>     | [SerializeField] public | Буфер координат/кватернионов        |
| CubeManager      | \_style                                               | string            | [SerializeField] public | Текущий стиль материалов            |
| CubeManager      | saveSystem                                            | SaveSystem        | public                  | Ссылка на систему сохранений        |
| CubeManager      | data                                                  | string            | public                  | Строка даты/метки                   |
| CubePiaceScr     | Planes                                                | List<GameObject>  | public                  | 6 плоскостей граней                 |
| CubePiaceScr     | Cube                                                  | GameObject        | public                  | Центральный “корпус”                |
| CubePiaceScr     | Pastel/Neon/Grey/StandartCollors                      | Material[]        | public                  | Наборы материалов                   |
| SaveSystem       | cubeManager                                           | CubeManager       | public                  | Источник данных/применение загрузки |
| SaveSystemBridge | saveSystem                                            | SaveSystem        | public                  | Прокладка под HTML-кнопки           |
| CubeSaveData     | cubelets                                              | List<CubeletData> | public                  | 27 кубиков: позиция/кватернион      |
| CubeSaveData     | step/min/sec/elapsedTime/rotationSpeed/cubeStyle/Data | числа/строка      | public                  | Поля сохранения                     |
| CubeletData      | px/py/pz/rx/ry/rz/rw                                  | float             | public                  | Позиция + кватернион                |

## 2) Методы в Unity (C#)

Основные публичные и используемые методы.

| Скрипт           | Метод                                  | Сигнатура                                          | Назначение / вызов                                                 |
| ---------------- | -------------------------------------- | -------------------------------------------------- | ------------------------------------------------------------------ |
| CameraMovement   | LateUpdate                             | void                                               | Обработка мыши, выбор 2 частей, вызов DetectRotate, поворот камеры |
| CubeManager      | Start                                  | void                                               | Создание куба, отправка Step/Speed в JS                            |
| CubeManager      | Update                                 | void                                               | Клавиши, таймер, проверка победы                                   |
| CubeManager      | GetTimeS/GetTimeM/GetStep/GetEclipsedT | int/float                                          | Геттеры для SaveSystem                                             |
| CubeManager      | GetPozition                            | List<float[]>                                      | Координаты/кватернионы всех частей                                 |
| CubeManager      | RefreshCube                            | void (string, List<float[]>)                       | Пересоздаёт куб из сохранения                                      |
| CubeManager      | CheckKeyboardInput                     | void                                               | W/S/A/D/F/B вращения; R — shuffle; E — reset                       |
| CubeManager      | CreateCube                             | void (string style="Standart")                     | Новое состояние 3×3×3 + раскраска                                  |
| CubeManager      | GetFacePieces                          | List<GameObject> (int)                             | Части грани по индексу                                             |
| CubeManager      | ShuffleCube                            | IEnumerator                                        | Перемешивание случайными поворотами                                |
| CubeManager      | RotateFace                             | IEnumerator (List<GameObject>, Vector3, float=90)  | Плавный поворот грани, инкремент step                              |
| CubeManager      | DetectRotate                           | void (List<GameObject>, List<GameObject>, Vector3) | Определяет грань/ось по двум частям и свайпу                       |
| CubeManager      | Timer/ClearTimer                       | void                                               | Подсчёт/сброс времени                                              |
| CubeManager      | SendNumberInt/SendNumberFloat/SendTime | void                                               | Unity → JS: Step/Speed/Time через DllImport                        |
| CubeManager      | WebGL_CreateCube                       | void (string)                                      | Точка входа из HTML/JS                                             |
| CubeManager      | WebGL_Shuffle                          | void                                               | Точка входа из HTML/JS                                             |
| CubeManager      | WebGL_SetSpeed                         | void (float)                                       | Точка входа из HTML/JS + отправка Speed в JS                       |
| CubeManager      | CheckSolved                            | void                                               | Проверка победы, сохранение, показ модалки                         |
| CubeManager      | ShowWinModal                           | void                                               | Unity → JS: ExternalCall("showWinModal")                           |
| CubePiaceScr     | SetColor                               | void (int,int,int)                                 | Активирует плоскости граней                                        |
| CubePiaceScr     | SetColor2                              | void (string)                                      | Ставит материалы по стилю                                          |
| CubePiaceScr     | GetActiveColors                        | List<Color>                                        | Цвета активных граней                                              |
| SaveSystem       | Start                                  | void                                               | Поиск CubeManager                                                  |
| SaveSystem       | SaveToJSON                             | void                                               | Сохранение (WebGL — download, Editor — файл)                       |
| SaveSystem       | LoadFromJSON                           | void (string json)                                 | Загрузка из строки                                                 |
| SaveSystem       | ApplySaveData                          | void (CubeSaveData)                                | Применение сохранения + обновление UI через Send...                |
| SaveSystemBridge | WebGL_SaveGame                         | void                                               | HTML кнопка “Сохранить”                                            |
| SaveSystemBridge | WebGL_LoadGame                         | void                                               | HTML кнопка “Загрузить” (TriggerFileUpload)                        |
| SaveSystemBridge | OnLoadComplete                         | void (string json)                                 | Callback: JSON получен → LoadFromJSON                              |

## 3) HTML/JS: id и скрипты

### Подключённые скрипты

| HTML                   | Скрипты                                                                    |
| ---------------------- | -------------------------------------------------------------------------- |
| WebGLBuild/index1.html | TemplateData/diagnostics.js, js/main.js, js/ui.js, js/unity.js, js/save.js |
| WebGLBuild/index.html  | js/ui.js, js/unity.js, js/save.js                                          |

## 4) Взаимодействия Unity ↔ HTML/JS

### Unity → JS/DOM

| Откуда                                             | Как                                         | Куда                                                    | Что делает                                 |
| -------------------------------------------------- | ------------------------------------------- | ------------------------------------------------------- | ------------------------------------------ |
| CubeManager.SendTime/SendNumberInt/SendNumberFloat | DllImport("\_\_Internal")                   | WebGLSay.jslib → document.getElementById(...).innerText | Обновление TimeM/TimeS/Step/Speed          |
| CubeManager.ShowWinModal                           | Application.ExternalCall("showWinModal")    | ui.js/main.js: showWinModal()                           | Открыть модалку победы и заполнить данными |
| SaveSystem.SaveToJSON (WebGL)                      | DllImport("\_\_Internal") DownloadFile      | SaveSystem.jslib                                        | Скачать JSON через Blob                    |
| SaveSystemBridge.WebGL_LoadGame                    | DllImport("\_\_Internal") TriggerFileUpload | SaveSystem.jslib                                        | Открыть file picker                        |

### JS → Unity

| Источник                          | Как                                              | Цель (GameObject) | Метод                                                               |
| --------------------------------- | ------------------------------------------------ | ----------------- | ------------------------------------------------------------------- |
| ui.js/main.js/unity.js            | unityInstance.SendMessage(...)                   | 'Cube'            | CreateCube, SolveCube, GetStats, ExportStatsToJSON, WebGL_SetSpeed  |
| save.js/main.js                   | unityInstance.SendMessage(...)                   | 'SaveSystem'      | SaveToJSON, LoadFromJSON(json)                                      |
| SaveSystem.jslib (input onchange) | SendMessage('SaveSystem','OnFileLoaded',content) | 'SaveSystem'      | **В C# метода OnFileLoaded нет** (есть LoadFromJSON/OnLoadComplete) |

## 5) Соответствие GameObject ↔ ожидаемые методы (для SendMessage)

| GameObject (строка) | Методы, которые вызывает JS                                        | Где должны быть в Unity                                                                    |
| ------------------- | ------------------------------------------------------------------ | ------------------------------------------------------------------------------------------ |
| 'Cube'              | CreateCube, SolveCube, GetStats, ExportStatsToJSON, WebGL_SetSpeed | Скрипт на объекте Cube (есть CreateCube/WebGL_SetSpeed в CubeManager; остальные проверить) |
| 'SaveSystem'        | SaveToJSON, LoadFromJSON, (из .jslib: OnFileLoaded)                | Скрипт SaveSystem на объекте SaveSystem; OnFileLoaded сейчас отсутствует                   |

### Примечание

В `Assets/Plugins/SaveSystem.jslib` при выборе файла вызывается `SendMessage('SaveSystem','OnFileLoaded', content)`, но в C# такого метода нет (есть `SaveSystemBridge.OnLoadComplete` и `SaveSystem.LoadFromJSON`). Это просто зафиксировано для сверки; код не менялся.
