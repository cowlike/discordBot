# Overview
This is a simple test bot that implements a few commands. All commands are preceded by the string "//". The bot only responds to messages starting with "//" that have not been sent by a bot (including itself!)

All of this can change at any time:

* echo "a string"
* showArgs 'I32 42 98.4 "98.4" True 'U64 99999999999999997
* showGuilds
* addChannel name
* rmChannel name
* rmChannel 'U64 id

# Adding the bot to your server
* login to https://discordapp.com/developers/docs/intro
* go to My Apps
* create a bot app
* copy the Client ID of the bot
* authorize the bot: https://discordapp.com/oauth2/authorize?client_id=CLIENT_ID_GOES_HERE&scope=bot