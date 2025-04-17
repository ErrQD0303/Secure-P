@echo off
setlocal enabledelayedexpansion

set "classLibs=tests/ApiTests"

for %%l in (%classlibs%) do (
    dotnet new xunit -o %%l
    dotnet sln add %%l/%%l.csproj
    dotnet add %%l/%%l.csproj package Microsoft.AspNetCore.Mvc.Testing
)

endlocal