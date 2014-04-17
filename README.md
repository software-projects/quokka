Quokka
======

Quokka is a library used to add some structure to Windows Forms programs. It
provides:



-   Support for Model-View-Presenter (MVP)

-   Grouping of View/Presenter pairs into tasks, in which navigation between
    views is easy

-   Publish/Subscribe event pattern for communication between objects in the
    process

-   Diagnostic classes for verification and logging

-   Network support classes and utilities, including STOMP protocol support

-   Threading utilities (many of which are not needed in .NET 4.5)

-   Convenient classes for saving UI settings (such as window sizes, splitter
    window location, etc)

Assemblies
----------

**Quokka.Core **is the core assembly and contains most of the functionality.
Apart from the CLR assemblies, Quokka.Core depends on:

-   Castle.Core

-   Castle.Windsor

**Quokka.NH** contains additional functionality for integration with NHibernate.
Apart from the CLR assemblies, Quokka.NH depends on:

-   Castle.Core

-   Castle.Windsor

-   NHibernate

Roadmap
-------

Quokka is not under active support. It is, however, used in a number of software
products used by clients of Software Projects. Bug fixes are applied as they are
identified.
