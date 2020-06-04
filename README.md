# TcBlack: TwinCAT code formatter
Opnionated code formatter for TwinCAT.

## Status
There is an idea, but it doesn't work yet. When [Milestone 0.1](https://github.com/Roald87/TcBlack/milestone/1) 
is completed there should be a first working version. 

You're more then welcome to help if you'd like! See the [contributing guidelines](https://github.com/Roald87/TcBlack/blob/master/CONTRIBUTING.md)
for more info.

## Idea

Change

```
FUNCTION_BLOCK   DoSomething
VAR
	Condition : ARRAY[1..5] OF BOOL;
SomeText: STRING;
	Counter		: DINT:= 1 ;
	Result		: DINT :=2;
END_VAR
===================================
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
FUNCTION_BLOCK DoSomething
VAR
	Condition : ARRAY[1..5] OF BOOL;
	SomeText : STRING;
	Counter : DINT := 1;
	Result : DINT := 2;
END_VAR
===================================
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