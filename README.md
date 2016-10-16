# Akka.NET sandbox

Example of application built using Akka.net. Please Expect WIP code-quality.

## Stack

### Frontend  
Still missing

### Backend  
C#  
Topshelf  
Akka.Net
Serilog  

### Domain Model  
F#  
Chessie

### Persistence
Still missing

### Build + DevOps  
Still missing

## References

Domain-Driven Design: Tackling Complexity in the Heart of Software  
https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215

Implementing Domain-Driven Design [IDDD]  
https://www.amazon.com/gp/product/0321834577/

Reactive Messaging Patterns with the Actor Model: Applications and Integration in Scala and Akka  
https://www.amazon.com/Reactive-Messaging-Patterns-Actor-Model/dp/0133846830/

Effective Akka  
https://www.amazon.com/Effective-Akka-Jamie-Allen/dp/1449360076

Enterprise Integration Patterns: Designing, Building, and Deploying Messaging Solutions  
https://www.amazon.com/Enterprise-Integration-Patterns-Designing-Deploying/dp/0321200683

Pattern-Oriented Software Architecture Volume 1: A System of Patterns  
https://www.amazon.com/Pattern-Oriented-Software-Architecture-System-Patterns/dp/0471958697

## DDD Patterns

### Bounded Contexts

From IDDD:

    Shared Kernel: Sharing part of the model [...] can leverage design work or undermine it. [...] 
    Keep the kernel small. [...] Define a continuous integration process [...]
    
    Customer-Supplier Development: When two teams are in an upstream-downstream relationship, 
    where the upstream team may succeed interdependently of the fate of the downstream team, 
    [...] Negotiate and budget tasks for downstream requirements so that everyone understands 
    the commitment and schedule.

    Conformist: When two development teams have an upstream/downstream relationship in which the 
    upstream team has no motivation to provide for the downstream team’s needs, the downstream 
    team is helpless. [...] The downstream [...] slavishly adhering to the model of the upstream team.
    
    Anticorruption Layer: [...]  when control or communication is not adequate to pull off a 
    shared kernel, partner, or customer-supplier relationship [...] As a downstream client,
    create an isolating layer to provide your system with functionality of the upstream system in
    terms of your own domain model.
    [...]
    A Domain Service (7) can be defined in the downstream Context for each type of Anticorruption Layer. You may also put an Anticorruption Layer behind a Repository (12) interface. If using REST, a client Domain Service
    implementation accesses a remote Open Host Service. Server responses produce representations
    as a Published Language. The downstream Anticorruption Layer translates representations into
    domain objects of its local Context.
    
    Open Host Service: Define a protocol that gives access to your subsystem as a set of services.
    [...]
    This pattern can be implemented as REST-based resources [...] We generally think of 
    Open Host Service as a remote procedure call (RPC) API, but it can be implemented
    using message exchange.
    
    Published Language: The translation between the models of two Bounded Contexts requires a 
    common language. Use a well-documented shared language that can express the necessary domain
    information as a common medium of communication [...]
    [...]
    This can be implemented [...] as an XML schema. When expressed with REST-based services,
    the Published Language is rendered as representations of domain concepts. [...] 
    It is also possible to render representations as Google Protocol Buffers. If you are
    publishing Web user interfaces, it might also include HTML representations. One advantage 
    to using REST is that each client can specify its preferred Published Language, 
    and the resources render representations in the requested content type. REST also has the
    advantage of producing hypermedia representations, which facilitates HATEOAS.
    Hypermedia makes a Published Language very dynamic and interactive, enabling clients
    to navigate to sets of linked resources. The Language may be published using standard 
    and/or custom media types. A Published Language is also used in an Event-Driven
    Architecture (4), where Domain Events (8) are delivered as messages to subscribing interested parties.
    
    Separate Ways: [...] If two sets of functionality have no significant relationship, they can
    be completely cut loose from each other.
    
    Big Ball of Mud: [...] models are mixed and boundaries are inconsistent. Draw a boundary around 
    the entire mess and designate it a Big Ball of Mud.

### Aggregates

From IDDD

    [...] model true invariants in consistency boundaries according to real business rules.
    Consider the advantages of designing small Aggregates.
    See why you should design Aggregates to reference other Aggregates by identity.
    Discover the importance of using eventual consistency outside the Aggregate boundary.
    Learn Aggregate implementation techniques, including Tell, Don’t Ask and Law of Demeter.
    
### Entities

From IDDD

    Consider why Entities have their proper place when we need to model unique things.
    See how unique identities may be generated for Entities.
    Look in on a design session as a team captures its Ubiquitous Language (1) in Entity design.
    Learn how you can express Entity roles and responsibilities.
    See examples of how Entities can be validated and how to persist them to storage.
    
### Value Objects

From IDDD

    Learn how to understand the characteristics of a domain concept to model as a Value.
    See how to leverage Value Objects to minimize integration complexity.
    Examine the use of domain Standard Types expressed as Values.
    Consider how SaaSOvation learned the importance of Values.
    Learn how the SaaSOvation teams tested, implemented, and persisted their Value types.

//TODO replace the chapter roadmap with better guidelines

## CQRS

Why CQRS?
From [IDDD]

    It can be difficult to query from Repositories all the data users need to view.
    This is especially so when user experience design creates views of data that
    cuts across a number of Aggregate types and instances. The more sophisticated 
    your domain, the more this situation tends to occur.
    Using only Repositories to solve this can be less than desirable. We could require
    clients to use multiple Repositories to get all the necessary Aggregate instances,
    then assemble just what’s needed into a Data Transfer Object (DTO) [Fowler, P of EAA].
    Or we could design specialized finders on various Repositories to gather the disjointed
    data using a single query. If these solutions seem unsuitable, perhaps we should instead
    compromise on user experience design, making views rigidly adhere to the model’
    s Aggregate boundaries. Most would agree that in the long run a mechanical and spartan user
    interface won’t suffice.
    Is there an altogether different way to map domain data to views? The answer lies in the
    oddly named architecture pattern CQRS [Dahan, CQRS; Nijof, CQRS]. It is the result of pushing
    a stringent object (or component) design principle, command-query separation (CQS), up to
    an architecture pattern.
    This principle, devised by Bertrand Meyer, asserts the following:
        Every method should be either a command that performs an
        action, or a query that returns data to the caller, but
        not both. In other words, asking a question should not change
        the answer. More formally, methods should return a value only 
        if they are referentially transparent and hence possess no
        side effects. [Wikipedia, CQS]
        
## Event Sourcing

From IDDD:

    There are varying definitions of Event Sourcing, so some clarification is fitting.
    We are discussing the use where every operational command executed on any given
    Aggregate instance in the domain model will publish at least one Domain Event that
    describes the execution outcome. Each of the events is saved to an Event Store (8)
    in the order in which it occurred. When each Aggregate is retrieved from its
    Repository, the instance is reconstituted by playing back the Events in the order
    in which they previously occurred

## Long Process

From IDDD:

    Long-Running Processes, aka Sagas
    A Long-Running Process is sometimes called a Saga, but depending on your background that
    name may collide with a preexisting pattern. An early description of Sagas is presented 
    in [Garcia-Molina & Salem].
    //IMPROVE description

## Interesting Akka Patterns and References

### Effective Akka (https://www.amazon.com/Effective-Akka-Jamie-Allen/dp/1449360076)

#### Workers Actors

    worker actors are meant for parallelization or separation of dangerous tasks into
    actors built specifically for that purpose, and the data upon which they will act is always
    provided to them. 

#### Domain Actors

    Domain actors, introduced in the previous section, represent a live
    cache where the existence of the actors and the state they encapsulate are a view of the
    current state of the application.
    
#### Extra Pattern 

    One of the most difficult tasks in asynchronous programming is trying to capture con‐
    text so that the state of the world at the time the task was started can be accurately
    represented at the time the task finishes. However, creating anonymous instances of
    Akka actors is a very simple and lightweight solution for capturing the context at the
    time the message was handled to be utilized when the tasks are successfully completed.
    
#### Cameo Pattern

    Those are good reasons for pulling the type you’re creating with the Extra Pattern into
    a pre-defined type of actor, where you create an instance of that type for every message
    handled.
    
Also See: https://www.javacodegeeks.com/2014/01/three-flavours-of-request-response-pattern-in-akka.html

Petabridge also have a post about this pattern, but have called it https://petabridge.com/blog/top-akkadotnet-design-patterns/

    The Character Actor Pattern is used when an application has some risky but critical operation to execute,
    but needs to protect critical state contained in other actors and ensure that there are no negative side effects.
    It’s often cheaper, faster, and more reliable to simply delegate these risky operations to a purpose-built,
    but trivially disposable actor whose only job is to carry out the operation successfully or die trying.
    These brave, disposable actors are Character Actors.

#### Superviser-Only Pattern

    introducing a layer of supervision between the
    [one hierarchy] and [another hierarchy of] children, as we see [below] [...] I can tailor
    supervision to be specific to failure that can occur with accounts and that which may
    occur with [a hierarchy].
    
                            +---------------+
                  +---------+   Actor       +-------+
                  |         +---------------+       |
                  |                                 |
         +--------+------+                      +---+---------------+
       +-+  Supervisor 1 +--+                 +-+ Supervisor 2      +--+
       | +---------------+  |                 | +-------------------+  |
       |                    |                 |                        |
    +--+------+     +-------+--+         +----+-----+         +--------+--+
    | child 1 |     | child 2  |         | child 3  |         | child 4   |
    +---------+     +----------+         +----------+         +-----------+
    
also see http://getakka.net/docs/concepts/supervision

    At this point it is vital to understand that supervision is about forming a recursive fault
    handling structure. If you try to do too much at one level, it will become hard to reason about,
    hence the recommended way in this case is to add a level of supervision.

#### Sentinel Pattern

    I call these kinds of actors
    “sentinels,” as they guard the system from falling out of synchrony with a known
    expectation of what state should exist in the system.

## Best Practices

https://petabridge.com/blog/how-to-stop-an-actor-akkadotnet/

Use Immutable Messages with Immutable Data

    The reasons for this are the same as stabilizing a variable before using it in a future—you
    don’t know when the other actor will actually handle the message or what the value of the
    variable will be at that time. The same goes for when you want to send an immutable value
    that contains mutable attributes.
    Erlang does this for you with copy on write (COW) semantics, but we don’t get that for free
    in the JVM. Assuming your application has the heap space, take advantage of it.
    Hopefully, these copies will be short-lived allocations that never leave the Eden space of
    your garbage collection generations, and the penalty for having duplicated the value will be minimal.
    
also see:
http://hackerboss.com/i-have-seen-the-future-and-it-is-copy-on-write/  
https://www.nuget.org/packages/System.Collections.Immutable/

Immutability in c# is Hard

https://blogs.msdn.microsoft.com/lucabol/2007/12/03/creating-an-immutable-value-object-in-c-part-i-using-a-class/

Comparison between c# and f# for Immutability
http://www.slideshare.net/ScottWlaschin/domain-driven-design-with-the-f-type-system-functional-londoners-2014

### Akka Documentation

####  Stashing Patterns

http://getakka.net/docs/working-with-actors/Stashing%20Messages TODO

### Guaranteed Operation between Two Actors

When DDD is applied with Actor based, it is common to model Aggregate Roots with Actors
see http://pkaczor.blogspot.co.uk/2014/04/reactive-ddd-with-akka.html

To allow horizontal scalability, the ideal is that each Aggregate Root could be running in a different machine. Hence, there is no distributed transaction mechanism to allow modification of two Actors, for example the famous "transfer money between two accounts".

To allow such operation without distributed transaction, one can use the following workflow. 

1 - With just one operation on the Source AccountActor:  
1.1 - Decrease the account balance (the desired operation)  
1.2 - Insert that there is an operation in progress (pending operation storage)  
2 - Persist Source AccountActor;
2.1 - From this point it is guaranteed that the operation will finish;
3 - Create a TransferActor to communicate with Destination AccountActor; 
3.1 - AccountActor must have a mechanism to guarantee that for every operation in progress exist one associated TransferActor (Cameo Pattern);
4 - TransferActor must send the message to the Destination AccountActor "n" times following some kind of pattern (every x seconds, for example);  
4.1 - If after n transient errors or a non-transient error, the actor can enter in a human-attention-needed state (maybe the destination account does not exist);  
5 - TransferActor enter in Wait-for-confirmation state;  
6 - Destination AccountActor will receive the message and do something: or will accept the tranfer or will ask the Account Manager if the money must be accepted, for example;
7 - After the transfer is accepted, Destination AccountActor will send the confirmation that the Transfer was accepted to the TransferActor;  
8 - TransferActor will send a message to its parent telling that the transfer was accepted and will kill itself;  
9 - Source AccountActor will remove the operation from the  in-progress-list;
10 - Operation finished!

This complex workflow guarantees that the operation will be completed even with the following errors:  
1 - Source AccountActor modified its data but could not start the TransferActor;  
1.1 - The Source AccountActor have a auto-healing mechanism to always have a TransferActor to each operation-in-progress. So eventually the TransferActor will be started;
1.2 - If for some reason the application died, when the application start again, the AccountActor will create the TransferActor for each operation-in-progress;  
2 - TransferActor started but somehow died;  
2.1 - Transient errors will be handled by the retry mechanism;  
2.2 - Non-transient errors will scalate to human intervention;  
2.3 - If some reason the TransferActor vanished, the AccountActor-auto-healing-mechanism will recreate it.  
3 - Source TransferActor cannot communicate with the Destination AccountActor  
3.1 - Transient errors will be handled by the retry mechanism;  
1.2 - Non-transient errors will scalate to human intervention;  
4 - Destination AccountActor accepted the transfer, persisted its data, but died after that;  
4.1 - The Source TransferActor will resend the message after some time;
4.1 - Destination AccountActor must be able to recognize that a tranfer was already accepted (must idempotently process this message). So if it died before sending the confirmation the Source TransferActor will resend the request and the Destination AccountActor will confirm the transfer without doing any data modification.  
5 - The TransferActor died between waiting the confirmation and sending the confirmation to its parent  
5.1 - The Source AccountActor will recreat it, the TransferActor will send the request again, the destination AccountActor will just confirm it;  
6 - The TransferActor have sent the confirmation to the AccountActor and killed itself but the AccountActor have not persisted the operation in progress updated  
6.1 - The AccountActor auto-healing-mechanism will start the whole process again until everything works.

see:
Guaranteed Sender Actor
Guaranteed Receiver Actor

### Bulkhead Pattern

    how to use dispatchers
    to create failure zones and prevent failure in one part of the application from affecting
    another. This is sometimes called the Bulkhead Pattern. And once I create the failure
    zones, how do I size the thread pools so that we get the best performance from the least
    amount of system resources used?
    
also see: http://skife.org/architecture/fault-tolerance/2009/12/31/bulkheads.html

### Reactive Enterprise with Actor Model: Applications and Integration in Scala and Akka
https://www.amazon.de/Reactive-Enterprise-Actor-Model-Applications/dp/0133846830

#### Domain Supervision Actor

    The domainModel actor serves as parent and supervisor for categories of
    domain model of concepts [IDDD]
    
#### Guaranteed Sender Actor

    By definition, when using Actor Model, messages are delivered to an actor at most once.
    For many uses this all works out just fine, especially when considering that any given
    message is almost always successfully delivered. As well, when a given message is not
    actually delivered, there are ways to make sure that it is eventually. For example, the
    sending actor can listen for a response to its sent message, and also set a timeout. If the
    timeout fires before the response is received, the client actor will simply resend the
    message. In the case of a crash, possibly restart the receiving actor before it sends the
    repeat message.
    
also see: http://getakka.net/docs/persistence/at-least-once-delivery
