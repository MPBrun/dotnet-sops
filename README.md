# dotnet-sops: SOPS for dotnet
Store and share [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) secure. Uses [SOPS](https://github.com/mozilla/sops) to encrypt and decrypt secrets.

"No more plain secrets in Git"

"Share development secrets securely with all team members"

Supports all encryption types supported by SOPS: AWS KMS, GCP KMS, Azure Key Vault, Hashicorp Vault, age and PGP

# Warning
When secrets are decrypted they are stored in plain, unencrypted text, that can be loaded by [user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows#secret-manager) tool.
Only store development secrets that cannot access production like environment.

# Install
Run the following commands to install tool:
```
dotnet tool install dotnet-sops
```

# Usage

## Init dotnet-sops
Initialize dotnet-sops
```
dotnet sops init
```

Creates .sops.yaml file

## Download SOPS
Download SOPS from https://github.com/getsops/sops
```
dotnet sops download-sops
```

The executable will be used hereafter. The SOPS executable is file SHA-512 checksum checked.

## Encrypt
Encrypt existing dotnet user secrets.
```
dotnet sops encrypt --id <user-secret-id>
```

## Decrypt
Decrypt secrets into dotnet user secrets.
```
dotnet sops decrypt --id <user-secret-id>
```

## Help
Show help and usage information
```
dotnet sops --help
```

# Configure SOPS
Prepare .sops.yaml for encrypting secrets.json. The content should define a `creation_rules` for the path `secrets.json`.

See [SOPS documentation](https://github.com/getsops/sops#using-sops-yaml-conf-to-select-kms-pgp-for-new-files) for more information.

## Using age
Usage with [age](https://github.com/FiloSottile/age) key
``` yaml
creation_rules:
  - path_regex: secrets.json
    age: age1
```

Only decryption is possible for user with the private key. If more should have access, then mulitple public age keys can be specified:
``` yaml
creation_rules:
  - path_regex: secrets.json
    age: >-
      age1,
      age2
```

# Configure SOPS with key groups
SOPS can also be configured to requred access to more than one key. This can be configured by using key groups in the configuration.
Example using 3 key groups:
creation_rules:
  - path_regex: secrets.json
    key_groups:
      - pgp:
          - fingerprint1
        age:
          - age1
      - pgp:
          - fingerprint2
      - age:
          - age2
```

This requires access to fingerprint1 or age1. Require access to fingerprint2 and age2.

# Example usage

## Init user secrets
Initialize dotnet user-secrets using the following command:
```
dotnet user-secrets init
```

This will add a `UserSecretsId` element with an generated id to the project file.

For more information on see [Safe storage of app secrets in development in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows)

## Set secret
To set a secret execute the following command:
```
dotnet user-secrets set --id <user-secret-id> [key] [value]
```

encrypt

decrypt

rotation? - decrypt + encrypt

# Development
## Build
```
dotnet build
```

## Lint
```
dotnet format --verify-no-changes
```

## Test
```
dotnet test
```

## Package
```
dotnet pack
```
