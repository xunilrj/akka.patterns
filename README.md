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
    
Extra/Cameo Pattern 

    One of the most difficult tasks in asynchronous programming is trying to capture con‐
    text so that the state of the world at the time the task was started can be accurately
    represented at the time the task finishes. However, creating anonymous instances of
    Akka actors is a very simple and lightweight solution for capturing the context at the
    time the message was handled to be utilized when the tasks are successfully completed.
    
In the book the author creates a Extra Actor using anonimous lambdas but this is not necessary. For a example, this post use the same Extra Pattern, but with a typed Acto. See: https://www.javacodegeeks.com/2014/01/three-flavours-of-request-response-pattern-in-akka.html

Petabridge also have a post about this pattern, but have called it https://petabridge.com/blog/top-akkadotnet-design-patterns/

    The Character Actor Pattern is used when an application has some risky but critical operation to execute, but needs to protect critical state contained in other actors and ensure that there are no negative side effects.
    It’s often cheaper, faster, and more reliable to simply delegate these risky operations to a purpose-built, but trivially disposable actor whose only job is to carry out the operation successfully or die trying.
    These brave, disposable actors are Character Actors.
