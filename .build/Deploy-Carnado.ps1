# Version: 1.1
# - Added 'DEV' to the available Enviroment options.
# Version: 1.0
# - First draft. Replaces previous used Deploy-CarnadoToEnvironment cmdlet.

# Variables
# Loads the Publish-CarCom.ps1 cmdlet for use in the script below
. .\.build\Publish-CarCom.ps1

$Environment = Split-Path $Env:JOB_NAME -Leaf

switch($Environment){
    "CD"{
        $User = "DEV\.jenkinsdeploy"
        $URI = "http://w3.cd.dev.car.com/"
    }
    "Release"{
        $User = "DEV\.jenkinsdeploy"
        $URI = "http://w3.dev.car.com/"
    }
    "DEV"{
        $User = "DEV\.jenkinsdeploy"
        $URI = "http://w3.dev.car.com/"
    }
    "QA"{
        $User = "DEV\.jenkinsdeploy"
        $URI = "http://w3.qa.car.com/"
    }
    "STG"{
        $User = "PROD\.jenkinsdeploy"
        $URI = "http://w3.stg.car.com/"
    }
	"PROD"{
        $User = "PROD\.jenkinsdeploy"
        $URI = "http://w3.stg.car.com/"	
	}
}

$Password = ConvertTo-SecureString -String "huw_3thE" -AsPlainText -Force
$Credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $User, $Password

# Main
if($Environment -eq "Release"){
	Publish-CarCom -BuildNumber $Env:VERSION -Environment "DEV" -Credential $Credential
}else{
    Publish-CarCom -BuildNumber $Env:VERSION -Environment $Environment -Credential $Credential
}

$time = Measure-Command {
    $webrequest = Invoke-WebRequest -Uri $URI
}
Write-Host -Message "Time to load $URI is $time."