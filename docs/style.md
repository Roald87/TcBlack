# The _TcBlack_ code style

_This document is an edited version of _Black_'s 
[code style document](https://raw.githubusercontent.com/psf/black/master/docs/the_TcBlack_code_style.md)._

## Code style

_TcBlack_ reformats entire files in place and creates a `.bak` file of the original one. 
It is not configurable. It doesn't take previous formatting into account. It doesn't reformat 
blocks that start with `// fmt: off` and end with `// fmt: on` [#16](https://github.com/Roald87/TcBlack/issues/16). 

### How _TcBlack_ wraps lines (Milestone 0.5+)

_TcBlack_ ignores previous formatting and applies uniform horizontal and vertical
whitespace to your code. The rules for horizontal whitespace can be summarized as: do
whatever makes `pycodestyle` happy. The coding style used by _TcBlack_ can be viewed as a
strict subset of PEP 8/[Black](https://raw.githubusercontent.com/psf/black/master/docs/the_TcBlack_code_style.md). 
Furthermore, formatting is done in such a way to reduce potential diffs.

As for vertical whitespace, _TcBlack_ tries to render one full expression or simple
statement per line. If this fits the allotted line length, great.

```
// in:

j = DoSomething(var1:=1,
     var2:=3,
     var3=5
);

// out:

j = DoSomething(var1:=1, var2:=3, var3=5);
```

If not, _TcBlack_ will look at the contents of the first outer matching brackets and put
that in a separate indented line.

```
// in:

SomeImportantFunctionBlock.WithAMethod(And:='a', few:='other', variables:='to', worry:='about');

// out:

SomeImportantFunctionBlock.WithAMethod(
	And:='a', few:='other', variables:='to', worry:='about'
);
```

If that still doesn't fit the bill, it will decompose the internal expression further
using the same rule, indenting matching brackets every time. If the contents of the
matching brackets pair are comma-separated like an argument list then _TcBlack_ will 
first try to keep them on the same line with the matching brackets. If that doesn't work, 
it will put all of them in separate lines.

```
// in:

AnotherFunctionBlock(without:='a', method:=', ', which:='has', quite:='a', bit:='more', variables:='to', worry:='about');

// out:

AnotherFunctionBlock(
	without:='a', 
	method:=', ', 
	which:='has', 
	quite:='a', 
	bit:='more', 
	variables:='to', 
	worry:='about',
);
```

You might have noticed that closing brackets are always dedented and that a trailing
comma is always added ([#18](https://github.com/Roald87/TcBlack/issues/18)). Such formatting produces smaller diffs; when you add or remove an
element, it's always just one line. Also, having the closing bracket dedented provides a
clear delimiter between two distinct sections of the code that otherwise share the same
indentation level (like the arguments list in the example above).

### Line length (Milestone 0.5+)

You probably noticed the peculiar default line length. _TcBlack_ defaults to 88 characters
per line, which happens to be 10% over Python's popular 80. This number was found to produce
significantly shorter files than sticking with 80 (the most popular), or even 79 (used
by the standard library). In general,
[90-ish seems like the wise choice](https://youtu.be/wf-BqAjZb8M?t=260).

If you're paid by the line of code you write, you can pass `--line-length` 
[#17](https://github.com/Roald87/TcBlack/issues/17) with a lower
number. _TcBlack_ will try to respect that. However, sometimes it won't be able to without
breaking other rules. In those rare cases, auto-formatted code will exceed your allotted
limit.

You can also increase it, but remember that people with sight disabilities find it
harder to work with line lengths exceeding 100 characters. It also adversely affects
side-by-side diff review on typical screen resolutions. Long lines also make it harder
to present code neatly in documentation or talk slides.

### Empty lines

_TcBlack_ avoids spurious vertical whitespace. This is in the spirit of PEP 8 which says
that in-function vertical whitespace should only be used sparingly.

_TcBlack_ will allow single empty lines inside functions, except when they're within
parenthesized expressions. Since such expressions are always reformatted to fit minimal
space, this whitespace is lost.

Finally _TcBlack_ will add a single whiteline to a function or function block definition and
implementation. This is to prevent unessesarry diffs in the raw TcPOU xml file, especially in the 
implementation part. Since the declaration part will usually end with a `END_VAR` anyway, but
for consistency the declaration part will also get a trailing white space.

Take the following diffs where an extra statement gets added, but this version doesn't have a 
trailing white line:

```diff
<?xml version="1.0" encoding="utf-8"?>
<TcPlcObject Version="1.1.0.1" ProductVersion="3.1.4024.3">
  <POU Name="FB_Child" Id="{0e65befd-f733-4035-9cb9-b4c563001612}" SpecialFunc="None">
...
    <Implementation>
      <ST><![CDATA[SomeText := 'Current counts';
...
-Counts := Counts + 1;]]></ST>
+Counts := Counts + 1;
+ExtraCounts := ExtraCounts + 1;]]></ST>
    </Implementation>
  </POU>
</TcPlcObject>
```

and this one does.

```diff
<?xml version="1.0" encoding="utf-8"?>
<TcPlcObject Version="1.1.0.1" ProductVersion="3.1.4024.3">
  <POU Name="FB_Child" Id="{0e65befd-f733-4035-9cb9-b4c563001612}" SpecialFunc="None">
...
    <Implementation>
      <ST><![CDATA[SomeText := 'Current counts';
...
Counts := Counts + 1;
+ExtraCounts := ExtraCounts + 1;
]]></ST>
    </Implementation>
  </POU>
</TcPlcObject>
```

### Trailing commas (Milestone 0.5+, [#18](https://github.com/Roald87/TcBlack/issues/18))

_TcBlack_ will add trailing commas to expressions that are split by comma where each
element is on its own line.

Unnecessary trailing commas are removed if an expression fits in one line. This makes it
1% more likely that your line won't exceed the allotted line length limit. Moreover, in
this scenario, if you added another argument to your call, you'd probably fit it in the
same line anyway. That doesn't make diffs any larger.

### Line breaks & binary operators (Milestone 0.5+)

_TcBlack_ will break a line before a binary operator when splitting a block of code over
multiple lines. This is so that _TcBlack_ is compliant with the recent changes in the
[PEP 8](https://www.python.org/dev/peps/pep-0008/#should-a-line-break-before-or-after-a-binary-operator)
style guide, which emphasizes that this approach improves readability.

So instead of 

```
// Wrong:
// operators sit far away from their operands
income := (
    gross_wages +
    taxable_interest +
    (dividends - qualified_dividends) -
    ira_deduction -
    student_loan_interest
);
```

do 

```
// Correct:
// easy to match operators with operands
income := (
    gross_wages
    + taxable_interest
    + (dividends - qualified_dividends)
    - ira_deduction
    - student_loan_interest
);		  
```

### Parentheses (Milestone 0.5+)

Some parentheses are optional in the IEC structured text grammar. Any expression can be wrapped in a
pair of parentheses to form an atom. There are a few interesting cases:

- `IF (...) THEN`
- `WHILE (...) DO`
- `FOR (...) TO (...) DO`

In those cases, parentheses are removed when the entire statement fits in one line, or
if the inner expression doesn't have any delimiters to further split on. If there is
only a single delimiter and the expression starts or ends with a bracket, the
parenthesis can also be successfully omitted since the existing bracket pair will
organize the expression neatly anyway. Otherwise, the parentheses are added.

Please note that _TcBlack_ does not add or remove any additional nested parentheses that
you might want to have for clarity or further code organization. For example those
parentheses are not going to be removed:

```
MethodName := NOT (this OR that);
decision := (maybe.this() AND values > 0) OR (maybe.that() AND values < 0);
```

### Call chains (Milestone 0.5+)

Some APIs use call chaining. This API style is known as a
[fluent interface](https://en.wikipedia.org/wiki/Fluent_interface). _TcBlack_ formats
those by treating dots that follow a call or an indexing operation like a very low
priority delimiter. It's easier to show the behavior than to explain it. Look at the
example:

```
result := (
    session.query(models.Customer.id)
    .filter(
        models.Customer.account_id = account_id,
        models.Customer.email = email_address,
    )
    .order_by(models.Customer.id.asc())
    .all()
);
```

### The magic trailing comma (Milestone 0.5+)

_TcBlack_ in general does not take existing formatting into account.

However, there are cases where you put a short collection or function call in your code
but you anticipate it will grow in the future.

For example:

```
Thermocouples : ARRAY[1..2] OF FB_Tc = [
	(Pos:='4 Left'), 
	(Pos:='5 Below'),
];
```

In principle this statement fits on a single 88 character line. However, you can communicate that 
you don't want to format it, by putting a trailing comma in the collection yourself. When 
you do, _TcBlack_ will know to always explode your collection into one item per line.

How do you make it stop? Just delete that trailing comma and _TcBlack_ will collapse your
collection into one line if it fits.


