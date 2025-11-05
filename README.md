# MultiPublish

[![Tests](https://github.com/AdamTovatt/multipublish/actions/workflows/dotnet.yml/badge.svg)](https://github.com/AdamTovatt/multipublish/actions/workflows/dotnet.yml)
[![NuGet Version](https://img.shields.io/nuget/v/MultiPublish.svg)](https://www.nuget.org/packages/MultiPublish/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MultiPublish.svg)](https://www.nuget.org/packages/MultiPublish/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

Tired of running dotnet publish over and over again with different parameters to publish for all the different runtimes and options you want to support?

What if you could just use dotnet publish but with parameters that accepted array-like values. Well, now you can.

`MultiPublish` is a dotnet tool that extends `dotnet publish` to support multiple runtime identifiers and self-contained options in a single command, with optional zip packaging of outputs.

## What It Does

- Supports array syntax for `--runtime` and `--self-contained` parameters
- Automatically executes publish commands for all combinations
- Optionally creates zip files of publish outputs in `bin/MultiPublish/`
- Passes through all other arguments unchanged to `dotnet publish`

## Installation

```bash
dotnet tool install -g multipublish
```

## Usage

### Multiple Runtimes

Use array syntax to publish for multiple runtimes:

```bash
dotnet multipublish -c Release -r [win-x64, win-x86, linux-arm64]
```

### Multiple Runtimes and Self-Contained Options

Publish for multiple runtimes with both self-contained and framework-dependent outputs:

```bash
dotnet multipublish -c Release -r [win-x64, win-x86] --self-contained [true, false] /p:PublishSingleFile=true
```

This will create 4 publish outputs:
- `win-x64` self-contained
- `win-x64` framework-dependent
- `win-x86` self-contained
- `win-x86` framework-dependent

### Basic Usage

The goal of `multipublish` is to be able to serve as a complete replacement for `publish`. This means that if you want to, you can still use `multipublish` for scenarios where it isn't actually needed.

For example:

```bash
dotnet publish -c Release -r win-x64
```

Can be replaced with:

```bash
dotnet multipublish -c Release -r win-x64
```

### Disable Zip Creation

By default, zip files are created in `bin/MultiPublish/`. To disable:

```bash
dotnet multipublish -c Release -r [win-x64, win-x86] --no-zip
```

### Array Syntax

Arrays can be specified with or without spaces:

```bash
-r [win-x64, win-x86, linux-arm64]
--self-contained [true, false]
```

All other `dotnet publish` arguments are supported and passed through unchanged.

## Zip Output

When zip creation is enabled (default), zip files are created in `bin/MultiPublish/` with the naming pattern:

`{ProjectName}-{runtime}-{self-contained|framework-dependent}.zip`

For example:
- `MinerUHost-win-x64-self-contained.zip`
- `MinerUHost-win-x64-framework-dependent.zip`

## Requirements

- .NET 8.0 or later

