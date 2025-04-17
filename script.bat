@echo off
setlocal enabledelayedexpansion

set "classLibs=SecureP.Repository.ParkingZones SecureP.Service.ParkingZoneService"

for %%l in (%classLibs%) do (
    dotnet new classlib -n %%l
    dotnet sln add %%l/%%l.csproj
)

dotnet add SecureP.Repository.ParkingZones/SecureP.Repository.ParkingZones.csproj reference SecureP.Repository.Abstraction/SecureP.Repository.Abstraction.csproj
dotnet add SecureP.Service.ParkingZoneService/SecureP.Service.ParkingZoneService.csproj reference SecureP.Service.Abstraction/SecureP.Service.Abstraction.csproj
dotnet add SecureP.Service.ParkingZoneService/SecureP.Service.ParkingZoneService.csproj reference SecureP.Repository.Abstraction/SecureP.Repository.Abstraction.csproj

endlocal