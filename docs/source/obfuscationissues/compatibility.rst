The module is built for ...
===========================

Understanding Compatibility
---------------------------

When using ObscuraX for obfuscation, it's critical to ensure that the version of ObscuraX matches the framework your application is built on. 

For example, if your application is built on .NET Core, you **must** use the version of ObscuraX that is also built for .NET Core. Using an incompatible version will result in your application not functioning after obfuscation.

.. note:: 
   A common error message you may encounter is:  
   ``The module is built for .NET (Core), but you're using a version of ObscuraX intended for .NET Framework.``
   This indicates a mismatch between your app's framework and ObscuraX's version.

Examples of Compatibility
--------------------------

Here are some examples of correct and incorrect configurations:

**✅ Good Configurations:**

- **ObscuraX for .NET Core** with an application built on **.NET Core**
- **ObscuraX for .NET Framework** with an application built on **.NET Framework**

**❌ Bad Configurations (These Won't Work!):**

- **ObscuraX for .NET Core** with an application built on **.NET Framework**
- **ObscuraX for .NET Framework** with an application built on **.NET Core**

Key Takeaways
-------------

- Always ensure that **ObscuraX's framework version** matches the **framework version** of your application.
- Incompatible configurations will break your app after obfuscation.
- Carefully check the framework version of both your app and the ObscuraX release you are using.

.. warning:: 
   Mixing framework versions (e.g., using ObscuraX for .NET Framework with a .NET Core app) will cause the app to fail after obfuscation.