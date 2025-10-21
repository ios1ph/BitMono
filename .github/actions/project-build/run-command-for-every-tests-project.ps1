#
# Finds all tests projects matching 'ObscuraX.*.Tests' for a project passed as the first argument
# and runs a command passed as the second argument for every tests project found.
# Also sets PROJECT_PATH environment variables with the value of the tests project folder path.
# Exits with a non-zero status if any command fails.
#
# Example usage:
# pwsh -f .github/actions/project-build/run-command-for-every-tests-project.ps1 "src/ObscuraX.Core" "echo \$PROJECT_PATH"
#
# Example output:
# Tests project found: /home/runner/work/ObscuraX/ObscuraX/tests/ObscuraX.Core.Tests. Executing a command: ...
# Assumes the first argument is already the path to the tests directory.

$testsFolderPath = $args[0]
$commandToExecute = $args[1]
$global:exitCode = 0

Get-ChildItem -Path $testsFolderPath -Directory -Recurse `
| Where-Object { $_.Name -match "^ObscuraX\..*\.Tests$" } `
| ForEach-Object {
    $testsProjectPath = $_.FullName
    Write-Output "Tests project found: $testsProjectPath. Executing a command: $commandToExecute"
    bash -c "PROJECT_PATH=$testsProjectPath && $commandToExecute"
    if ($LASTEXITCODE -ne 0) {
        $global:exitCode = $LASTEXITCODE
    }
}

if ($global:exitCode -ne 0) {
    exit $global:exitCode
}