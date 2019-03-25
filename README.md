# Die Scheite C# Client Library
Client libraries used for creating and publishing logging data to Die Scheite.

See `PlugAndTrade.DieScheite.Client.Example` and `PlugAndTrade.DieScheite.Client.Example.AspNetCore` for example code covering most features.

## Overview

Die Scheite is built on the concept of scopes. Everything logged using Die Scheite is associated with a scope and
grouped in a single object. A scope constitutes a single unit of processing, it could be an HTTP request, a RabbitMQ
message, a cron job or any processing with a start and an end. Any processing without a start and an end is not a scope
and cannot be logged using Die Scheite.

Each scope has some basic properties:
 * `id`: The id of the scope.
 * `parentId`: The id of the scope making the request, publishing the message etc.
 * `correlationId`: An id common to all scope with the same scope origin.
 * `serviceId`: The id or name of the service/software processing the scope.
 * `timestamp`: The time the scope was started.
 * `duration`: The duration of the scope.
 * `protocol`: The protocol used to create/start the scope, eg `http`, `rabbitmq`, `cron`.
 * `route`: A protocol specific name/string identifying which path was executed. Eg for `http` the route is the URL path
   template, for `rabbitmq` it is the name of the queue.
 * `level`: Analogous to the ordinary log level.

In addition to the above basic properties a scope contains a list of log messages and a list of traces. A trace is used
for performance measurement. Some protocols also attach protocol specific data, eg http usually attach an object with
request and response information such as method, uri, status and headers.

## C# client setup

Die Scheite could be set up in multiple ways to accomodate different protocols and environments. Some general setup is
always required, a `LogEntryFactory` must be created and at least one `ILogger` must be created. These are found in the
`PlugAndTrade.DieScheite.Client.Common` package.

### General

Install the `PlugAndTrade.DieScheite.Client.Common` nuget package.

```shell
dotnet add package PlugAndTrade.DieScheite.Client.Common
```

The `LogEntryFactory` is created with three parameters, `serviceId` (the name or id of the sercie/software),
`instanceId` (the id of this specific instance of the service if applicable), and `version` (the version of the
service/software).
