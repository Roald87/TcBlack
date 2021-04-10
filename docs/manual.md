# Manual

Below are all the different command line options available for _TcBlack_. In order to run these commands: open a windows command prompt and navigate to the folder where `TcBlack.exe` is located. Then enter one of the commands, or combine some options.

## Help

`--help`

Shows the different commands and options which can be used with _TcBlack_.

### Example
```
TcBlack --help
```

## Format files

`-f {filename1} {filename2} ...` or `--file {filename1} {filename2} ...` 

Select one or more `.TcPOU` or `.TcIO` files to format. Before any formatting is done, a `.bak` back-up copy of each file is generated. This is an extra safety measure in case unintended changes are made and/or the code is not under source control.

The options `--file` and `--project` are mutually exclusive, i.e. you can only use one of them!

Note: Currently _TcBlack_ only formats the declaration part of function blocks, functions, methods, properties and interfaces. Formatting of the implementation is planned for a future release.

### Examples

Format a single file with the short command option.

```
> TcBlack -f C:\Path\To\File1.TcPOU
```

Format multiple files with the long command option.

```
> TcBlack --file C:\Path\To\File1.TcPOU C:\Path\To\File1.TcIO
```

## Format a plc project

`-p {project}` or `--project {project}` 

Select a `.plcproj` file to format. _TcBlack_ tries to find all the `.TcPOU` and `.TcIO` files in the subdirectories of the `.plcproj` file. Then it will format all the found files. 

Before any formatting is done, a `.bak` back-up copy of each file is generated. This is an extra safety measure in case unintended changes are made and/or the code is not under source control.

The options `--file` and `--project` are mutually exclusive, i.e. you can only use one of them!

Note: Currently _TcBlack_ only formats the declaration part of function blocks, functions, methods, properties and interfaces. Formatting of the implementation is planned for a future release.

### Examples

Format a project with the short command option.

```
> TcBlack -p C:\Path\To\PlcProject.plcproj
```

Format multiple files with the long command option.

```
> TcBlack --project C:\Path\To\PlcProject.plcproj
```

You can also use a single `.TcPOU` or `.TcIO` file from a project as an argument. It will then find all the  `.TcPOU` and `.TcIO`. For example if `FB_FunctionBlock.TcPOU` is part of `PlcProject.plcproj` from the previous example, then the following command will have the same effect as the previous example.

```
> TcBlack -p C:\Path\To\Plc\FB_FunctionBlock.TcPOU
```

## Safe mode

`--safe`

This will build the `.plcproj` file before and after formatting, in order to check if unintended changes were made. Unintended changes are changes in the behavior of the code. Changes are detected by comparing the generated hash of the compilation before and after formatting. The hashes are the filenames of the `.COMPILEINFO` files in the `_CompileInfo` folder of a plc project. If any changes are detected, it will revert all the files to their previous state.

### Examples

Format a whole plc project in safe mode.

```
> TcBlack --safe -p C:\Path\To\PlcProject.plcproj
```

## Indentation

`--indentation {indentation}`

This option overrides the standard behavior for the indentation of _TcBlack_. By default it will look if there is a tab present for each inidvidual file which it is going to format. If a tab is found, this is used as indentation. If no tab is found, four spaces are used as the indentation. This option allows you to for example equalize the indentation type across a project.

### Example

Change the indentation to two spaces for a whole project.

```
> TcBlack --indentation "  " -p C:\Path\To\PlcProject.plcproj
```

## Line ending

`--windowslineending` or `--unixlineending`

This option overrides the standard behavior for the line ending of _TcBlack_. By default it will look if there is a Windows line ending (`\r\n`) present for each inidvidual file which it is going to format. If a `\r\n` is found, this is used as line ending. If no `\r\n` is found, the unix line ending `\n` is used. This option allows you to for example equalize the line ending type across a project.

Note: it will only change the line ending of the implementation part, not for the other code in the  `.TcPOU` or `.TcIO` files!

### Examples

Use a windows line ending for all the files in a plc project.

```
TcBlack --windowslineending --project C:\Path\To\PlcProject.plcproj
```

Use a unix line ending for all the given files.

```
TcBlack --unixlineending --files C:\Path\To\File1.TcPOU C:\Path\To\File1.TcIO
```

## Verbose

`--verbose`

Shows the commands which are used to build the project. Currently only has an effect when `--safe` option is used.

### Example

```
TcBlack --verbose --safe -f C:\Path\To\Plc\FB_FunctionBlock.TcPOU
```
## Version

`--version`

Shows the version of _TcBlack_.

### Example
```
TcBlack --version
```
