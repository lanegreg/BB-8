# Version: 1.1
# - Updated to be able to process multiple computers (aka hostnames) for a person.
# Version: 1.0
# - First draft
$Contacts = Get-Content ".\.build\growlnotify.json" | Out-String | ConvertFrom-Json

$Image = "D:\Program Files (x86)\Jenkins\war\images\headshot.png"
$Message = "$Env:JOB_NAME Build Status: Failed."
$Password = "leeroy"


foreach($Contact in $Contacts){
	foreach($hostname in $Contact.hostname){
		growlnotify /t:"Jenkins" /id:1234 /s:true /i:$Image /cu:"$Env:JOB_URL" /host:$hostname /port:23053 /pass:$Password $Message
	}
}