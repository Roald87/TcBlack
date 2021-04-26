# Contributing to TcBlack
We love your input! We want to make contributing to this project as easy and 
transparent as possible, whether it's:

- Reporting a bug
- Discussing the current state of the code
- Submitting a fix
- Proposing new features
- Becoming a maintainer

You can also work on an open issue. Issues which are not assigned to anyone, are up for 
grabs.

## We Develop with Github
We use GitHub to host code, to track issues and feature requests, as well as accept 
pull requests.

## We Use [Github Flow](https://guides.github.com/introduction/flow/index.html), So All Code Changes Happen Through Pull Requests
Pull requests are the best way to propose changes to the codebase (we use 
[Github Flow](https://guides.github.com/introduction/flow/index.html)). We actively 
welcome your pull requests:

1. Fork the repo and create your branch from `master`.
2. Make the changes in your forked branch.
2. If you've added new functionality, it's necessary to add tests. For this there is a 
separate test project called [TcBlackTests](https://github.com/Roald87/TcBlack/tree/master/src/TcBlackTests). 
In that project there is a seperate test suite defined for each class in TcBlack.
**No new functionality will be accepted without any proper tests**.
3. Ensure the test suite passes.
4. Issue that pull request!

## Any contributions you make will be under the MIT Software License
In short, when you submit code changes, your submissions are understood to be under the 
same [MIT License](http://choosealicense.com/licenses/mit/) that covers the project. 
Feel free to contact the maintainers if that's a concern.

## Report bugs/ideas using Github's [issues](https://github.com/Roald87/TcBlack/issues)
We use GitHub issues to track public bugs and collect ideas. Report a bug or suggest an
idea by [opening a new issue](https://github.com/Roald87/TcBlack/issues/new); 
it's that easy!

## Write bug reports and ideas with detail, background, and sample code

**Great bug reports/ideas** tend to have:

- A quick summary and/or background.
- Steps to reproduce:
  - Be specific!
  - Give sample code if you can.
- What you expected would happen.
- What actually happens.
- Notes (possibly including why you think this might be happening, or stuff you tried 
that didn't work).

## Build environment
* Make sure to edit the project with the same version of Visual Studio as the master 
branch. TcBlack has been developed using Visual Studio 2017. You can download
[VS2017 Community edition](https://visualstudio.microsoft.com/vs/older-downloads/) for 
free.
* *If using Visual Studio 2019 make sure to update the `Microsoft.VSSDK.BuildTools` nuget
package in the `TcBlackExtension` project to version `15.9.3039`(unfortunaly `15.9.3043`is
not available on nuget.org)*								   				 
* Add your own `pfx` file in the `TcBlackCore` project. This is neccessary to sign the
assembly. This can be done by navigating to the `TcBlackCore` project settings, go to 
`signing` and add a new keyfile with the name `TcBlackCoreSign.pfx`. Make sure to enter 
a password.  See also [here](https://github.com/Roald87/TcBlack/issues/57#issuecomment-814382341) 
* Although most of the development will take place in C#, it is good to use the same 
TwinCAT version if you're making changes to the TwinCAT projects. TcBlack currently 
uses **TwinCAT 4024.7**.

## Use a Consistent Coding Style
* In order to keep the code readable, a maximum line length of 88 characters is used.
This is the same in [Black](https://github.com/psf/black) for Python. You can add
a [Guideline](https://marketplace.visualstudio.com/items?itemName=PaulHarrington.EditorGuidelines)
to the Visual Studio to help you here.

Furthermore use the following TwinCAT editor settings:

* Make sure that your TwinCAT development environment uses spaces instead of tabs. The 
default behaviour of the TwinCAT development environment is to use tabs so it needs to 
be changed. The option can be found under **Tools → Options → TwinCAT → PLC Environment
 → Text editor**. Here you want to de-select **Keep tabs**. See also 
[this guide](https://alltwincat.com/2017/04/14/replace-tabs-with-whitespaces/).
* Make sure to set your TwinCAT development environment to use Separate LineIDs. This 
is available in the TwinCAT XAE under 
**Tools → Options → TwinCAT → Write options → Separate LineIDs** (set this to TRUE, 
more information is available [here](https://infosys.beckhoff.com/english.php?content=../content/1033/tc3_userinterface/18014403202147467.html&id=)).

## Testing 
If you've implemented a new feature, you can try it by formatting or adding a file to 
the ShowcaseProject. Please only commit pre-formatted versions of these files, 
otherwise others can't use it. To show the new feature you can update the README's 
Current state section.

## License
By contributing, you agree that your contributions will be licensed under its MIT License.

## References
This document was adapted from TcUnits's excellent [contribution guidelines](https://github.com/tcunit/TcUnit/blob/master/CONTRIBUTING.md).
