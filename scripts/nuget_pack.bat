dotnet build NAutowired\NAutowired.csproj -c Release
dotnet build NAutowired.Core\NAutowired.Core.csproj -c Release
dotnet build NAutowired.Console\NAutowired.Console.csproj -c Release
nuget pack NAutowired\NAutowired.csproj -Prop Configuration=Release
nuget pack NAutowired.Core\NAutowired.Core.csproj -Prop Configuration=Release
nuget pack NAutowired.Console\NAutowired.Console.csproj -Prop Configuration=Release