# Version: 1.0
# - First draft.
function Write-Build(){
    $BuildNumber = "$Env:VERSION"

    Write-Host "BUILD NOTICE: Creating build.txt."
    New-Item -Path .\Car.Com -Name build.txt -ItemType file -Value $BuildNumber
}

# Version: 1.0
# - First draft.
# Version: 2.0
# - Added error catching and reporting.
function Run-Node(){
    $StartLocation = Get-Location
    Set-Location -Path .\Car.Com\App_Workflow

    Write-Host "BUILD NOTICE: Running 'npm install'."
    npm install
    if($Global:LASTEXITCODE -ne 0){throw "ERROR: Exit Code 1 - 'npm install' failed."}

    Write-Host "BUILD NOTICE: Running 'gulp'."
    gulp
    if($Global:LASTEXITCODE -ne 0){throw "ERROR: Exit Code 1 - 'gulp' failed."}

    Write-Host "BUILD NOTICE: Running 'gulp produce-busted-files'."
    gulp produce-busted-files
    if($Global:LASTEXITCODE -ne 0){throw "ERROR: Exit Code 1 - gulp 'produce-bused-files' failed."}

    Set-Location $StartLocation
}

# Version: 1.0
# - First draft.
# Version: 2.0
# - Added error catching and reporting.
# Version: 3.0
# - Added copying of .build/config files, and copying of App_Sql folder to Drop folder.
# Version: 3.1
# - Fixed issue where App_Sql subdirectories were not being copied.
# Version: 3.2
# - Added copying of tests to the Drop folder.
function Copy-WorkspaceToDrop(){
    $Drop = "D:\Drop"

    $Path = ".\Car.Com\App_Assets\dist"
    if($Env:JOB_NAME -eq "Consumer/Carnado/Release"){
        $Destination = Join-Path $Drop "$Env:JOB_NAME\$Env:VERSION\staticroot\car"
    }else{
        $Destination = Join-Path $Drop "$Env:JOB_NAME\staticroot\car"
    }

    Write-Host "BUILD NOTICE: Copying 'staticroot' to Drop folder."
    robocopy $Path $Destination /MIR /COPY:DAT /R:1 /W:5 /NP /V
    Test-RobocopyExitCode

    $Path = ".\Car.Com"
    
    if($Env:JOB_NAME -eq "Consumer/Carnado/Release"){
        $Destination = Join-Path $Drop "$Env:JOB_NAME\$Env:VERSION\wwwroot"
    }else{
        $Destination = Join-Path $Drop "$Env:JOB_NAME\wwwroot"
    }
    
    $ExcludeFiles = Get-Content ".\.build\exclude.txt" | where {$_ -like '*.*'}
    $ExcludeDirectories = Get-Content ".\.build\exclude.txt" | where {$_ -notlike '*.*'}
    
    # Removes the following two files from the array, as they do need to be copied over.
    $ExcludeFiles = $ExcludeFiles -ne "Web.config"
    $ExcludeFiles = $ExcludeFiles -ne "Services.config"

    Write-Host "BUILD NOTICE: Copying 'wwwroot' to Drop folder."
    robocopy $Path $Destination /XF $ExcludeFiles /XD $ExcludeDirectories /MIR /COPY:DAT /R:1 /W:5 /NP /V
    Test-RobocopyExitCode

    $Path = ".\.build\config_files"
    Write-Host "BUILD NOTICE: Copying '.build/config' files to Drop folder."
    robocopy $Path $Destination /E /COPY:DAT /R:1 /W:5 /NP /V
    Test-RobocopyExitCode

    $Path = ".\Car.Com.Service\Data\App_Sql"
    if($Env:JOB_NAME -eq "Consumer/Carnado/Release"){
        $Destination = Join-Path $Drop "$Env:JOB_NAME\$Env:VERSION\wwwroot\App_Sql"
    }else{
        $Destination = Join-Path $Drop "$Env:JOB_NAME\wwwroot\App_Sql"
    }
    Write-Host "BUILD NOTICE: Copying 'App_Sql' files to Drop folder."
    robocopy $Path $Destination /E /COPY:DAT /R:1 /W:5 /NP /V
    Test-RobocopyExitCode

	$Path = ".\Car.Com.Tests\bin\Release"
	if($Env:JOB_NAME -eq "Consumer/Carnado/Release"){
        $Destination = Join-Path $Drop "$Env:JOB_NAME\$Env:VERSION\tests"
    }else{
        $Destination = Join-Path $Drop "$Env:JOB_NAME\tests"
    }
	$Files = "Car.Com.Common.dll Car.Com.Tests.dll chromedriver.exe IEDriverServer.exe Newtonsoft.Json.dll nunit.framework.* phantomjs.exe TechTalk.Specflow.dll WebDriver.*"
	Write-Host "BUILD NOTICE: Copying 'tests' to Drop folder"
	robocopy $Path $Destination $Files /COPY:DAT /R:1 /W:5 /NP /V
	Test-RobocopyExitCode
}

# Version: 1.0
# - First draft.
function Test-RobocopyExitCode(){
    if($Global:LASTEXITCODE -ge 8){
        Write-Host "ERROR: robocopy failed."
        switch($Global:LASTEXITCODE){
		    "8"{throw "Robocopy Failed: Exit Code 08 - FAIL"}
		    "9"{throw "Robocopy Failed: Exit Code 09 - OKCOPY + FAIL"}
		    "10"{throw "Robocopy Failed: Exit Code 10 - FAIL + XTRA"}
		    "11"{throw "Robocopy Failed: Exit Code 11 - OKCOPY + FAIL + XTRA"}
		    "12"{throw "Robocopy Failed: Exit Code 12 - FAIL + MISMATCHES"}
		    "13"{throw "Robocopy Failed: Exit Code 13 - OKCOPY + FAIL + MISMATCHES"}
		    "14"{throw "Robocopy Failed: Exit Code 14 - FAIL + MISMATCHES + XTRA"}
		    "15"{throw "Robocopy Failed: Exit Code 15 - OKCOPY + FAIL + MISMATCHES + XTRA"}
		    "16"{throw "Robocopy Failed: Exit Code 16 - FATAL ERROR"}
	    }
    }else{
		$Global:LASTEXITCODE = 0
	}
}

# Variables
$User = "DEV\.jenkinsdeploy"
$Password = ConvertTo-SecureString -String "huw_3thE" -AsPlainText -Force
$Credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $User, $Password

# Main
Write-Build
Run-Node
if($Env:JOB_NAME -ne "Consumer/Carnado/CI"){
    Copy-WorkspaceToDrop
}