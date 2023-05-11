# hpl2-parser

A parser for HPL2's scripting language

## Prototype Goal

The goal is to create a parser that will be able to validate a script file for Amnesia: The Dark Descent. It should catch the following problems:

- Calling a function that is not declared either in a script, or in Amnesia's API
- Common Syntax problems such as missing parenthesis or semicolons


## Progress

*Recognize in this context means the generated Abstract Syntax Tree (AST) contains the correct values and types

- [x] Recognize a function declaration
- [ ] Recognize a function call (1/?)
    - [x] Recognize a parameterless function call
    - [ ] Recognize a function call with parameters that are all literal values
- [ ] Recognize variable declaration
- [ ] Recognize a function call (2/?)
    - [ ] Recognize a function call with literal AND variable parameters
    - [ ] Recognize a function call with literal, variable, AND function call parameters
- [ ] ... plan the next set of goals ...
