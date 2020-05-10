# TcBlack: TwinCAT code formatter
Opnionated code formatter for TwinCAT.

## Idea

Change

```
VAR
	Condition : ARRAY[1..5] OF BOOL;
SomeText: STRING;
	Counter		: DINT:= 1 ;
	Result		: DINT :=2;
END_VAR

SomeText:= 'Current counts';

IF Condition[1] AND Condition[2]  AND CONDITION[3] AND CONDITION[4] AND CONDITION[5] THEN
	Counter :=Counter+ 1;

	IF Counter > 2 THEN
	Counter := Counter + 5 ;
	END_IF
END_IF

Method(Variable1:=2, Variable2:=3 , Variable3:= 5, Condition :=Counter, Output=>Result);


AddTwo(First:= Counter, 
	Second := Result);

```

Into

```
VAR
	Condition : ARRAY[1..5] OF BOOL;
	SomeText : STRING;
	Counter : DINT := 1;
	Result : DINT := 2;
END_VAR

SomeText := 'Current counts';

IF (
	Condition[1] 
	AND Condition[2] 
	AND CONDITION[3] 
	AND CONDITION[4] 
	AND CONDITION[5]
) THEN
	Counter := Counter + 1;

	IF Counter > 2 THEN
		Counter := Counter + 5;
	END_IF
END_IF

Method(
	Variable1:=2, 
	Variable2:=3, 
	Variable3:=5, 
	Condition:=Counter, 
	Output=>Result
);

AddTwo(First:=Counter, Second:=Result);

```

## Why

Get a consistent style across your project, without having to go through all the
code. Focus on the logic and structure of the code, not the formatting.

## How

By making a command line tool which can be either used manually on 
individual files, a whole project or added as a pre-hook commit which
automatically reformats before making a commit.

## Style

Follow the same style rules as [Black](https://github.com/psf/black/) 
for Python (where applicable). Why try to reinvent the wheel, when Black 
offers a popular rule base which has been tested and tried?

## Implementation

There are two modes. A safe mode which checks if the code did not undergo
unwanted changes after reformatting. The non-safe mode is faster, but it 
could be that there were unwanted changes to the code.

The safe mode builds the project before and after formatting. It then compares
the generated number (a sort of checksum?) which is used as 
the name of the `*.compileinfo` file. This file is generated in the 
_\_CompileInfo_ folder of a project each time it is build.

The number doesn't change when you alter whitespaces, add/change comments
or add brackets around a long if statement. Only if the actual code changes
then the number also changes. For example, if you add a variable, add a line 
of code or change the order of variables.

The project will be build using the [Twincat API](https://infosys.beckhoff.com/content/1033/tc3_automationinterface/63050395025936267.html?id=4055881424125395371)
and [Visual Studio DevEnv](https://infosys.beckhoff.com/content/1033/tc3_automationinterface/1520210443.html?id=2562914764838059793).
