#!/bin/bash
wget http://nuget.org/nuget.exe
mkdir -p ./KLog/.nuget
cp nuget.exe ./KLog/.nuget/NuGet.exe
chmod a+x ./KLog/.nuget/NuGet.exe