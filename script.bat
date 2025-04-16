@echo off
setlocal enabledelayedexpansion

set "baseClasslib=tests/ServiceTests"
set "classlibs=SecureP.Data SecureP.Repository.ParkingRates SecureP.Service.ParkingRateService"

for %%l in (%classlibs%) do (
    dotnet add %baseClasslib% reference %%l/%%l.csproj
)

endlocal