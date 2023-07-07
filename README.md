# dotSOPS: SOPS for dotnet
Store and share [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) secure. Uses [SOPS](https://github.com/mozilla/sops) to encrypt and decrypt secrets.

"No more plain secrets in Git"

"Share secrets securely with all team members"

Supports all keys supported by SOPS: AWS KMS, GCP KMS, Azure Key Vault, age and PGP


# Install
Run the following commands to install tool:
```
dotnet tool install dotsops
```

# Usage

## Encrypt
Encrypt existing dotnet user secrets.
```
dotnet dotsops encrypt --id <user-secret-id>
```

## Decrypt
Decrypt secrets into dotnet user secrets.
```
dotnet dotsops decrypt --id <user-secret-id>
```

## Download SOPS
Download SOPS from https://github.com/mozilla/sops
```
dotnet dotsops download-sops
```

The SOPS executable is file SHA-512 checksum checked.

## Help
Show help and usage information
```
dotnet dotsops --help
```

# Configure SOPS
.sops.yaml

# Example usage

dotnet add

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
