Param (
    [string]$FullName
)


Write-Host "Updating: " + $FullName

# all lines end with closing tags
.\ReplaceIT.ps1 -File $FullName -Find "`r`n`r`n" -Replace "`r`n"
.\ReplaceIT.ps1 -File $FullName -Find "`r`n`r`n" -Replace "`r`n"
.\ReplaceIT.ps1 -File $FullName -Find "`r`n" -Replace " `r`n"
.\ReplaceIT.ps1 -File $FullName -Find "> `r`n<" -Replace ">`r`n<"
.\ReplaceIT.ps1 -File $FullName -Find " `r`n" -Replace " "

# expand span tags
.\ReplaceIT.ps1 -File $FullName -Find "<span " -Replace "`r`n<span "

# replace images
.\ReplaceIT.ps1 -File $FullName -Find "<span[^>]*><img[^>]*></span>" -Replace '<img class="myimgclass" src="images/image001.png" alt="" title="">'
.\ReplaceIT.ps1 -File $FullName -Find "<img[^>]*>" -Replace '<img class="myimgclass" src="images/image001.png" alt="" title="" />'

# dedupe super and subscripts
.\ReplaceIT.ps1 -File $FullName -Find "<sup><span[^>]*>" -Replace "<sup>"
.\ReplaceIT.ps1 -File $FullName -Find "<sub><span[^>]*>" -Replace "<sub>"
.\ReplaceIT.ps1 -File $FullName -Find "</span></sup>" -Replace "</sup>"
.\ReplaceIT.ps1 -File $FullName -Find "</span></sub>" -Replace "</sub>"

# superscripts => identify and insert
.\ReplaceIT.ps1 -File $FullName -Find "position:relative;top:[^1]*1.0[^>]*>" -Replace "insertsuper'>"
.\ReplaceIT.ps1 -File $FullName -Find "position:relative;top:[^2]*2.5[^>]*>" -Replace "insertsuper'>"
.\ReplaceIT.ps1 -File $FullName -Find "position:relative;top:[^4]*4.0[^>]*>" -Replace "insertsuper'>"
.\ReplaceIT.ps1 -File $FullName -Find "position:relative;top:[^4]*4.5[^>]*>" -Replace "insertsuper'>"
.\ReplaceIT.ps1 -File $FullName -Find "position:relative;top:[^5]*5.0[^>]*>" -Replace "insertsuper'>"
.\ReplaceIT.ps1 -File $FullName -Find "position:relative;top:[^5]*5.5[^>]*>" -Replace "insertsuper'>"

$Start = "insertsuper'>"
$End = "</span>"
$Pattern = $Start + "(.*?)" + $End    # [^<]*
$NewStart = "insertsuper'><sup>"
$NewEnd = "</sup></span>"
.\ReplaceIT.ps1 -File $FullName -AllMatches -Start $Start -End $End -Pattern $Pattern -NewStart $NewStart -NewEnd $NewEnd

# subscripts => identify  and insert
.\ReplaceIT.ps1 -File $FullName -Find "position:relative;top:[^1]*1.5[^>]*>" -Replace "insertsub'>"
.\ReplaceIT.ps1 -File $FullName -Find "position:relative;top:[^2]*2.0[^>]*>" -Replace "insertsub'>"
.\ReplaceIT.ps1 -File $FullName -Find "position:relative;top:[^3]*3.0[^>]*>" -Replace "insertsub'>"
.\ReplaceIT.ps1 -File $FullName -Find "position:relative;top:[^3]*3.5[^>]*>" -Replace "insertsub'>"

$Start = "insertsub'>"
$End = "</span>"
$Pattern = $Start + "(.*?)" + $End    # [^<]*
$NewStart = "insertsub'><sub>"
$NewEnd = "</sub></span>"
.\ReplaceIT.ps1 -File $FullName -AllMatches -Start $Start -End $End -Pattern $Pattern -NewStart $NewStart -NewEnd $NewEnd

# collapse span tags
.\ReplaceIT.ps1 -File $FullName -Find "`r`n<span " -Replace "<span "

# class | align | width | valign | style, | span | &nbsp; -- empty <p> tags -- border | cellpadding | cellspacing
.\ReplaceIT.ps1 -File $FullName -Find "\s+class=[^ >]*|\s+align=[^ >]*|\s+width=[^ >]*|\s+valign=[^ >]*|\s+style='+[^']*'|</?span+\s+[^>]*>|</span>|&nbsp;|<p></p>|\s+border=[^ >]*|\s+cellpadding=[^ >]*|\s+cellspacing=[^ >]*" -Replace ""

# style -- empty <p> | <b> | <i> -- div | br clearall
.\ReplaceIT.ps1 -File $FullName -Find "<p></p>|</b><b>|</i><i>|<div>|</div>|<br clear=all>" -Replace ""
.\ReplaceIT.ps1 -File $FullName -Find "<style>(.*`r`n)*</style>" -Replace "<style>.subsection { margin-left: 20px; }\r\n body { font-family:-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif }\r\n</style>"

# remove class, name, and style
.\ReplaceIT.ps1 -File $FullName -Find 'class="(.*?)"' -Replace ""
.\ReplaceIT.ps1 -File $FullName -Find '  ' -Replace " "
.\ReplaceIT.ps1 -File $FullName -Find 'style="(.*?)"' -Replace ""
.\ReplaceIT.ps1 -File $FullName -Find '  ' -Replace " "
.\ReplaceIT.ps1 -File $FullName -Find 'name="(.*?)"' -Replace ""
.\ReplaceIT.ps1 -File $FullName -Find '  ' -Replace " "
.\ReplaceIT.ps1 -File $FullName -Find ' >' -Replace ">"

# duplicate tags
.\ReplaceIT.ps1 -File $FullName -Find "</strong><strong>|</em><em>" -Replace ""
.\ReplaceIT.ps1 -File $FullName -Find "<a></a>|</a><a>" -Replace ""

# <b> and <i> => <strong> and <em>
.\ReplaceIT.ps1 -File $FullName -Find "`r`n</i>" -Replace "</i>"
.\ReplaceIT.ps1 -File $FullName -Find "`r`n</b>" -Replace "</b>"
.\ReplaceIT.ps1 -File $FullName -Find "<i>" -Replace "<em>"
.\ReplaceIT.ps1 -File $FullName -Find "</i>" -Replace "</em>"
.\ReplaceIT.ps1 -File $FullName -Find "<b>" -Replace "<strong>"
.\ReplaceIT.ps1 -File $FullName -Find "</b>" -Replace "</strong>"

# table formatting
.\ReplaceIT.ps1 -File $FullName -Find '<table>' -Replace '<table border="1" align="center" cellpadding="3" cellspacing="0">'
.\ReplaceIT.ps1 -File $FullName -Find "<td nowrap>" -Replace '<td nowrap="nowrap">'
.\ReplaceIT.ps1 -File $FullName -Find "<tr height=0>" -Replace "<tr>"
.\ReplaceIT.ps1 -File $FullName -Find "<td" -Replace "`r`n<td"
.\ReplaceIT.ps1 -File $FullName -Find "<tr" -Replace "`r`n<tr"

# combine adjacent super/subscript tags
.\ReplaceIT.ps1 -File $FullName -Find "insertsuper'>|insertsub'>" -Replace ""
.\ReplaceIT.ps1 -File $FullName -Find "</sup>`r`n<sup>" -Replace ""
.\ReplaceIT.ps1 -File $FullName -Find "`r`n<sup>" -Replace "<sup>"
.\ReplaceIT.ps1 -File $FullName -Find "</sub>`r`n<sub>" -Replace ""
.\ReplaceIT.ps1 -File $FullName -Find "`r`n<sub>" -Replace "<sub>"
.\ReplaceIT.ps1 -File $FullName -Find '</sup>\)' -Replace ')</sup>'
.\ReplaceIT.ps1 -File $FullName -Find '\(<sup>' -Replace '<sup>('
.\ReplaceIT.ps1 -File $FullName -Find '</sub>\)' -Replace ')</sub>'
.\ReplaceIT.ps1 -File $FullName -Find '\(<sub>' -Replace '<sub>('


# bullets => lists
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8226;"
.\ReplaceIT.ps1 -File $FullName -Find "&#149;" -Replace "&#8226;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#183;"
.\ReplaceIT.ps1 -File $FullName -Find "<p>&#183;" -Replace "<li>"
.\ReplaceIT.ps1 -File $FullName -Find "<p>&#8226;" -Replace "<li>"

$Start = "<li>"
$End = "</p>"
$Pattern = $Start + "(.*?)" + $End
$NewStart = "<li>"
$NewEnd = "</li>"
.\replaceit.ps1 -File $FullName -AllMatches -Start $Start -End $End -Pattern $Pattern -NewStart $NewStart -NewEnd $NewEnd

.\ReplaceIT.ps1 -File $FullName -Find "<li>" -Replace "<ul><li>"
.\ReplaceIT.ps1 -File $FullName -Find "</li>" -Replace "</li></ul>"
.\ReplaceIT.ps1 -File $FullName -Find "</ul>`r`n<ul>" -Replace "`r`n"

# FHWA
.\ReplaceIT.ps1 -File $FullName -Find '<html>' -Replace '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en-US" lang="en-US">'
.\ReplaceIT.ps1 -File $FullName -Find "<br>" -Replace "<br />"


# M$ space character !IMPORTANT!
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace ""

# START M$ ASCII to HTML 128 - 159
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8364;"
.\ReplaceIT.ps1 -File $FullName -Find "&#128;" -Replace "&#8364;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8218;"
.\ReplaceIT.ps1 -File $FullName -Find "&#130;" -Replace "&#8218;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#402;"
.\ReplaceIT.ps1 -File $FullName -Find "&#131;" -Replace "&#402;"
.\ReplaceIT.ps1 -File $FullName -Find '�' -Replace "&#8222;"
.\ReplaceIT.ps1 -File $FullName -Find "&#132;" -Replace "&#8222;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8230;"
.\ReplaceIT.ps1 -File $FullName -Find "&#133;" -Replace "&#8230;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8224;"
.\ReplaceIT.ps1 -File $FullName -Find "&#134;" -Replace "&#8224;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8225;"
.\ReplaceIT.ps1 -File $FullName -Find "&#135;" -Replace "&#8225;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#710;"
.\ReplaceIT.ps1 -File $FullName -Find "&#136;" -Replace "&#710;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8240;"
.\ReplaceIT.ps1 -File $FullName -Find "&#137;" -Replace "&#8240;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#352;"
.\ReplaceIT.ps1 -File $FullName -Find "&#138;" -Replace "&#352;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8249;"
.\ReplaceIT.ps1 -File $FullName -Find "&#139;" -Replace "&#8249;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#338;"
.\ReplaceIT.ps1 -File $FullName -Find "&#140;" -Replace "&#338;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#381;"
.\ReplaceIT.ps1 -File $FullName -Find "&#142;" -Replace "&#381;"

# .\ReplaceIT.ps1 -File $FullName -Find "�|�" -Replace "'"    # uncomment => eliminate single "Smart Quotes"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8216;"
.\ReplaceIT.ps1 -File $FullName -Find "&#145;" -Replace "&#8216;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8217;"
.\ReplaceIT.ps1 -File $FullName -Find "&#146;" -Replace "&#8217;"

# .\ReplaceIT.ps1 -File $FullName -Find '�|�' -Replace '"'    # uncomment => eliminate "Smart Quotes"
.\ReplaceIT.ps1 -File $FullName -Find '�' -Replace "&#8220;"
.\ReplaceIT.ps1 -File $FullName -Find "&#147;" -Replace "&#8220;"
.\ReplaceIT.ps1 -File $FullName -Find '�' -Replace "&#8221;"
.\ReplaceIT.ps1 -File $FullName -Find "&#148;" -Replace "&#8221;"

# .\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8226;"    # bullets => lists
# .\ReplaceIT.ps1 -File $FullName -Find "&#149;" -Replace "&#8226;"    # bullets => lists

.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8211;"
.\ReplaceIT.ps1 -File $FullName -Find "&#150;" -Replace "&#8211;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8212;"
.\ReplaceIT.ps1 -File $FullName -Find "&#151;" -Replace "&#8212;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#732;"
.\ReplaceIT.ps1 -File $FullName -Find "&#152;" -Replace "&#732;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8482;"
.\ReplaceIT.ps1 -File $FullName -Find "&#153;" -Replace "&#8482;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#353;"
.\ReplaceIT.ps1 -File $FullName -Find "&#154;" -Replace "&#353;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#8250;"
.\ReplaceIT.ps1 -File $FullName -Find "&#155;" -Replace "&#8250;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#339;"
.\ReplaceIT.ps1 -File $FullName -Find "&#156;" -Replace "&#339;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#382;"
.\ReplaceIT.ps1 -File $FullName -Find "&#158;" -Replace "&#382;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#376;"
.\ReplaceIT.ps1 -File $FullName -Find "&#159;" -Replace "&#376;"
# END M$ ASCII to HTML 128 - 159

# ASCII to HTML
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#161;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#162;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#163;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#164;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#165;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#166;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#167;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#168;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#169;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#170;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#171;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#172;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#173;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#174;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#175;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#176;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#177;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#178;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#179;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#180;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#181;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#182;"
# .\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#183;"    # bullets => lists
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#184;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#185;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#186;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#187;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#188;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#189;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#190;"
.\ReplaceIT.ps1 -File $FullName -Find "�" -Replace "&#191;"

# foreign language characters
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#192;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#193;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#194;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#195;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#196;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#197;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#198;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#199;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#200;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#201;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#202;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#203;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#204;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#205;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#206;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#207;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#208;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#209;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#210;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#211;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#212;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#213;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#214;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#215;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#216;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#217;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#218;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#219;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#220;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#221;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#222;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#223;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#224;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#225;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#226;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#227;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#228;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#229;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#230;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#231;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#232;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#233;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#234;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#235;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#236;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#237;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#238;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#239;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#240;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#241;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#242;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#243;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#244;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#245;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#246;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#247;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#248;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#249;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#250;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#251;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#252;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#253;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#254;"
.\CMatch.ps1 -File $FullName -Find "�" -Replace "&#255;"

# line breaks
.\ReplaceIT.ps1 -File $FullName -Find "`r`n `r`n" -Replace "`r`n"
.\ReplaceIT.ps1 -File $FullName -Find "`r`n`r`n`r`n" -Replace "`r`n`r`n"
.\ReplaceIT.ps1 -File $FullName -Find "`r`n`r`n`r`n" -Replace "`r`n`r`n"
.\ReplaceIT.ps1 -File $FullName -Find "`r`n`r`n`r`n" -Replace "`r`n`r`n"