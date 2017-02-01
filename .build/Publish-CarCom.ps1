function Publish-CarCom{
    <#
        VERSION: 1.0
            - First Draft.
        VERSION: 1.1
            - Added Update-SupportFiles.
        VERSION: 2.0
            - Restructured cmdlet to be managed by either Jenkis or an SSC Analyst.
		VERSION: 2.1
			- Fixed the Restart-IISAppPool function.
    #>
    
    <#
        .Synopsis
           The Publish-CarCom cmdlet is used to deploy the www.car.com code to the Production environment.
        .DESCRIPTION
           The Publish-CarCom cmdlet copies the www.car.com build from the build server to the www.car.com servers in Production. It primarily uses robocopy as the mechanism to do the copying.
        .EXAMPLE
           Publish-CarCom -BuildNumber 20150527.1 -Environment CD -Credentials $Credential
           Publish-CarCom -BuildNumber 20150527.1 -Environment DEV -Credentials $Credential
           Publish-CarCom -BuildNumber 20150527.1 -Environment QA -Credentials $Credential
           Publish-CarCom -BuildNumber 20150527.1 -Environment STG -Credentials $Credential
           Publish-CarCom -BuildNumber 20150527.1 -Environment PROD -Credentials $Credential
        
           The above examples are how the cmdlet would be called from Jenkins.
        .EXAMPLE
           Publish-CarCom -BuildNumber 20150527.1 -Environment PROD -Credentials $prodcred -Email devops@autobytel.com -SSC

           This is how the SSC would use the cmdlet for managed deploys to PROD.
    #>

    [CmdletBinding()]
    PARAM(
        # The BuildNumber parameter is specific build that will be deployed. It determines where the script will find the files to copy. 
        [Alias("BN")]
        [Parameter(Mandatory=$true,Position=0)]
        [ValidatePattern("20[0-9][0-9][0-9][0-9][0-9][0-9].[0-9]")]
        [string]$BuildNumber,

        # The Environment parameter specifies the which environment that the script will deploy to. Currently it only supports PROD.
        [Alias("ENV")]
        [Parameter(Mandatory=$true,Position=1)]
        [ValidateSet("CD","DEV","QA","STG","PROD")]
        [string]$Environment,

        # The Credential parameter is used specifically to pass your PROD domain credentials in with. Typically you should be able to pass in the variable $prodcred.
        [Parameter(Mandatory=$true,Position=2)]
        [ValidateScript({
            if($_ -is [pscredential]){$True}elseif($_ -is [string]){$Script:Credential=Get-Credential -Credential $_;$True}else{Write-Error "You passed an unexpected object type for the credential."}
        })]
        [object]$Credential,

        # The Email parameter if included, it will cause the output to be emailed to the provided email address.
        [Parameter(Mandatory=$false)]
        [ValidateScript({[bool]($_ -as [System.Net.Mail.MailAddress])})]
        [string[]]$Email,

        # The SSC parameter is a switch. It is only used for manual managed deployments by the SSC to the PROD environment.
        [Parameter(Mandatory=$false)]
        [switch]$SSC
    )
    
    BEGIN{
        function Clear-StaticRoot{
            [CmdletBinding()]
            param([string]$ServerName)

            begin{
                $Destination = "\\$ServerName\staticroot"
                Write-Host "Cleaning up staticroot folder on $ServerName."
            }
            
            process{
                # Clean up any files, not directories, older than 90 days in the staticroot\car directory and its subdirectories
                Get-ChildItem "$Destination\car" -Recurse | 
			        where{($_.LastWriteTime -lt (Get-Date).AddDays(-90)) -and (-not $_.PSIsContainer)} | 
			        Remove-Item -Force
                
                # Targeted clean up of files in the \css and \js folders.
                $SubFolders += @("$Destination\car\css")
                $SubFolders += @("$Destination\car\js")

                foreach($SubFolder in $SubFolders){
                    $files += Get-ChildItem $SubFolder |
                        Where-Object {$_.Name -match "\w+\.\w+\.\w+-\w{8}\.min\.(css|js)"}

                    $fileprefixes = ($files).Name.Split("-") | 
                        Sort-Object -Unique | 
                        Where-Object {($_ -notlike "*.min.css") -and ($_ -notlike "*.min.js")}

                    foreach($prefix in $fileprefixes){
                        $similarfiles = $files | 
                            Where-Object {$_ -like "$prefix*"}
                        if($similarfiles.Count -gt 2){
                            $similarfiles = $similarfiles | 
                                Sort-Object -Property LastWriteTime -Descending
                            $similarfiles = $similarfiles[2..($similarfiles.Count-1)]
                            foreach($similarfile in $similarfiles){
                                Remove-Item $similarfile.FullName -Force
                            }
                        }
                    }
                    Clear-Variable files    
                }
            }   
        }

        function Copy-StaticRoot{
            [CmdletBinding()]
            param([string]$ServerName)

            begin{
                $ExlcudeThisFile = "KeepAlive.html"
                if($Environment -eq "CD"){
                    $Path = "\\jciapp401.prod.autobytel.com\Drop\Consumer\Carnado\CD\staticroot"
                }else{
                    $Path = "\\jciapp401.prod.autobytel.com\Drop\Consumer\Carnado\Release\$BuildNumber\staticroot"
                }
                $Destination = "\\$ServerName\staticroot"
                if($Credential){net use $Destination /USER:$UserName $Password}
            }
            
            process{
                robocopy $Path $Destination /XF $ExlcudeThisFile /E /COPY:DAT /R:1 /W:5 /NP /V
                Test-RobocopyExitCode
                Clear-StaticRoot -ServerName $ServerName
            }

            end{
                if($Credential){net use $Destination /DELETE}
            }
        }

        function Copy-WWWRoot{
            [CmdletBinding()]
            param([string]$ServerName)

            begin{
				Write-Host "Deploying the WWW site to $ServerName..."
                if($Environment -eq "CD"){
                    $Path = "\\jciapp401.prod.autobytel.com\Drop\Consumer\Carnado\CD\wwwroot"
                }else{
                    $Path = "\\jciapp401.prod.autobytel.com\Drop\Consumer\Carnado\Release\$BuildNumber\wwwroot"
                }
                $Destination = "\\$ServerName\wwwroot"
                if($Credential){net use $Destination /USER:$UserName $Password}
            }

            process{
                robocopy $Path $Destination /XF $ExcludeFiles /XD $ExcludeDirectories /MIR /COPY:DAT /R:1 /W:5 /NP /V
                Test-RobocopyExitCode 
            }

            end{
                if($Credential){net use $Destination /DELETE}
				Write-Host "Deploy of the WWW site on $ServerName is complete!"
            }
        }
        
		function Restart-IISAppPool{
			[CmdletBinding()]
			param(
				[string]$ServerName, 
				[string]$AppPool
			)
            
			begin{
				Write-Host "Recycling the '$AppPool' Application Pool."

				if($Credential){
					$Session = New-PSSession -ComputerName $ServerName -Credential $Credential
				}
				else{
					$Session = New-PSSession -ComputerName $ServerName
				}
				Invoke-Command -Session $Session {Import-Module -Name WebAdministration}
			}
                 
			process{
				Invoke-Command -Session $Session {Restart-WebAppPool -Name "www"}
				# Write-Host "The current status of the 'www' Application Pool on is $status."
				Write-Host "The '$AppPool' Appliction Pool has been recycled."
			}

			end{
				Invoke-Command -Session $Session {Remove-Module -Name WebAdministration}
				Remove-PSSession -Session $Session
			}
		}
        
        function Resume-CSSService{
            [CmdletBinding()]
            param([string]$Uri)
            begin{}
            process{}
            end{}
        }
        
        # Used to split an array's even rows into a seperate array
        function Split-Even{
            [CmdletBinding()]
            param($inArray)
            begin{$count = 0;$outArray = @()}            
            process{foreach($item in $inArray){if($count %2){$outArray += $item}else{};$count++}}
            end{return ,$outArray}
        }
            
        # Used to split an array's odd rows into a seperate array
        function Split-Odd{
            [CmdletBinding()]
            param($inArray)
            begin{$count = 0; $outArray = @()}
            process{foreach($item in $inArray){if($count %2){}else{$outArray += $item};$count++}}
            end{return ,$outArray}
        }

        function Suspend-CSSService{
            [CmdletBinding()]
            param([string]$Uri)
            begin{}
            process{}
            end{}
        }
        
        function Test-RobocopyExitCode{
            process{
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
        }
        
        function Test-CarCom{
            [CmdletBinding()]
            param([string]$Uri)
            
            begin{
                <#
                $BasePath = Join-Path (($env:PSModulePath).Split(';')[0]) "SSCAnalyst\bin"

                [Reflection.Assembly]::LoadFile(“$basepath\Newtonsoft.Json.dll”) | Out-Null
                [Reflection.Assembly]::LoadFile(“$basepath\nunit.framework.dll”) | Out-Null
                [Reflection.Assembly]::LoadFile(“$basepath\WebDriver.dll”) | Out-Null
                [Reflection.Assembly]::LoadFile(“$basepath\WebDriver.Support.dll”) | Out-Null
                [Reflection.Assembly]::LoadFile(“$basepath\Car.Com.Tests.dll”) | Out-Null

                $test = New-Object “Car.Com.Tests.SmokeTest”

                $test.SiteDomain = $Uri
                $test.Browser = "PhantomJS"
                #>
            }

            process{
                <#
                $test.SetUp()
                $test.SectionOne()
                $test.TearDown()

                $test.SetUp()
                $test.SectionTwo()
                $test.TearDown()

                $test.SetUp()
                $test.SectionThree()
                $test.TearDown()

                $test.SetUp()
                $test.SectionFour()
                $test.TearDown()

                $test.SetUp()
                $test.SectionFive()
                $test.TearDown()

                $test.SetUp()
                $test.SectionSix()
                $test.TearDown()

                $test.SetUp()
                $test.SectionSeven()
                $test.TearDown()

                $test.SetUp()
                $test.SectionEight()
                $test.TearDown()
                #>
            }
            
            end{}
        }

        function Update-SupportFiles{
            begin{
                # Carnado
                $Applications += @(@{
                    "Project" = "$/Core/Web/Carnado";
                    "WorkFold" = "C:\Temp\Carnado";
                    "Files" = @("./.build/exclude.txt",
                                "./.build/environment.json");
                    "FilePrefix" = "car.com.";
                })

                $LocalModuleBinDir = "C:\Users\$env:USERNAME\Documents\WindowsPowerShell\Modules\SSCAnalyst\bin"
                $LocalModuleConfigDir = "C:\Users\$env:USERNAME\Documents\WindowsPowerShell\Modules\SSCAnalyst\config"
                $RemoteModuleBinDir = "\\sscutil201\Scripts\WindowsPowerShell\Modules\SSCAnalyst\bin"
                $RemoteModuleConfigDir = "\\sscutil201\Scripts\WindowsPowerShell\Modules\SSCAnalyst\config"
                $TFSConsumer = "http://tfsapp401.prod.autobytel.com:8080/tfs/Consumer"
                $WorkSpaceNameAndOwner = "Publish-CarCom;" + $UserName
            }

            process{
                # If this is not run first, the cmdlet will be unable to connect to TFS and get the files
				tf workspaces /format:brief /collection:$TFSConsumer
				
                tf workspace /delete $WorkSpaceNameAndOwner /noprompt /collection:$TFSConsumer
                tf workspace /new $WorkSpaceNameAndOwner /noprompt /collection:$TFSConsumer

                # Get all config files and copy to the SSCAnalyst module folder
                foreach($App in $Applications){
                    New-Item -Path $App.WorkFold -ItemType directory -Force
                    tf workfold /map $App.Project $App.WorkFold /workspace:$WorkSpaceNameAndOwner /collection:$TFSConsumer
                    Set-Location $App.WorkFold
                    foreach($File in $App.Files){
                        tf get $File /force
                        $Destination = $RemoteModuleConfigDir + "\" + $App.FilePrefix + (Split-Path $File -Leaf)
                        Copy-Item -Path $File -Destination $Destination -Force
                        $Destination = $LocalModuleConfigDir + "\" + $App.FilePrefix + (Split-Path $File -Leaf)
                        Copy-Item -Path $File -Destination $Destination -Force
                    }
                    tf workfold /unmap $App.WorkFold "/workspace:$WorkSpaceNameAndOwner"
                    Set-Location ..
                    Remove-Item -Path $App.WorkFold -Recurse -Force
                }

                # Get all binary files and copy to the SSCAnalyst module folder     
                $TestsDir = (Get-ChildItem "\\jciapp401\Drop\Consumer\Carnado\Release" -Directory | Sort CreationTime -Descending | Select -First 1).FullName + "\tests\"
                $Files = Get-ChildItem $TestsDir
                foreach($File in $Files){
                    Copy-Item -Path (Join-Path $TestsDir $File) -Destination $RemoteModuleBinDir -Force
                    Copy-Item -Path (Join-Path $TestsDir $File) -Destination $LocalModuleBinDir -Force
                }
            }

			end{
				tf workspace /delete $WorkSpaceNameAndOwner /noprompt /collection:$TFSConsumer
			}
        }

        function Copy-CarCom{
            [CmdletBinding()]
            param(
                [ValidateSet("A", "B")]
                [string]$ServerGroup
            )
            begin{
                Write-Host "`nDeploying build $BuildNumber to the CCWEB WWW '$ServerGroup' group sites."
                if($ServerGroup -eq 'A'){
                    $Servers = Split-Odd $ServerList.$Environment.ComputerName
                }else{
                    $Servers = Split-Even $ServerList.$Environment.ComputerName
                }
            }
            process{
                foreach($Server in $Servers){
                    if($Server){
                        $SimpleComputerName = $Server.Split('.')[0]
                        $URI = "http://" + $SimpleComputerName + "-www.prod.autobytel.com"
                        
                        do{
							$message = "`nIs the service " + $SimpleComputerName + "-www suspended in the SLB and connections equal to 0? (y/n)"
							Write-Host $message -BackgroundColor DarkRed
							$ready = Read-Host
						}
                        until($ready.trim() -eq "y")
                        
                        # Suspends the CSS Service for current server. Then waits till its Connections equal 0 before continuing.
                        Suspend-CSSService $URI
                        
                        Write-Host "`nMake any web.config or service.config changes now." -BackgroundColor DarkRed
                        pause
                        Copy-WWWRoot -ServerName $Server
                        Restart-IISAppPool -ServerName $Server -AppPool "www"
                        
                        Write-Host "Loading website for $URI..."
                        $time = Measure-Command {
                            Invoke-WebRequest -Uri $URI -TimeoutSec 60 -ErrorAction SilentlyContinue
                        }
                        Write-Host "Time to load $URI is $time." -ForegroundColor Yellow
                        
                        Start-Process "iexplore.exe" -ArgumentList "$URI"
                        Write-Host "`nVisually confirm that $URI renders." -BackgroundColor DarkRed
                        pause

                        # Run the SmokeTest on the current server.
                        Test-CarCom -Uri $URI
                        
                        # Active the CSS Service for the current server.
                        Resume-CSSService -Uri $URI

                        $message = "`nDeploy of the WWW site on " + $Server + " is complete!"
                        Write-Host $message -ForegroundColor Green
                        
                        $message = "`nActivate the service " + $SimpleComputerName + "-www in the SLB."
                        Write-Host $message -BackgroundColor DarkRed
                        pause
                    }
                }
            }
            end{}
        }

        function Manage-Deploy{
            process{
                Start-Transcript -Path $Transcript

                Write-Host "`nDeploying to $Environment. Special APPROVAL REQUIRED from IT Ops Management."
                Write-Host "`nType CARNADO if you want to deploy to $Environment :" -BackgroundColor DarkRed
                $approval_required = Read-Host

                if($approval_required -eq "CARNADO"){
                    Write-Host "Deploying build $BuildNumber to all CCWEB Static sites sequentially."
                    foreach($Server in $ServerList.$Environment){
                        if($Server.ComputerName){
                            Copy-StaticRoot -ServerName $Server.ComputerName
                            $message = "Deploy of the Static site on " + $Server.ComputerName + " is complete!"
                            Write-Host $message -ForegroundColor Green
                        }
                    }
            
                    # Deploy to A server group first
                    Copy-CarCom -ServerGroup 'A'

                    Write-Host "`nDeploy of build $BuildNumber to the CCWEB WWW 'A' group sites is complete! " -ForegroundColor Green

                    Write-Host "`nInitiate the 15 minute burn in period." -BackgroundColor DarkRed
                    pause

                    # Deploy to B server group second
                    Copy-CarCom -ServerGroup 'B'

                    Write-Host "`nDeploy of build $BuildNumber to the CCWEB WWW 'B' group sites is complete! " -ForegroundColor Green

                    Write-Host "`nDeployment complete!" -ForegroundColor Green
                }
                else{
                    Write-Host "`nDeploy of Carnado build $BuildNumber to $Environment aborted." -ForegroundColor Green
                }

                Stop-Transcript
            }
        }

        if($Email){
            $SmtpServer = "alerts.mail.bot.services.prod.autobytel.com"
            $From = "ssc@autobytel.com"
            $Subject = "Consumer/Carnado/Deploy/$Environment - Build $BuildNumber"

            $cssstyle = "<style>body{color:#333333;font-family:Calibri,Tahoma;font-size: 10pt;}h1{text-align:center;}h2{border-top:1px solid #666666;}.header{font-weight:bold;color:#eeeeee;background-color:#333333;}</style>"
            $html_head = "<title>$Subject</title>$cssstyle"
            $html_preContent = "<h1>$Subject</h1>"
        }
        
        if($SSC){
            # Used by SSC managed deploys to Environment.

            # Updates the /bin and /config files needed below to their latest version from TFS.
            Update-SupportFiles

            #$PSModulePath = $env:PSModulePath.Split(';')[0] + "SSCAnalyst"
            $PSModulePath = "C:\Users\$env:USERNAME\Documents\WindowsPowerShell\Modules\SSCAnalyst"
            $ServerList = Get-Content "$PSModulePath\config\car.com.environment.json" | Out-String | ConvertFrom-Json
            $ExcludeFiles = Get-Content "$PSModulePath\config\car.com.exclude.txt" | where {$_ -like '*.*'}
            $ExcludeDirectories = Get-Content "$PSModulePath\config\car.com.exclude.txt" | where {$_ -notlike '*.*'}

            $Transcript = ".\Consumer-Carnado-Deploy-$Environment-$BuildNumber.txt"
        }else{
            # Used by Jenkins managed deploys to PROD.
            $ServerList = Get-Content ".\.build\environment.json" | Out-String | ConvertFrom-Json
            $ExcludeFiles = Get-Content ".\.build\exclude.txt" | where {$_ -like '*.*'}
            $ExcludeDirectories = Get-Content ".\.build\exclude.txt" | where {$_ -notlike '*.*'}
        }

        if($Credential){
            $UserName = $Credential.UserName.ToString()
            $Password = $Credential.GetNetworkCredential().password
        } 
    }
    
    PROCESS{
        switch($Environment){
            {($_ -eq "CD") -or ($_ -eq "DEV") -or ($_ -eq "QA")}{
                foreach($Server in $ServerList.$Environment){
                    if($Server.ComputerName){
                        Copy-StaticRoot -ServerName $Server.ComputerName
                        Copy-WWWRoot -ServerName $Server.ComputerName
                    }
                }
            }
            "STG"{
                if($SSC){
                    # Beginning SSC managed deploy to STG.
					$DeployStart = (Get-Date).DateTime
                    Manage-Deploy
					$DeployFinish = (Get-Date).DateTime
                }else{
                    # Beginning Jenkins managed deploy to STG.
                    foreach($Server in $ServerList.$Environment){
                        if($Server.ComputerName){
                            Copy-StaticRoot -ServerName $Server.ComputerName
                        }
                    }
                    foreach($Server in $ServerList.$Environment){
                        if($Server.ComputerName){
                            Copy-WWWRoot -ServerName $Server.ComputerName
                        }
                    }
                }
            }
            "PROD"{
                if($SSC){
                    # Beginning SSC managed deploy to PROD.
					$DeployStart = (Get-Date).DateTime
                    Manage-Deploy
					$DeployFinish = (Get-Date).DateTime
                }else{
                    # Beginning Jenkins managed deploy to PROD.
                    foreach($Server in $ServerList.$Environment){
                        if($Server.ComputerName){
                            Copy-StaticRoot -ServerName $Server.ComputerName
                        }
                    }
                    foreach($Server in $ServerList.$Environment){
                        if($Server.ComputerName){
                            Copy-WWWRoot -ServerName $Server.ComputerName
                        }
                    }
                }
            }
        }
    }
    
    END{
        # Displays the output of the cmdlet either in an email or to the CLI.
        if($Email){ 
            # Output should be a nice log of all the major events that happend in the deploy. It should be displayed on screen as well as sent out in an email.
            # TODO: update environment.json to include URI for each Environment. Use the $ServerList.$Environment.URI to provide the link for Web Site.
            $html_postContent = "<table><tr><td class='header'>Job Name</td><td>Publish-CarCom</td></tr><tr><td class='header'>Build Number</td><td>$BuildNUmber</td></tr><tr><td class='header'>Deployed By</td><td>$UserName</td></tr><tr><td class='header'>Deploy Start</td><td>$DeployStart</td></tr><tr><td class='header'>Deploy Finish</td><td>$DeployFinish</td></tr><tr><td class='header'>Web Site</td><td><a href='http://www.car.com'>http://www.car.com</a></td></tr></table>"

            # this hash object has all the parts of an HTML page
            $params = @{
                "Head" = $html_head;
                "PreContent" = $html_preContent;
                "PostContent" = $html_postContent;
            }

            # Converts the hash object with HTML parts into a page
            $Body = ConvertTo-Html @params | Out-String

            # Output sent via email
            Send-MailMessage -To $Email -Subject $Subject -Body $Body -SmtpServer $SmtpServer -From $From -BodyAsHtml -Attachments $Transcript
        }
    }
}