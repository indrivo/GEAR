# `Workflow builder`

Workflow builder is a module that belongs to Gear and aims to manage states for an object.

## Description
A workflow consists of an orchestrated and repeatable pattern of business activity enabled by the systematic organization of resources into processes that transform materials, provide services, or process information It can be depicted as a sequence of operations, declared as work of a person or group, an organization of staff, or one or more simple or complex mechanisms.

The module is made on 3 layers:

  - GR.WorkFlows
  - GR.WorkFlows.Abstractions
  - GR.Workflows.Razor
 
GR.WorkFlows.Abstractions contains interfaces:
  - `IWorkflowContext` - contains the contracts with the entities, in order to use this interface is injected
  - `IWorkFlowCreatorService` - contains the description of the workflow creation methods
  - `IWorkflowExecutorService` - contains the description of the methods of changing transitions, states and executing actions for a particular object
 
## Installation

To install this module you need to refer to GR.WorkFLows.Abtractions or to the library on Nuget with the same name
`Example`
```c#
services.RegisterGearWebApp(config =>
        {
//--------- some configuration ------------
//-------------------------------------Workflow module-------------------------------------
            config.GearServices.AddWorkFlowModule<WorkFlow, WorkFlowCreatorService, WorkFlowExecutorService>()
                .AddWorkflowModuleStorage<WorkFlowsDbContext>(options =>
                {
                    options.GetDefaultOptions(Configuration);
                    options.EnableSensitiveDataLogging();
                })
                .AddWorkflowRazorModule();

//--------- another modules ------------
```
`WorkFlowCreatorService`, `WorkFlowExecutorService` are the classes that have the basic implementation for the behavior of a workflow, they implement `IWorkFlowCreatorService` and `IWorkFlowExecutorService`, so if you want the basic implementation you need to use GR.WorkFlows. At your choice you can inherit these classes and override the methods

## Register entity contract

In order to be able to use workflow manger for an object it is necessary to create a contract for an entity. For this we use the interface `IWorkflowExecutorService`
Here we use the method:
```c#
Task<ResultModel<Guid>> RegisterEntityContractToWorkFlowAsync([Required] string entityName, Guid? workFlowId);
```
Parameters: 
- `entityName` -> represents the name of the entity, in this version there is no close connection with the entity from the database and this one from the workflow, we chose a more abstract way so that we do not have dependents and can be more generic
- `workFlowId` -> represents the id of a workflow already created using the ui builder (id belongs to the WorkFlow entity)

Also a contract can be registered using `IServiceCollection`
`Example`: 
```c#
config.GearServices.RegisterWorkFlowContract(nameof(DocumentVersion), Guid.Empty);
```
## Workflow Actions
Workflow actions are post actions that are executed when changing a state for an object
To create an action we must create a class that inherits from `BaseWorkFlowAction`, it contains an abstract method `InvokeExecuteAsync` that receives as a parameter a Dictionary parameter `Dictionary<string, string>`. This method will be called when the action is invoked to change the state of an object, of course this action must be attached to the appropriate transition.

`Base action`

```c#
 public abstract class BaseWorkFlowAction
    {
        #region Injectable

        /// <summary>
        /// Executor
        /// </summary>
        protected readonly IWorkFlowExecutorService Executor;

        #endregion

        /// <summary>
        /// Entry state
        /// </summary>
        protected EntryState EntryState { get; set; }

        /// <summary>
        /// Current transition
        /// </summary>
        protected Transition CurrentTransition { get; set; }

        /// <summary>
        /// Next transitions
        /// </summary>
        protected IEnumerable<Transition> NextTransitions { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="currentTransition"></param>
        /// <param name="nextTransitions"></param>
        protected BaseWorkFlowAction(EntryState entry, Transition currentTransition, IEnumerable<Transition> nextTransitions)
        {
            EntryState = entry;
            CurrentTransition = currentTransition;
            NextTransitions = nextTransitions;
            Executor = IoC.Resolve<IWorkFlowExecutorService>();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        public abstract Task InvokeExecuteAsync(Dictionary<string, string> data);
    }
```

`Example of send notifications actions`

```c#
    public class SendNotificationAction : BaseWorkFlowAction
    {
        #region Injectable

        /// <summary>
        /// Inject notifier
        /// </summary>
        private readonly INotify<GearRole> _notify;

        #endregion

        public SendNotificationAction(EntryState entry, Transition transition, IEnumerable<Transition> nextTransitions) : base(entry, transition, nextTransitions)
        {
            _notify = IoC.Resolve<INotify<GearRole>>();
        }

        /// <summary>
        /// Execute data
        /// </summary> 
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task InvokeExecuteAsync(Dictionary<string, string> data)
        {
            var rolesForPrevTransition = await Executor.GetAllowedRolesToTransitionAsync(CurrentTransition);
            var subject = "Entry x";
            if (data.ContainsKey("Name")) subject = data["Name"];
            await _notify.SendNotificationAsync(rolesForPrevTransition, new Notification
            {
                Subject = $"{subject} state changed",
                Content = $"{subject} has changed its status from {CurrentTransition?.FromState?.Name} to {CurrentTransition?.ToState?.Name}",
                SendLocal = true,
                SendEmail = true,
                NotificationTypeId = NotificationType.Info
            }, EntryState.TenantId);

            foreach (var nextTransition in NextTransitions)
            {
                var rolesForNextTransition = await Executor.GetAllowedRolesToTransitionAsync(nextTransition);

                await _notify.SendNotificationAsync(rolesForNextTransition, new Notification
                {
                    Subject = "You have new actions",
                    Content = $"{subject} can be switched to {nextTransition?.ToState.Name} state",
                    SendLocal = true,
                    SendEmail = true,
                    NotificationTypeId = NotificationType.Info
                }, EntryState.TenantId);
            }
        }
    }
```

### The registration of an action is done in the following way:

```c#
 services.RegisterWorkflowAction<TActionClass>();
```
Note: TActionClass need to inherit BaseWorkFlowAction

### Injecting services into action handlers
The injection of services can only be done through Castle Windsor, an example of injection
```c#
IoC.Resolve<TService>();
```
Note: This service must first be registered using `IoC` service registration methods


## The structure of a workflow

A workflow has the following structure :
- `Name` - the name of your workflow 
- `Description` - some description about it
- `Enabled` - represent if it is active for usage
- `States` - represent a list of states that caracterize it
- `Transitions` - represent transitions between states

A state has the following structure:
- `Name` - unique name for some workflow that will be displayed on usage
- `Description` - something descriptive
- `IsStartState` - represents the initial state that will be set for an object, note: only one start state can exist
- `IsEndState` - represents the last state of an object
- `AdditionalSettings` - they are used for store some settings as a dictionary (`ex`: we store here the position x and y on the builder canvas)

A transition has the following structure:
- `Name` - abstract name that identify the transition
- `FromState` - is the start point for a transition, ex: the first transition has FromState value of the first state of a workflow
- `ToState` - is the end point for entry transition
- `Actions` - actions are the handlers that will be executed after the state of object will be changed to another, regulary  in this system actions are classes that are use to execute some actions for state change
- `AllowedRoles` - here we store the user roles that can do this change of transition

## The structure of entry that use workflows
For store the state of an object we use an `Entry State` entity that have the following structure: 
- `Contract` - represents the id of contract of entity and workflow
- `EntryId` - represents the id of object
- `State` - represents the current state of object
- `Message` - represents the message that will be changed on state change

We store history of object states in `EntryStateHistory`, this entity has the following structure:
- `EntryState` - store the id of entry state
- `FromState` - store the precedent state of entry
- `ToState` - store the current state that is set in EntryState
- `Message` - represents the message that is set when the state of the object changes

## License

MIT