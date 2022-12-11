![Official TcBlack logo](logo/TcBlack_logo_small.png)

# TcBlack: TwinCAT code formatter

Opinionated code formatter for TwinCAT. Currently in the **alpha state**. Use at your **own risk** and only with files which are under source control.

_TcBlack_ is available as a command line tool ([TcBlackCLI](https://github.com/Roald87/TcBlack/releases/tag/v0.2.0)) as well as a visual studio extension ([TcBlackExtension](https://github.com/Roald87/TcBlack/releases/tag/TcBlackExtension_v0.1.0)).

## Current state

`FB_Child` from ShowcaseProject formatted using the _TcBlackExtension_ for Visual Studio.

![tcblack_extension](tcblack_extension.gif)

## _TcBlackCLI_ usage

1. [Download](https://github.com/Roald87/TcBlack/releases/latest) the latest release.
1. Open the windows command prompt and navigate to the folder containing `TcBlack.exe`.
1. Reformat one or more file by giving their full path names:

    ```
    > TcBlack --safe --file C:\Full\Path\To\Filename.TcPOU C:\Full\Path\To\AnotherFilename.TcPOU
    ```

    or using the short version and format a single file:

    ```
    > TcBlack --safe -f C:\Full\Path\To\Filename.TcPOU
    ```

    or format a whole project at once and replace all indentation by a two spaces:

    ```
    > TcBlack --safe -f C:\Full\Path\To\Project.plcproj --indentation "  "
    ```

For more info enter `> TcBlack --help` in the command prompt or check the
[manual](docs/manual.md).

## Installing the extension

To install the VSIX extension in Visual Studio or TcXaeShell please see the [installation guide](docs/vsix_installation.md).

## Idea

Change

```
FUNCTION_BLOCK  FB_Child EXTENDS FB_Base  IMPLEMENTS I_Interface,I_Interface2

VAR_INPUT
END_VAR
VAR

SomeText: STRING;
	Counter		: DINT:= 1 ;
  Result		: DINT :=2;


      Base:FB_Base;
END_VAR
===================================
SomeText:= 'Current counts';

IF Conditions[1] AND Conditions[2]  AND Conditions[3] AND Conditions[4] AND Conditions[5]AND Conditions[6] THEN
	Counter :=Counter+ 1;

	IF Counter > 2 THEN
	Counter := Counter + 5 ;
	END_IF
END_IF

Base(Variable1:=2, Variable2:=3 , Variable3:= 5,Sentence:='Entropy is a real bitch.', Conditions :=Conditions);


AddTwoInts(    Variable1 :=4,
    Variable2:=4);
```

Into

```
FUNCTION_BLOCK FB_Child
EXTENDS FB_Base
IMPLEMENTS I_Interface, I_Interface2
VAR
    SomeText : STRING;
    Counter : DINT := 1;
    Result : DINT := 2;

    Base : FB_Base;
END_VAR

===================================
SomeText := 'Current counts';

IF
    Conditions[1]
    AND Conditions[2]
    AND Conditions[3]
    AND Conditions[4]
    AND Conditions[5]
    AND Conditions[6]
THEN
    Counter := Counter + 1;

    IF Counter > 2 THEN
        Counter := Counter + 5 ;
    END_IF
END_IF

Base(
    Variable1:=2,
    Variable2:=3 ,
    Variable3:=5,
    Sentence:='Entropy is a real bitch.',
    Conditions:=Conditions
);

AddTwoInts(Variable1:=4, Variable2:=4);

```

## Why

Get a consistent style across your project, without having to go through all the code. Focus on the logic and structure of the code, not the formatting.

## How

By making a command line tool which can be either used manually on individual files, a whole project or added as a pre-hook commit which automatically reformats before making a commit.

## Style

Follow the same style rules as [Black](https://github.com/psf/black/) for Python (where applicable). Why try to reinvent the wheel, when Black offers a popular rule base which has been tested and tried? For more info see the [style guide](docs/style.md).

## _TcBlackCLI_ implementation

There are two modes for the _TcBlackCLI_. A safe mode which checks if the code did not undergo unwanted changes after reformatting. The non-safe mode is faster, but it could be that there were unwanted changes to the code. _TcBlackExtension_ always operates in the non-safe mode.

The safe mode builds the project before and after formatting. It then compares the generated number (a sort of checksum?) which is used as the name of the `*.compileinfo` file. This file is generated in the _\_CompileInfo_ folder of a project each time it is build.

The number doesn't change when you alter whitespaces, add/change comments or add brackets around a long if statement. Only if the actual code changes then the number also changes. For example, if you add a variable, add a line of code or change the order of variables.

## Contributing

You're more then welcome to help if you'd like! See the [contributing guidelines](CONTRIBUTING.md) for more info.
