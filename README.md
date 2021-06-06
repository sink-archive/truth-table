# Truth Table
[![`GPL-3.0-or-later`](https://img.shields.io/badge/license-GPL--3.0--or--later-blue)](https://github.com/yellowsink/truth-table/blob/master/LICENSE.md)

Automagically generate truth tables for any function in .NET.

Includes a CLI tool which takes either lambda functions `a, b, c -> a | b & c` or simpler logic notation `a | b & c`, and creates a truth table.

It supports any datatypes, including custom classes / structs (the CLI tool assumes `bool`s, however).
