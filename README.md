# akka.patterns
A sandbox of akka patterns

## From Effective Akka (https://www.amazon.com/Effective-Akka-Jamie-Allen/dp/1449360076)

Workers Actors

    worker actors are meant for parallelization or separation of dangerous tasks into
    actors built specifically for that purpose, and the data upon which they will act is always
    provided to them. 

Domain Actors

    Domain actors, introduced in the previous section, represent a live
    cache where the existence of the actors and the state they encapsulate are a view of the
    current state of the application.
    
Extra Pattern 

    One of the most difficult tasks in asynchronous programming is trying to capture con‐
    text so that the state of the world at the time the task was started can be accurately
    represented at the time the task finishes. However, creating anonymous instances of
    Akka actors is a very simple and lightweight solution for capturing the context at the
    time the message was handled to be utilized when the tasks are successfully completed.
    
Cameo Pattern

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

Superviser-Only Pattern

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

Sentinel Pattern

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

## Stashing Patterns

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

### Bulkhead Pattern

    how to use dispatchers
    to create failure zones and prevent failure in one part of the application from affecting
    another. This is sometimes called the Bulkhead Pattern. And once I create the failure
    zones, how do I size the thread pools so that we get the best performance from the least
    amount of system resources used?
    
also see: http://skife.org/architecture/fault-tolerance/2009/12/31/bulkheads.html


From 
## Reactive Enterprise with Actor Model: Applications and Integration in Scala and Akka
https://www.amazon.de/Reactive-Enterprise-Actor-Model-Applications/dp/0133846830

Domain Supervision Actor

    The domainModel actor serves as parent and supervisor for categories of
    domain model of concepts [IDDD]
    
//TODO push the working code.
