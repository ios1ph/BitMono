Troubleshooting
==============

Common Issues and Solutions
--------------------------

Access Denied
~~~~~~~~~~~~

**Problem**: You get an "Access Denied" error when ObscuraX tries to open the output directory.

**Solution**: Set `OpenFileDestinationInFileExplorer` to `false` in `obfuscation.json`.

Missing Dependencies
~~~~~~~~~~~~~~~~~~~

**Problem**: ObscuraX can't find required dependencies.

**Solution**: 
- Make sure all required .dll files are in the ``libs`` directory
- Check that the dependency files exist and are accessible
- Try setting `FailOnNoRequiredDependency` to `false` in `obfuscation.json` if the dependencies are optional

Framework Mismatch
~~~~~~~~~~~~~~~~~~

**Problem**: You get compatibility warnings or errors.

**Solution**: Use the ObscuraX version that matches your target framework:

- .NET 8 applications → Use .NET 8 version of ObscuraX
- .NET Framework applications → Use .NET Framework version of ObscuraX
- Unity/Mono applications → Use .NET Framework version of ObscuraX

No Protections Enabled
~~~~~~~~~~~~~~~~~~~~~

**Problem**: ObscuraX says no protections are enabled.

**Solution**: 
- Enable at least one protection in `protections.json` by setting `Enabled` to `true`
- Or specify protections via command line: `-p ProtectionName1,ProtectionName2`

Permission Errors
~~~~~~~~~~~~~~~~

**Problem**: ObscuraX can't write to the output directory.

**Solution**: 
- Make sure ObscuraX has write permissions to the output directory
- Try running as administrator if needed
- Check that the output path is valid and accessible

Getting More Help
----------------

If you're still having issues:

- Check the `obfuscation.json` and `protections.json` configuration files
- Look at the console output for specific error messages
- Try running ObscuraX in interactive mode to see detailed prompts
- Visit the `GitHub Issues <https://github.com/sunnamed434/ObscuraX/issues>`_ page 