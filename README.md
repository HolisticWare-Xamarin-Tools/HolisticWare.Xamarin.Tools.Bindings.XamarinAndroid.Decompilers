# HolisticWare.Xamarin.Tools.Bindings.XamarinAndroid.Intellisense


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


## Code Intellisense for Managed Callable Wrappers

## Usage

1.  install nuget package `HolisticWare.Xamarin.Tools.Bindings.XamarinAndroid.Intellisense`

2.  do the bindings and enjoy code completion in MCW C# code

3.  uninstall nuget package after bindings are done

## Motivation

This repository consists of the source for the build task in the `./source` folder and 2 bindings
projects in `./samples` folder. Though folder is named `./samples` those are not true Android
apps, but bindings projects for testing MSBuild tasks that are supposed to fix this issue.

There are 2 bindings projects in `./samples` folder - one classic Xamarin.Android bindings
project and other with new "SDK style project" flavour.

Xamarin Android bindings tools (bindings generator) emit MCW (C# wrapper code) to

```
<!--
MCWs C# code is emitted to folowing folder:
$(IntermediateOutputPath = "obj/$(Configuration)/generated/src/"
$(IntermediateOutputPath = "obj/Debug/generated/src/"
$(IntermediateOutputPath = "obj/Release/generated/src/"
-->
<BindingMCWSourceFiles Include="$(IntermediateOutputPath)\generated\src\*"></BindingMCWSourceFiles>  
```

Normally this code is included in build process, but not in the bindings project itself, so
IDE intellisense (code completion) does not work and very often navigation through generated
code is needed when fixing compile (C#) errors due differences between c# and java.

Go ahead clone this repo, open solution in the `./samples` folder and try to compile project
`Sample.XamarinAndroid.Bindings.Library`. This is classic Xamarin.Android bindings project
to bind Google GRPC Stub library (jar). There should be 2 errors from MCW C# code. Clicking
on the error code will be opened in the IDE, but user cannot navigate to generated classes
(for 1st error for example `global::IO.Grpc.Stub.IStreamObserver`).

For years one personal trick to obtain intellisense was to add following code to the bindings project:

```
<ItemGroup>
	<Compile Include="obj\Debug\generated\src\*.cs" />
	<Compile Include="obj\Release\generated\src\*.cs" />
	<None Include="obj\Debug\api.xml" />
	<None Include="obj\Release\api.xml" />
</ItemGroup>
```

or 

```
<ItemGroup>
	<Compile Include="obj\$(Configuration)\generated\src\*.cs" />
	<None Include="obj\$(Configuration)\api.xml" />
</ItemGroup>
```

Adding this code to the project enables code completion and navigation, so when the file with 
generated C# MCW code is opened user will be able to navigate with "Go to definition" to various
classes, interfaces etc...

But this approach is not ideal, because next consecutive build will fail due to duplicate code
(included dynamically with MSBuild tasks and with project code snippet from above).

Go ahead and try to compile/build with above code snippet in your project.

Error should be something like:

```
/Library/Frameworks/Mono.framework/Versions/5.12.0/lib/mono/msbuild/15.0/bin/Roslyn/Microsoft.CSharp.Core.targets(5,5): 
Error MSB3105: 
The item "obj/Debug/generated/src/IO.Grpc.Stub.AbstractStub.cs" was specified more than once in the "Sources" parameter.  
Duplicate items are not supported by the "Sources" parameter. 
(MSB3105) (Sample.XamarinAndroid.Bindings.Library)
```

Workaround is to remove that snippet before build and add it after the build to re-enable intellisense
and code navigation in the IDE.

This solution is less than ideal, so next improvement was to add MSBuild task that injects that
code automagically into the project.
