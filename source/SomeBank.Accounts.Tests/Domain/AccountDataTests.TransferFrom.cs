using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chessie.ErrorHandling.CSharp;
using System;
using static SomeBank.Accounts.Domain;

namespace SomeBank.Domains.Accounts.Tests.Data
{
    public partial class AccountDataTests
    {
        [TestMethod]
        public void ShouldAbleToRecieveATransfer()
        {
            var id = Guid.NewGuid();
            var result = Account.Create(VALIDNAME, 50);
            result = Account.StartTransferFrom(id, "AC002", 10, result.SucceededWith());

            var account = result.SucceededWith();

            Assert.AreEqual(1, account.PendingIn.Count);

            var pending = account.PendingIn[id];

            Assert.AreEqual(IngoingPendingOperation.Types.TransferFrom, pending.Type);
            Assert.AreEqual("AC002", pending.Source);
            Assert.AreEqual(10, pending.Value);
            Assert.IsFalse(pending.NeedsHumanApproval);
        }

        [TestMethod]
        public void ShouldBeAbleToAcceptPendingInTransfer()
        {
            var id = Guid.NewGuid();
            var result = Account.Create(VALIDNAME, 50);
            result = Account.StartTransferFrom(id, "AC002", 10, result.SucceededWith());
            result = Account.AcceptTransferFrom(id, result.SucceededWith());

            var account = result.SucceededWith();

            Assert.AreEqual(60, account.Balance);
            Assert.AreEqual(0, account.PendingIn.Count);
        }
    }
}
