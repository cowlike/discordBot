FROM microsoft/dotnet:2-runtime
ARG BOT_TOKEN=none
WORKDIR /opt/bot
ADD DiscordBotFSharp/publish .
ENV PATH="/opt/bot:${PATH}"
ENV BOT_TOKEN ${BOT_TOKEN}
EXPOSE 8080
CMD ["DiscordBotFSharp"]
