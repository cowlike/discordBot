#!/bin/bash

export BOT_TOKEN=$1
nohup /opt/bot/publish/DiscordBotFSharp $1 >~/nohup.out 2>&1 &
