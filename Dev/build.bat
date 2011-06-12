set MicrosoftDotNet=%systemRoot%\Microsoft.NET

if %PROCESSOR_ARCHITECTURE%==x86 (
	set DotNetFramework=Framework
) else (
	set DotNetFramework=Framework64
)

set DotNetFrameworkVersionPath=%MicrosoftDotNet%\%DotNetFramework%\v4.0.30319

%DotNetFrameworkVersionPath%\MSbuild Keypadawan.sln /p:Configuration=Debug /p:Platform="Any CPU"

