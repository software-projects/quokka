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

Quokka.Core depends on the following external components:

-   Castle.Core

-   Castle.Windsor

In addition, Quokka.NH depends on NHibernate
