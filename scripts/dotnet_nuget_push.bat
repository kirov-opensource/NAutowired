set nupkgname=%1%
dotnet nuget push %nupkgname%  -s GitHub
