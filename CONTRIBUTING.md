# Contributing to dotnet-sops <!-- omit in toc -->

Welcome to the dotnet-sops repository on GitHub! We're excited that you're interested in contributing to our project. Your contributions, whether they're in the form of code, documentation, bug reports, or feature requests, play a crucial role in making this project better for everyone.

Please take a moment to review this Contributing Guide before you get started. It outlines the process for contributing to dotnet-sops and provides important information on how to submit your contributions.

## Table of Contents <!-- omit in toc -->

- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Development](#development)
- [Releases](#releases)
- [Contributing](#contributing)
  - [Submitting Issues](#submitting-issues)
  - [Creating Pull Requests](#creating-pull-requests)
- [Coding Guidelines](#coding-guidelines)
- [Documentation](#documentation)
- [License](#license)

## Getting Started

### Prerequisites

Make sure you have the following prerequisites installed before you start contributing:

- [.NET SDK 7.x](https://dotnet.microsoft.com/download) that are defined in [global.json](global.json)

### Development

#### Build

Code can be compiled by running the following command:

```bash
dotnet build
```

Warnings are treated as errors in Release configuration build.

#### Test

Test can be executed by running the following command:

```bash
dotnet test
```

#### Lint

Lint of code can be checked by the following command:

```bash
dotnet format --verify-no-changes
```

All rules are defined by [.editorconfig](.editorconfig) file

#### Package

Tool can be packed by the following command:

```bash
dotnet pack
```

## Releases

### Creating release

Releases are created by maintainers following this process:
1. Tag the branch with a semantic version number, e.g., "v0.1.0" or "v0.1.0-alpha.1".
2. Push the tag to GitHub. This action initiates a new build and generates a draft release on GitHub.
3. Verify the release and publish it. This initiates a workflow that uploads the release to NuGet.org.

## Contributing

### Submitting Issues

If you find a bug, have a question, or want to request a new feature, please [open an issue](https://github.com/MPBrun/dotnet-sops/issues) on GitHub. Provide a clear and detailed description of the issue, and use the appropriate issue template if available.

### Creating Pull Requests

We welcome pull requests (PRs) from contributors. To submit a PR:

1. Make sure your forked repository is up to date with the latest changes from the main repository.
2. Create a new branch for your feature/bug fix.
3. Make your changes and commit them with a clear and concise message.
4. Push your changes to your forked repository.
5. Open a pull request against the main branch of the main repository. Provide a detailed description of your changes and reference any related issues.
6. When all checks are green and all feedback have been addressed, we will merge the pull request with the "Squash and merge" option.

## Coding Guidelines

We follow the same coding guidelines used by [dotnet/aspnetcore](https://github.com/dotnet/aspnetcore/wiki/Engineering-guidelines#coding-guidelines). Some of the coding guidelines are enforced by the [.editorconfig](.editorconfig) file.

## Documentation

Good documentation is essential for the success of any project. Help us improve dotnet-sops's documentation by creating clear, concise, and easy-to-understand documentation.

## License

By contributing to dotnet-sops, you agree that your contributions will be licensed under the [MIT License](LICENSE.md). Make sure you understand and agree to the terms before submitting your contributions.
