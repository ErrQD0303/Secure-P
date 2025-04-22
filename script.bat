@echo off
setlocal enabledelayedexpansion

@REM set "classLibs=SecureP.Repository.AppUserParkingSubscriptions,SecureP.Service.AppUserParkingSubscriptionService"

@REM for %%l in (%classLibs%) do (
@REM     dotnet new classlib -o "%%l"
@REM     dotnet sln add "%%l/%%l.csproj"
@REM )

set "dependencyLibs=SecureP.Repository.AppUserParkingSubscriptions-SecureP.Repository.Abstraction,SecureP.Service.AppUserParkingSubscriptionService-SecureP.Service.Abstraction,SecureP.Service.AppUserParkingSubscriptionService-SecureP.Repository.AppUserParkingSubscriptions,tests/ServiceTests-SecureP.Service.AppUserParkingSubscriptionService,SecureP.Repository.AppUserParkingSubscriptions-SecureP.Data"

for %%l in (%dependencyLibs%) do (
    for /f "tokens=1,2 delims=-" %%a in ("%%l") do (
        set "libName=%%a"
        set "libDependency=%%b"
        dotnet add !libName! reference !libDependency!
    )
)

endlocal