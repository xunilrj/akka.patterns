module SomeBank.Accounts.Domain

module OutgoingPendingOperation =
    open Chessie.ErrorHandling
    type Types =
        | TransferTo
    type Errors = 
        | ValueNotPositive
    [<NoComparison;NoEquality>]
    type T = {
        Id: System.Guid;
        Type: Types;
        Destination:string;
        Value:double
    }
    let warnWhenBalanceIsNotPositive operation =
        if operation.Value <= 0.0
        then warn ValueNotPositive operation
        else ok operation
    let creationInvariance = warnWhenBalanceIsNotPositive >> failOnWarnings
    let CreateTransferTo id destination value =
        {
            Id = id;
            Type =TransferTo;
            Destination = destination;
            Value = value
        } |> creationInvariance

module IngoingPendingOperation =
    open Chessie.ErrorHandling
    type Types =
        | TransferFrom
    type Errors = 
        | ValueNotPositive
    [<NoComparison;NoEqualityAttribute>]
    type T = {
        Id: System.Guid;
        Type: Types;
        Source : string;
        Value:double;
        NeedsHumanApproval:bool
    }
    let warnWhenBalanceIsNotPositive operation =
        if operation.Value <= 0.0
        then warn ValueNotPositive operation
        else ok operation
    let creationInvariance = warnWhenBalanceIsNotPositive >> failOnWarnings
    let CreateTransferFrom id source value =
        {
            Id = id;
            Type = TransferFrom;
            Source = source;
            Value = value;
            NeedsHumanApproval = false
        } |> creationInvariance

module Account =
    open Chessie.ErrorHandling
    [<NoComparison;NoEquality>]
    type T = {
        Name:string;
        Balance:double;
        OverdraftLimit:double;
        PendingOut:Map<System.Guid,OutgoingPendingOperation.T>;
        PendingIn:Map<System.Guid,IngoingPendingOperation.T>;
    }
    type Errors =
    | InitialBalanceBelowZero
    | NameDoesNotStartWithAC
    | OverdraftLimitAchieved
    | GenericError
    //Invariances
    let warnWhenInitialBalanceBelowZero account =
        if account.Balance < 0.0 then warn InitialBalanceBelowZero account
        else pass account
    let warnWhenNameDoesNotStartWithAC account =
        if account.Name.StartsWith("AC") = false then warn NameDoesNotStartWithAC account
        else pass account
    let warnWhenBalanceBelowOverdraftLimit account =
        if account.Balance < account.OverdraftLimit then warn OverdraftLimitAchieved account
        else pass account
    //Composite Invariantes
    let objectInvariance = warnWhenNameDoesNotStartWithAC >> bind warnWhenBalanceBelowOverdraftLimit >> failOnWarnings
    let creationInvariance = warnWhenInitialBalanceBelowZero >> bind warnWhenNameDoesNotStartWithAC >> bind warnWhenBalanceBelowOverdraftLimit >> failOnWarnings
    //Transitions
    let Create name initialBalance =
        {Name = name;
        Balance = initialBalance;
        OverdraftLimit = -100.0;
        PendingOut = Map.empty;
        PendingIn = Map.empty} |> creationInvariance
    let StartTransferTo id destination value account =
        let adjustBalance account = pass {account with Balance = account.Balance - value}
        let insertOperation account = 
            let newOperation = OutgoingPendingOperation.CreateTransferTo id destination value
            match newOperation with
            | Pass op -> pass {account with PendingOut = account.PendingOut.Add(op.Id, op) }
            | _ -> warn GenericError account
        let run = adjustBalance >> bind insertOperation >> bind objectInvariance
        account |> run
    let StartTransferFrom id source value account =
        let insertOperation account =
            let newOperation = IngoingPendingOperation.CreateTransferFrom id source value
            match newOperation with
            | Pass op -> pass {account with PendingIn = account.PendingIn.Add(op.Id, op) }
            | _ -> warn GenericError account
        let run = insertOperation
        account |> run
    let AcceptTransferFrom id account =
        let operation = account.PendingIn.Item id
        let adjustBalance account = pass {account with Balance = account.Balance + operation.Value}
        let removeOperation account =
            pass {account with PendingIn = account.PendingIn.Remove id}
        let run = adjustBalance >> bind removeOperation >> bind objectInvariance
        account |> run
    let AcceptTransferTo id account =
        let operation = account.PendingOut.Item id
        let removeOperation account =
            pass {account with PendingOut = account.PendingOut.Remove id}
        let run = removeOperation >> bind objectInvariance
        account |> run

//let (>>=) = Chessie.ErrorHandling.Trial.bind
//let (<!>) = Chessie.ErrorHandling.Trial.lift
//let ok1 = Account.create "AC001" 20.0
//let nok1 = Account.create "AC001" -20.0
//let nok2 = Account.create "001" 20.0
//let nok3 = Account.create "001" -20.0
//
//let giveAC002100Bucks = Account.startTransferTo "AC002" 100.0
//let nok41 = Account.create "AC001" 20.0
//let nok42 = giveAC002100Bucks >>= nok41
//let nok43 = giveAC002100Bucks >>= nok42

//type RecordWithString = {Name:string}
//type RecordWithMap = {
//    SomeMap: Map<int,string>;
//    SomeList: List<int>}
//let createRecordWithMap = {
//    SomeMap = Map.empty;
//    SomeList = List.Empty }