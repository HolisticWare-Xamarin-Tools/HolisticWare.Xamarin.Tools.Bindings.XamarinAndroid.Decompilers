# HolisticWare.Xamarin.Tools.Bindings.XamarinAndroid.Decompilers


One of the key factors whether technology will be successful and accepted by users/consumers
is certainly - ecosystem - 3rd part libraries, SDKs and tools.

Xamarin's ecosystem and ability to enhance, extend that ecosystem is one of the biggest advantages
when compared to other cross platform development technologies. Enhancing ecosystem id mostly 
done through bindings. Though Xamarin tooling for binding have huge advantage compared to other
techmologies, tools itself are not always ideal and perfect.

This set of nuget packages is an attempt to improve developers experience and productivity through
improving the build system, IDE experience for Xamarin.Android bindings like:

*   code intellisene (autocompletion) for Managed Callable Wrappers MCW 

*   Decompiler dumps (java code dumps) for the cases when Xamarin tools cannot surface
    some classes

This repository is personal side project to add code completion (intellisense) to MCWs and it
consists of custom MSBuild task that hooks in the Xamarin.Android bindings build task (process),
intercepts it and adds generated code tot the IDE enabling code completion and navigation through
the code.

NOTE: this is personal attempt to learn MSBuild via solving some problems and code is not ideal, 
so any kind of suggestions, ideas, constructive criticism is accepted.


## Decompilers support 

## Usage

1.  install nuget package `HolisticWare.Xamarin.Tools.Bindings.XamarinAndroid.Decompilers`

2.  do the bindings and decompiled code will be in `./holisticware-generated/decompilers`

3.  uninstall nuget package after bindings are done

NOTE: support for SDK style project is ongoing work due to lack of knowledge about file inclusion
mechanisms (Default).

## Motivation

This repository consists of the source for the build task in the `./source` folder and 2 bindings
projects in `./samples` folder. Though folder is named `./samples` those are not true Android
apps, but bindings projects for testing MSBuild tasks that are supposed to fix this issue.

There are 2 bindings projects in `./samples` folder - one classic Xamarin.Android bindings
project and other with new "SDK style project" flavour.

In some cases Xamarin.Android bindings tools (`jar2xml` and `class-parse`) cannot "surface" 
certain java API for various reasons (generics, obuscation, even bugs...).

In this case it is possible to inspect `*.jar` files (and `*.aar` of course) with various
decompilers for java binaries and to analyse the reasons why classes were not "surfaced" to
C# API (MCWs).

There are several decompilers with UI options:

1.	Luyten

2.	JD-GUI

3.	...

and of course several commandline options:

1.	`javap`

2.	procyon decompiler used by Luyten

3.	CFR decompiler

4. ...

This nuget package adds automatic decompiler dumps to Build target of the Xamarin.Android bindings
projects.

Dumps can be found in `./holisticware-generated/decompilers/*.class`


## TODOs

1.	MSBuild SDK style project support 

2. windows platform support 

3.	more parameters for customization (output folders, ...)

4. 	other Android artifacts support (`*.aar`, ...)

