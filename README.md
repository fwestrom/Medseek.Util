#Medseek.Util

Medseek Utility library for .NET projects and Micro-Services SDK.

## Vision

The Medseek Utility library for .NET projects (from here on, just Util) offers two primary features that can be leveraged by other projects: 

  1. an abstraction layer and utility library from which applications can be built to leverage 3rd party frameworks in a pluggable manner without coupling the application directly to the 3rd party libraries, and

  2. a collection of enabling applications that can be used to bootstrap product development.

### Abstraction Layer / Utility Library

The .NET abstraction layer is enabled by structuring project dependencies from plugin projects (named by the convention Medseek.Util.Plugin.{3rdPartyLibrary}) towards the base Medseek.Util project, with plugin projects implementing abstractions defined in Medseek.Util, rather than adding dependencies directly to the 3rd party libraries from Medseek.Util.  Dependencies on the .NET framework base class library are allowable in the base Medseek.Util project, but only if their functionality is fully provided by and included with the standard installation of the .NET Framework as expected on a non-development end user desktop.

Let me clarify that: **a dependency on a 3rd party library should never be added Medseek.Util**.  Instead, an abstraction should be created in Medseek.Util, and a new project sholud be added that depends on Medseek.Util and implements the abstraction using the 3rd party library.  However, all tests should be defined in the single Medseek.Util.Test project, unless otherwise strictly necessary.

This structure enables applications to select a specific implementation of a pluggable abstraction from the base library as desired, instead of having to accept whatever comes along for potentially undesired reasons.

When creating 3rd party abstraction implementations that provide features common to other 3rd party abstractions, it is necessary to use or create the appropriate other abstraction and create any additional wiring that may be necessary to connect the concerns (for example, the Web API dependency resolver uses the base library inversion of control abstraction instead of directly using a Windsor container, because it would violate the principles and vision of this project to have multiple 3rd party dependencies in a single project (seperate 3rd party plug-in projects for the Web API dependency and for the Castle Windsor dependency). 

### Enabling Applications

A collection of enabling applications is provided to bootstrap product development, and simplify development of applications in a micro-services architecture by providing features that extensibly automate the otherwise repetitive and difficult-to-maintian aspects of those types of projects.

#### Micro-Services Application Server

Provides a top-level application server, into which micro-services can be deployed, that starts, restarts, and otherwise manages the processes for micro-services (runs as a Windows service or console application).

TODO: Finish discussing the micro-services application server.

#### Micro-Services Host

Enbables simplified development of .NET micro-services in class library projects without having to define the process entry point and related concerns that rarely vary between implementations of micro-services.

TODO: Finish discussing the micro-services host.


## New Developer Ramp-up Resources

### Concepts

#### Dependency Injection

- http://martinfowler.com/articles/injection.html

#### Test-Driven Development

- http://en.wikipedia.org/wiki/Test-driven_development

#### Micro-Services

- http://martinfowler.com/articles/microservices.html

### Libraries and Tools

#### Moq

- [Moq4](https://github.com/Moq/moq4)

#### NUnit

- [NUnit](http://nunit.org)

#### RabbitMQ

- [Getting started with RabbitMQ](http://www.rabbitmq.com/getstarted.html)

#### Castle Project

- [Castle Windsor](http://docs.castleproject.org/Windsor.MainPage.ashx)

