# Version: 1.0
# - First draft.

function Set-AlternatingCSSClasses {
    <#
    .SYNOPSIS
    Sets the CSS Class attribute to Odd or Even for an HTML fragment, usually a table.
    .DESCRIPTION
    Sets the CSS Class attribute to Odd or Even for an HTML fragment, usually a table.
    .PARAMETER HTMLFragment
    .PARAMETER CSSEvenClass
    .PARAMETER CSSOddClass
    .EXAMPLE
    Set-AlternatingCSSClasses -HTMLFragment $fragment -CSSEvenClass "even" -CSSOddClass "odd"
    #>

    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$True,ValueFromPipeline=$True)]
        [string]$HTMLFragment,
        [Parameter(Mandatory=$True)]
        [string]$CSSEvenClass,
        [Parameter(Mandatory=$True)]
        [string]$CssOddClass
    )

    [xml]$xml = $HTMLFragment
    
    $table = $xml.SelectSingleNode('table')
    
    $classname = $CSSOddClass
    
    foreach ($tr in $table.tr) {
        if ($classname -eq $CSSEvenClass) {
            $classname = $CssOddClass
        } else {
            $classname = $CSSEvenClass
        }
        
        $class = $xml.CreateAttribute('class')
        
        $class.value = $classname
        
        $tr.attributes.append($class) | Out-null
    }
    
    $xml.innerxml | out-string
}

$URI = $Env:BUILD_URL + "api/json?tree=changeSet[items[comment,date,version,author[fullName]]]"
$json = (Invoke-WebRequest -Uri $URI).Content | Out-String | ConvertFrom-Json

foreach($item in $json.changeSet.items){
    if($item){
        $changes += @(@{
            ChangeSet = $item.version;
            Date = ([timezone]::CurrentTimeZone.ToLocalTime(([datetime]'1/1/1970').AddSeconds($item.date.ToString().Substring(0, $item.date.ToString().Length - 3)))).ToShortDateString();
            Author = $item.author.fullName;
            Comment = $item.comment
        })
    }
}
$html_changeset = $changes | 
					ForEach-Object {New-Object psobject -Property $_} |
                    ConvertTo-Html -As Table -Property ChangeSet,Date,Author,Comment -Fragment | 
                    Out-String |
					Set-AlternatingCSSClasses -CSSEvenClass "even" -CssOddClass "odd"

$html_changeset = "<h2>Changes</h2>$html_changeset"

# Remove tab, newline and carriage returns.
$html_changeset = $html_changeset -replace "`t|`n|`r",""

$output = "CHANGESETCONTENTS=" + $html_changeset
$output = $output -replace "`t|`n|`r","" 
$output | Out-File -Encoding ascii .build\build.properties


