#!/bin/bash
set pushKey="oy2cdyotfbd5rk7thnias2paqkriagtuxahd5t2n3woexy"
set pushHost="https://www.nuget.org"
declare -a projectList=(
	"./GR.Extensions/GR.Core.Extension/GR.Core/GR.Core" 
	"./GR.Extensions/GR.Core.Extension/GR.Core.Razor/GR.Core.Razor"
)
for project in "${projectList[@]}" 
do  projectName=$(echo "$project" | sed 's/.*\///')
    echo "Start to publish ${projectName}"
	dotnet pack $(echo "${project}.csproj") -o ../../../nupkgs | dotnet nuget push -k %pushKey% -s %pushHost% $(echo "${projectName}*")
done