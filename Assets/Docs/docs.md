# MegaPong

## Game mechanics

### Entities

- Ingame entities:
  - Board
  - Ball
  - Platform
  - Player
  - Powerups


- Ingame UI
  - Testing board UI
  - Score UI
  - Countdown UI
  - Controls


- Menu panels: 
  - Main menu
  - Lobby menu
  - Settings menu
  - Pause menu


### Events system

Events are split up to several channels.
Each channel implemented as a scriptable object with event actions and covers events related to certain part of the game.

There is a singleton EventsManager on a scene which has references to all channels as a static properties.
Any script on a scene can subscribe, or invoke any event without a single dependency.
However, scripts subscribed to an event, must check for EventsManager.HasInstance before unsubscribing from events.
UI also can invoke channel events by assigning through the inspector raise methods, defined in channels.

### Gameflow

Game consists of a single scene.

**Main menu**. Starting screen is a main menu panel that is controlled by **MenuController** script.
Available actions:
- Play. Menu panel is replaced by **Lobby panel**.
- Settings. Menu panel is replaced by **Settings panel**.
- Exit.

**Lobby panel**. Available actions:
- Test mechanics. Lobby panel is replaced by **Testing board UI**.
- Play with bot. Lobby panel is hidden, round with bot is loaded and **Ingame UI** appears.
- Host a match. TODO
- Find a match. TODO

**Settings panel**. Available settings:
- Language
- Controls type (levers, sliders, or gyroscope)

**Testing board UI**. Allows to train the game without opponent.
Player can apply any powerup using UI and change controls type.

**Ingame UI**. Consists of:
- Start round button
- Ready button (depending on mode)
- Pause button
- Controls
- Score
- Countdown
- Round winner notification
- Waiting for opponent notification (depending on mode)

### Controllers
- Controller for each panel from Gameflow block. They are subscribed to all interactable UI on a panels.
- Round controller, which handles round lifecycle, like starting or finishing round.
- Game controller, which handles ingame events, like ball touching walls.
- Countdown controller.