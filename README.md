# AleFIT Workflow <img align="right" src="https://www.alef.com/en/img/logo-alef.png">

AleFIT Workflow is a simple yet effective solution for execution code blocks in certain flow.
Library supports multiple blocks as described below, it follows fluent API principles and contains comprehensive test suite.

## Features

* Full async support
* Immutable API making it thread-safe
* Fluent API design
* No dependencies other than **.NETSTARDARD 1.2**
* Dependency injection friendly (could be improved)
* Type-safety using generics
* Documented code (could be improved)

## Requirements

* .NETSTARDARD 1.2
* .NETCORE Runtime (just to run unit tests)

## ToDo

* Ability to serialize the workflow to make it persistable
* Integration with some DI containers
* Better code documentation and examples
* Any ideas?

## Examples

Simple workflow is starting by conditional node checking whether registration email is unique.
If it is, it inserts user in database and sends him some email about the result. 
If the email is not unique, it just logs somewhere what happened.

```csharp
var workflow = WorkflowBuilder<RegistrationContext>.Create()
    .If(new CheckUserEmail())
        .Then(WorkflowBuilder<RegistrationContext>.Create()
            .Do(new InsertUserInDatabase())
            .Do(new NotifyCompleteRegistartion())
            .Build())
    .Else(new LogIncorrectRegistration())
.Build();

var context = await workflow.ExecuteAsync(new RegistrationContext("test@email.com"));
```

Same workflow using lambda syntax. These two syntaxes can be mixed together without any limitations.

```csharp
var workflow = WorkflowBuilder<RegistrationContext>.Create()
    .If(async context => await someService.IsEmailUnique(context.Email))
    .Then(WorkflowBuilder<RegistrationContext>.Create()
        .Do(async context => await repository.InsertAsync(context.UserData))
        .Do(async context => await notificationService.SendAsync(context.UserData))
        .Build())
    .Else(context =>
    {
        logger.Info($"Email {context.Email} is already registered.");
        return Task.CompletedTask;
    })
.Build();

var context = await workflow.ExecuteAsync(new RegistrationContext("test@email.com"));
```

**For more examples check out** [unit tests](https://github.com/alefgroup/AleFIT.Workflow/tree/master/AleFIT.Workflow.Test)