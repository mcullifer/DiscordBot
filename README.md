# PogBot
PogBot is a Discord bot project I started mainly as a way to track saves for CK3. It's also open for new features to be added overtime and serves as a practice project.

Due to CK being a long form game it's sometimes difficult to organize a time where all of your friends are available. 
This leads to new saves being created for every subset of players. Eventually, keeping track of which save files were with which people becomes cumbersome. Thus PogBot was created
as a way to keep track of these saves and the players that are involved in them.

Uses [Discord.NET](https://github.com/discord-net/Discord.Net)

## Current Features
* Slash Commands
  * `/create-save` Creates a new CK save object and appends it to a game save JSON file
  * `/find-save` Finds CK save object(s) in game save JSON file based on host of the game save

## Game Saves
Game saves are kept in a JSON file with the schema,
```json
{
  "Saves": [
    {
      "SaveName": "Name_Of_Save",
      "SaveDate": "01/01/2022 07:48 PM",
      "Players": [
        {
          "Player": "Player1",
          "Country": "Country1"
        },
        {
          "Player": "Player2",
          "Country": "Country2"
        }
      ]
    }
  ]
}
```
