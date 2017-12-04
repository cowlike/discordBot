# Overview

This is a simple test bot that implements a few commands. All commands are preceded by the string "//". The bot only responds to messages starting with "//" that have not been sent by a bot (including itself!)

All of this can change at any time:

* echo "a string"
* showArgs 32I 42 98.4 "98.4" True 64U 99999999999999997
* showGuilds
* showDMChannels
* addChannel name
* rmChannel name
* rmChannel 1234567890U //channel id

# Adding the bot to your server

* login to https://discordapp.com/developers/docs/intro
* go to My Apps
* create a bot app
* copy the Client ID of the bot
* authorize the bot: https://discordapp.com/oauth2/authorize?client_id=CLIENT_ID_GOES_HERE&scope=bot

# Running the bot

This is a Dotnet Core 2.0 application. To run it, first export the bot token:

    export BOT_TOKEN=xxxxxxxxxxx

You can get the bot's token from the [My Apps](https://discordapp.com/developers/applications/me) page. After you export the token, you can run the app with:

    dotnet run