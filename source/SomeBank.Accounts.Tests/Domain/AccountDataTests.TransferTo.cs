using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chessie.ErrorHandling.CSharp;
using System;
using static SomeBank.Accounts.Domain;

namespace SomeBank.Domains.Accounts.Tests.Data
{
    public partial class AccountDataTests
    {
        [TestMethod]
        public void ShouldBeAbleToTransferLessThanTheBalance()
        {
            string destination = "AC002";
            double value = 10;
            var id = Guid.NewGuid();
            var result = Account.Create(VALIDNAME, 50);
            result = Account.StartTransferTo(id, destination, value, result.SucceededWith());

            var account = result.SucceededWith();

            Assert.AreEqual(VALIDNAME, account.Name);
            Assert.AreEqual(40, account.Balance);
            Assert.AreEqual(1, account.PendingOut.Count);

            var operation = account.PendingOut[id];

            Assert.AreEqual(OutgoingPendingOperation.Types.TransferTo, operation.Type);
            Assert.AreEqual(destination, operation.Destination);
            Assert.AreEqual(value, operation.Value);
        }

        [TestMethod]
        public void ShouldBeAbleToTransferMoreThanTheBalanceButLessThanTheOverdraftLimit()
        {
            var id = Guid.NewGuid();
            var result = Account.Create(VALIDNAME, 50);
            result = Account.StartTransferTo(id, "AC002", 100, result.SucceededWith());

            var account = result.SucceededWith();

            Assert.AreEqual(VALIDNAME, account.Name);
            Assert.AreEqual(-50, account.Balance);
        }

        [TestMethod]
        public void ShouldNotBeAbleToTransferMoreThanTheOverdraftLimit()
        {
            var id = Guid.NewGuid();
            var result = Account.Create(VALIDNAME, 50);
            result = Account.StartTransferTo(id, "AC002", 200, result.SucceededWith());

            var error = result.FailedWith();

            Assert.AreEqual(Account.Errors.OverdraftLimitAchieved, error.Head);
        }

        [TestMethod]
        public void ShouldBeAbleToAcceptPendingOutTransfer()
        {
            var id = Guid.NewGuid();
            var result = Account.Create(VALIDNAME, 50);
            result = Account.StartTransferTo(id, "AC002", 10, result.SucceededWith());
            result = Account.AcceptTransferTo(id, result.SucceededWith());

            var account = result.SucceededWith();

            Assert.AreEqual(0, account.PendingOut.Count);
        }
    }
}
