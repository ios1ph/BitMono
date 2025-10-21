NuGet Configuration
==================

This guide explains how to configure NuGet when using ObscuraX as a NuGet package dependency.

When Configuration is Needed
----------------------------

You need to configure NuGet if you encounter dependency resolution errors when trying to use ObscuraX packages. This happens when ObscuraX may use nightly versions of AsmResolver (which we may use when needed for critical fixes) that are only available in a custom feed, not on the default nuget.org.

Configuration Steps
-------------------

1. **Create NuGet.config in your project root:**

.. code-block:: xml

   <?xml version="1.0" encoding="utf-8"?>
   <configuration>
     <packageSources>
       <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
       <add key="asmresolver-nightly" value="https://nuget.washi.dev/v3/index.json" />
     </packageSources>
   </configuration>

2. **Restore packages:**

.. code-block:: bash

   dotnet restore

That's it! Your project should now be able to resolve AsmResolver dependencies.

Available ObscuraX Packages
--------------------------

**Core Packages:**

- `ObscuraX.API <https://www.nuget.org/packages/ObscuraX.API/>`_ - Core interfaces and abstractions
- `ObscuraX.Core <https://www.nuget.org/packages/ObscuraX.Core/>`_ - Main obfuscation engine
- `ObscuraX.Protections <https://www.nuget.org/packages/ObscuraX.Protections/>`_ - Protection implementations
- `ObscuraX.Shared <https://www.nuget.org/packages/ObscuraX.Shared/>`_ - Shared utilities and models

**Host & Utilities:**

- `ObscuraX.Host <https://www.nuget.org/packages/ObscuraX.Host/>`_ - Application host framework
- `ObscuraX.Utilities <https://www.nuget.org/packages/ObscuraX.Utilities/>`_ - Helper functions
- `ObscuraX.Obfuscation <https://www.nuget.org/packages/ObscuraX.Obfuscation/>`_ - High-level obfuscation orchestrator
- `ObscuraX.Runtime <https://www.nuget.org/packages/ObscuraX.Runtime/>`_ - Runtime components

**Tools:**

- `ObscuraX.GlobalTool <https://www.nuget.org/packages/ObscuraX.GlobalTool/>`_ - .NET Global Tool