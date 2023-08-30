# dotnet-sops: A .NET tool for encrypting .NET user-secrets using SOPS

[![Nuget](https://img.shields.io/nuget/v/dotnet-sops?logo=nuget)](https://www.nuget.org/packages/dotnet-sops/) [![GitHub](https://img.shields.io/github/license/MPBrun/dotnet-sops)](https://github.com/MPBrun/dotnet-sops/blob/main/LICENSE.md) [![codecov](https://img.shields.io/codecov/c/github/MPBrun/dotnet-sops?token=D8YV1LQ4YE&logo=codecov)](https://codecov.io/gh/MPBrun/dotnet-sops)

`dotnet-sops` is a .NET tool for securely storing and sharing [user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) with all your team members.

It utilizes [SOPS](https://github.com/getsops/sops) under the hood to encrypt and decrypt secrets, thereby supporting all key types supported by SOPS: Azure Key Vault, AWS KMS, GCP KMS, HashiCorp Vault, age, and PGP.

> When secrets are decrypted using `dotnet sops decrypt` they are stored in plain, unencrypted text that can be loaded by the [user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows#secret-manager) tool.
>
> Remove plaintext by running `dotnet user-secrets clear` and use `dotnet sops run` to load secrets into the environment.

## Table of Contents <!-- omit in toc -->

- [Background](#background)
- [Installation](#installation)
- [Get started](#get-started)
- [Usage](#usage)
- [Configure SOPS](#configure-sops)
- [Contributing](#contributing)

## Background

`dotnet-sops` was developed to address the security risks posed by plain-text secrets in Git repositories. It offers a streamlined solution, integrating with SOPS encryption, to enable secure management and sharing of development secrets within the .NET ecosystem.

## Installation

### Local (Recommended)

Run the following commands to install the tool:

```bash
dotnet new tool-manifest
dotnet tool install dotnet-sops
```

This will write the `.config/dotnet-tools.json` file with version of the tool used. Add this file to source control. Your team members can then run the follwing command to install the same version of the tool:

```bash
dotnet tool restore
```

### Global

Run the following commands to install the tool globally:

```bash
dotnet tool install dotnet-sops --global
```

## Get started

### Basic usage

```bash
# Follow the text wizard to create .sops.yaml configuration file. (Only the first time)
dotnet sops init

# Download SOPS executable. (Only the first time)
dotnet sops download-sops

# Generate UserSecretId for dotnet project. Skip if already have it setup. (Only the first time)
dotnet user-secrets init

# Add an example secret to user-secrets. (Only for new secrets)
dotnet user-secrets set "MyApi:ApiKey" "MySuperSecretApiKeyValue"

# Encrypt user-secrets to the secrets.json file.
dotnet sops encrypt

# Clear secrets from user-secrets.
dotnet user-secrets clear

# Run 'dotnet run' with decrypted secrets injected into the environment.
dotnet sops run

# Decrypt secrets.json to user-secrets.
dotnet sops decrypt
```

For more information on how to use user-secrets to add secrets, refer to [Safe storage of app secrets in development in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows)

## Usage

### init

Initiate the prompt to create a valid `.sops.yaml` configuration file:

```bash
dotnet sops init
```

### download-sops

If you don't have SOPS in your PATH, you can download SOPS from https://github.com/getsops/sops using the follwing command:

```bash
dotnet sops download-sops
```

The downloaded SOPS executable has its SHA-512 checksum verified. This version of SOPS will be used by the `encrypt` and `decrypt` commands.

### encrypt

Encrypt existing .NET user secrets.

```bash
dotnet sops encrypt
```

This will create a `secrets.json` file that can be shared with other team members working on your project.

### decrypt

Decrypt a `secrets.json` file into .NET user secrets.

```bash
dotnet sops decrypt
```

### run

Run `dotnet run` with secrets from `secrets.json` injected into the environment.

```bash
dotnet sops run
```

`dotnet sops run` accepts the same arguments and options as `dotnet run`.

### --help

All commands support the `--help` option:

```bash
# List all commands
dotnet sops --help

# List all options for encryption
dotnet sops encrypt --help
```

## Configure SOPS

The `.sops.yaml` configuration file is used to inform SOPS how to handle encryption and decryption. The content should define a `creation_rules` for the `secrets.json` path.

The following command can be used to help create a vaild `.sops.yaml`:

```bash
dotnet sops init
```

You can also create and edit the `.sops.yaml` file yourself. Refer to the [SOPS documentation](https://github.com/getsops/sops#using-sops-yaml-conf-to-select-kms-pgp-for-new-files) for more information.

### Using age

Usage with [age](https://github.com/FiloSottile/age) key:

```yaml
creation_rules:
  - path_regex: secrets.json
    age: age1
```

Only decryption is possible for user with the private key.

Refer to the [SOPS documentation](https://github.com/getsops/sops#encrypting-using-age) to learn how SOPS accesses age private keys.

If more people should have access, then mulitple public age keys can be specified:

```yaml
creation_rules:
  - path_regex: secrets.json
    age: age1,age2
```

This requires user access to the age1 or age2 private key.

### Using Azure Key Vault

Usage with [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault) key:

```yaml
creation_rules:
  - path_regex: secrets.json
    azure_keyvault: https://my-key-vault-name.vault.azure.net/keys/my-key-name/my-key-version
```

Users should be assigned the `encrypt` and `decrypt` rights for the key vault.

### Configure SOPS with key groups

SOPS can also be configured to require access to more than one key. This can be achieved by using key groups in the configuration. Refer to the [SOPS key group dokumentation](https://github.com/getsops/sops#key-groups) for more information.

Example using three key groups:

```yaml
creation_rules:
  - path_regex: secrets.json
    key_groups:
      - pgp: # Key group 1
          - fingerprint1
        age:
          - age1
      - pgp: # Key group 2
          - fingerprint2
      - age: # Key group 3
          - age2
```

This requires that the user has access to all three key groups. For key group 1, it requires the user to have access to either fingerprint1 or age1.

## Contributing

Refer to [the contributing guide](CONTRIBUTING.md) for detailed instructions on how to contribute to dotnet-sops.
