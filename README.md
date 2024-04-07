# Claim Game
![Hero image for Claim Game](doc/images/overview/App_Launcher_Large.png)

## About
Claim Game is not an actual mobile game, it's a points-adding application for card games with your friends. <br />
This application will do the math for you by adding the points to each player.
You just need to add the players, start the game and the application and then after each round you pass points to everyone into the app.
> [!NOTE]
> Card games such as Claim, Macao, etc.. <br />
> Any card game where you have to add points to each player.

## Installing and running Claim Game
For the moment, the product is released only for Android `5.0` or higher devices.
We're still working for iOS devices, although everybody asks for launching the app on this platform.

## How to use Claim Game
- On the Main Menu page, you will have by default `6` player entries displayed. Add each friend's name in them.
- After each player's name has been added, press the red **Start Game** button.
- A new page will pop up where all the players are displayed. To add points, just enter a numeric value into the entries.

### How to add more players
- Before starting the game, in the Main Menu page press the **Settings** button. A panel will slide up from the bottom and there you will have 2 options:
  - A slider for number of players.
  - An entry for Max Points.
- To close the settings panel, slide/swipe your finger down on the screen and it will hide right away.

## Tips and Tricks
- Press **Enter** to move through visible players and score entries.
- **Tap** anywhere on the screen when an entry is focused to unfocus and hide the keyboard.
- **Points Entries** that are left blank are considered 0. In this way, you only add the points that add up.

## Features
- **New game**
  - Will start a new game with the new changes.
- **Resume game**
  - Return to current started game without any modifications.
- **Go back to Menu**
  - Go back to make modifications with the posibility to resume or start a new game.
- **Reset score**

## Restrictions
- **Player Names**
  - Only numbers, whitespaces and letters are allowed.
  - `2` to `18` players maximum allowed.
  - Entries must be completed one after another. Canot have empty entries between completed ones.
- **Max Points**
  - Only positive numeric integers are allowed.
  - Blank input is not allowed and it will be automatically set to the last valid Max Points.
  - Numbers starting with 0 will be trimmed. (ex: `0123` or `00123` --> `123`)
- **Points Entries**
  - Only positive and negative numeric integers are allowed.
  - Numbers starting with 0 will be trimmed. (ex: `0123` or `00123` --> `123`)

## Future updates
- Release build for iOS devices.

## Contributing
**Claim Game** project is still in development for a better user experience.
Please feel free to suggest modifications by sending a push request with an afferent description of your changes.
> [!TIP]
> Keep in mind to debug often after any code modifications to assure it's bug-free.
