VERSION=$1

if [ "x$VERSION" = "x" ]; then
  echo "pass version string as first argument"
  exit 1
fi

echo "using version $VERSION"

dotnet publish -c release -r linux-x64 -o publish
docker build -f Dockerfile \
  --build-arg BOT_TOKEN=`cat token` \
  -t cowlike/discord-bot:${VERSION} \
  -t cowlike/discord-bot:linux \
  -t cowlike/discord-bot:latest .

docker push cowlike/discord-bot:${VERSION}
docker push cowlike/discord-bot:linux
docker push cowlike/discord-bot:latest
