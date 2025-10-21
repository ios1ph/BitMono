How To Use
==========

Choose your integration:

CLI Tool
--------

Download and run ObscuraX from the command line.

Download
~~~~~~~~

Get ObscuraX from `GitHub releases <https://github.com/sunnamed434/ObscuraX/releases/latest>`_:

- .NET 8 apps: ``ObscuraX-v0.25.3-CLI-net8.0-win-x64.zip``
- .NET Framework apps: ``ObscuraX-v0.25.3-CLI-net462-win-x64.zip``
- Unity/Mono: Use the .NET Framework version

Usage
~~~~~

.. code-block:: console

   ObscuraX.CLI -f MyApp.exe

More options:

.. code-block:: console

   ObscuraX.CLI -f MyApp.exe -l Dependencies -o MyObfuscatedApp -p FullRenamer,StringEncryption

Available options:

- ``-f, --file``: File to obfuscate (required)
- ``-l, --libraries``: Dependencies folder (default: ``libs``)
- ``-o, --output``: Output folder (default: ``output``)
- ``-p, --protections``: Which protections to use
- ``--protections-file``: Custom protections config file
- ``--criticals-file``: Custom criticals config file
- ``--logging-file``: Custom logging config file
- ``--obfuscation-file``: Custom obfuscation config file
- ``--no-watermark``: Turn off watermarking
- ``--strong-name-key``: Path to strong name key (.snk) file for assembly signing

Setup
~~~~~

Put your files like this:

.. code-block:: text

   my_project/
   ├── MyApp.exe
   └── libs/
       ├── SomeLibrary.dll
       └── AnotherLibrary.dll

The ``libs`` folder has your app's dependencies. ObscuraX needs these to understand your code.

.NET Global Tool
----------------

Install and use ObscuraX as a global .NET tool.

Installation
~~~~~~~~~~~~

.. code-block:: console

   dotnet tool install --global ObscuraX.GlobalTool

Usage
~~~~~

Same as CLI tool, just run ``ObscuraX`` instead of ``ObscuraX.CLI``.

Configuration
-------------

ObscuraX uses these config files:

``protections.json`` - Which protections to use:

.. code-block:: text

   {
     "Protections": [
       {
         "Name": "AntiILdasm",
         "Enabled": false
       },
       {
         "Name": "AntiDe4dot",
         "Enabled": false
       },
       {
         "Name": "FullRenamer",
         "Enabled": true
       },
       {
         "Name": "StringsEncryption",
         "Enabled": false
       },
       {
         "Name": "BitDotNet",
         "Enabled": true
       },
       {
         "Name": "ObscuraX",
         "Enabled": true
       }
     ]
   }

.. note::

   The order of protections in the configuration determines their execution order. 
   Packers (like BitDotNet and ObscuraX) always run last, regardless of their position in the configuration.

``criticals.json`` - What NOT to obfuscate:

.. code-block:: text

   {
     "UseCriticalAttributes": true,
     "CriticalAttributes": [
       {
         "Namespace": "UnityEngine",
         "Name": "SerializeField"
       }
     ],
     "UseCriticalModelAttributes": true,
     "CriticalModelAttributes": [
       {
         "Namespace": "System",
         "Name": "SerializableAttribute"
       }
     ],
     "UseCriticalInterfaces": true,
     "CriticalInterfaces": [
       "IRocketPlugin",
       "IOpenModPlugin"
     ],
     "UseCriticalBaseTypes": true,
     "CriticalBaseTypes": [
       "RocketPlugin*",
       "OpenModUnturnedPlugin*"
     ],
     "UseCriticalMethods": true,
     "CriticalMethods": [
       "Awake",
       "Start",
       "Update",
       "OnDestroy"
     ],
     "UseCriticalMethodsStartsWith": true,
     "CriticalMethodsStartsWith": [
       "OV_"
     ]
   }

This file controls what gets excluded from obfuscation:

- **CriticalAttributes** - Exclude members with specific attributes
- **CriticalModelAttributes** - Exclude types with serialization attributes  
- **CriticalInterfaces** - Exclude types that inherit specific interfaces
- **CriticalBaseTypes** - Exclude types that inherit specific base types (supports glob patterns)
- **CriticalMethods** - Exclude methods by exact name
- **CriticalMethodsStartsWith** - Exclude methods that start with specific strings

You can use glob patterns (``*``) in base types and method patterns.

``obfuscation.json`` - General settings:

.. code-block:: text

   {
     "Watermark": true,
     "OutputDirectoryName": "output"
   }

Most settings have sensible defaults. You only need to change them if you want something different.

Unity Integration
----------------

ObscuraX includes Unity integration that automatically obfuscates your assemblies during the Unity build process. 
The integration hooks into Unity's build pipeline and runs ObscuraX CLI to protect your game code.

.. note::

   IL2CPP is not supported yet, however is planned to be supported in the future.

Installation
~~~~~~~~~~~

Download the Unity Integration
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

1. Go to the latest ObscuraX release on `GitHub <https://github.com/sunnamed434/ObscuraX/releases/latest>`_
2. For your Unity version, download the archive with both package formats:

   - File name pattern: ``ObscuraX-Unity-v<version>-Unity<unityVersion>.zip``
   - Example: ``ObscuraX-Unity-v1.2.3-Unity2021.3.45f1.zip``

   The archive contains:

   - ``ObscuraX-Unity-v<version>-Unity<unityVersion>.unitypackage`` (classic package)
   - ``ObscuraX-Unity-UPM-v<version>-Unity<unityVersion>.tgz`` (Unity Package Manager tarball)

Install (choose one)
~~~~~~~~~~~~~~~~~~~~

.. rubric:: Option A — Import .unitypackage (recommended for most users)

1. Extract the downloaded ``.zip``
2. In Unity: **Assets → Import Package → Custom Package**
3. Select ``ObscuraX-Unity-v<version>-Unity<unityVersion>.unitypackage``
4. Click **Import**

.. rubric:: Option B — Install via Unity Package Manager (UPM)

1. Extract the downloaded ``.zip``
2. In Unity: **Window → Package Manager**
3. Click the ``+`` dropdown → **Add package from tarball...**
4. Select ``ObscuraX-Unity-UPM-v<version>-Unity<unityVersion>.tgz``
5. Confirm installation

Project Structure
~~~~~~~~~~~~~~~~~

After importing, your project will contain:

.. code-block:: text

   Assets/
   ├── ObscuraX.Unity/
   │   ├── Editor/
   │   │   ├── ObscuraXBuildProcessor.cs    # Build hook implementation
   │   │   ├── ObscuraXConfig.cs            # Configuration ScriptableObject
   │   │   ├── ObscuraXConfigInspector.cs   # Unity Inspector UI
   │   │   └── ObscuraX.Unity.Editor.asmdef # Assembly definition
   │   ├── ObscuraXConfig.asset             # Your configuration file
   │   └── package.json                    # Unity Package Manager metadata
   └── ObscuraX.CLI/
       ├── ObscuraX.CLI.exe                 # The actual obfuscation tool
       ├── protections.json                # Protection settings
       ├── obfuscation.json                # Obfuscation settings
       ├── criticals.json                  # What not to obfuscate
       └── logging.json                    # Logging configuration

Configuration
~~~~~~~~~~~~~

1. In Unity, go to **Window → ObscuraX → Configuration**
2. Check **Enable Obfuscation** to turn on ObscuraX
3. That's it! ObscuraX will automatically protect your code during builds

The integration comes with sensible defaults. You only need to change settings if you want something different.

Usage
~~~~~

Just build your project normally:

1. Go to **File → Build Settings → Build**
2. ObscuraX automatically obfuscates your code during the build
3. Your final build contains protected code

That's it! No extra steps needed.

Troubleshooting
--------------

For detailed troubleshooting information, see the `troubleshooting guide <troubleshooting.html>`_.

NuGet Package Integration (For Developers)
------------------------------------------

.. note::

   This section is for developers who want to integrate ObscuraX into their own obfuscation tools or build custom solutions. For regular users, the CLI tool or Unity integration are recommended.

ObscuraX is also available as NuGet packages, allowing you to integrate obfuscation capabilities directly into your own applications or build custom obfuscation tools.

Available Packages
~~~~~~~~~~~~~~~~~~

**Core Components:**

- `ObscuraX.API <https://www.nuget.org/packages/ObscuraX.API/>`_ - Core interfaces and abstractions
- `ObscuraX.Core <https://www.nuget.org/packages/ObscuraX.Core/>`_ - Main obfuscation engine
- `ObscuraX.Protections <https://www.nuget.org/packages/ObscuraX.Protections/>`_ - Collection of protection implementations
- `ObscuraX.Shared <https://www.nuget.org/packages/ObscuraX.Shared/>`_ - Shared utilities and models

**Host & Utilities:**

- `ObscuraX.Host <https://www.nuget.org/packages/ObscuraX.Host/>`_ - Application host framework
- `ObscuraX.Utilities <https://www.nuget.org/packages/ObscuraX.Utilities/>`_ - Helper functions and utilities
- `ObscuraX.Obfuscation <https://www.nuget.org/packages/ObscuraX.Obfuscation/>`_ - High-level obfuscation orchestrator
- `ObscuraX.Runtime <https://www.nuget.org/packages/ObscuraX.Runtime/>`_ - Runtime components for obfuscated assemblies

Configuration
~~~~~~~~~~~~~

When using NuGet packages, you'll need to configure ObscuraX programmatically or through configuration files. See the `developer documentation <../developers/configuration.html>`_ for detailed configuration options.

Dependencies
~~~~~~~~~~~~

ObscuraX NuGet packages may use nightly versions of AsmResolver. If you encounter dependency resolution issues, see the `NuGet configuration guide <nuget-configuration.html>`_ for setup instructions.

Next Steps
----------

- Read about `available protections <../protection-list/overview.html>`_
- Learn about `configuration options <../configuration/protections.html>`_
- Check `best practices <../bestpractices/zero-risk-obfuscation.html>`_
- Explore `developer documentation <../developers/first-protection.html>`_ for custom protections
