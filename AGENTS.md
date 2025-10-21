# Repository Guidelines

## Mission & Scope
This fork exists only to reinforce the private WPF product in `C:\Users\ios1ph\source\repos\BoosterX_2.0` (target framework: .NET 4.8). Extend ObscuraX with protections that Eazfuscator.NET and Costura/Fody do not supply while keeping the stack silent about ObscuraX. Strip any telemetry, banners, or update checks that could reveal tooling and keep every artifact private.

## Project Structure & Touchpoints
`ObscuraX.sln` remains the entry point. Core orchestration lives in `src/ObscuraX.Core`, with transformation logic split across `src/ObscuraX.Obfuscation`, `src/ObscuraX.Protections`, and runtime helpers in `src/ObscuraX.Runtime`. Place BoosterX-specific adapters under `src/ObscuraX.BoosterX` (create the project if it does not exist) and keep common utilities in `ObscuraX.Shared`. Tests should mirror namespaces inside `test/` and integration harnesses that consume BoosterX binaries belong in `test/Sandbox/BoosterX` pointing to `..\..\..\BoosterX_2.0\BoosterX_2.0\bin\Release`.

## Build & Validation Commands
Run `dotnet restore ObscuraX.sln` once per session, then `dotnet build ObscuraX.sln -c Release` (use `-c Debug` during iteration). Execute protection suites with `dotnet test ObscuraX.sln --filter FullyQualifiedName~Protections`. Exercise the CLI through `dotnet run --project src/ObscuraX.CLI/ObscuraX.CLI.csproj -- --config <config>` to validate presets. To run the complete pipeline, build BoosterX with `msbuild BoosterX_2.0.sln /p:Configuration=Release`, then feed the resulting executable to our ObscuraX preset stored under `resources/presets/boosterx.yml`.

## Integration with Eazfuscator & Costura
`BoosterX_2.0/ObfuscationSettings.cs` already enables resource encryption, symbol encryption, control flow, and sanitization in Eazfuscator. ObscuraX must fill the remaining gaps: anti-debug and anti-dump (AntiDebugBreakpoints, DotNetHook), aggressive IL distortion (BitDotNet, BitMethodDotnet, BillionNops), namespace removal, stealth metadata tweaks, and runtime integrity guards. Always execute ObscuraX after Eazfuscator so the Costura bootstrapper stays intact, and confirm embedded assemblies still resolve before and after our pass.

## Coding Style & Naming
Follow `.editorconfig` and `CONTRIBUTING.md`: four-space indentation, braces on new lines, and expressive PascalCase members. Interfaces stay `IPrefixedPascalCase`, private fields use `_camelCaseWithUnderscore`, and locals stay camelCase. Prefer partial classes to manage large protection implementations and avoid undocumented abbreviations.

## Testing & Verification
Every new protection or pipeline hook needs xUnit coverage stored under `test/ObscuraX.*.Tests`. Create dedicated fixture assemblies in `test/TestBinaries/BoosterX` instead of copying proprietary BoosterX sources. Document manual verification (launch success, performance metrics, crash checks) in `docs/private/boosterx-validation.md` so regressions can be replayed.

## Security & Operational Practices
Treat generated binaries and logs as confidential. Remove or hide ObscuraX branding in CLI output and gate verbose diagnostics behind an opt-in environment variable. Never push this fork or related packages to public feeds; audit dependencies before updating and track active engineering work in `TASK.md`.
