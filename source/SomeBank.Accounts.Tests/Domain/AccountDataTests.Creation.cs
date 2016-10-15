using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chessie.ErrorHandling.CSharp;
using static SomeBank.Accounts.Domain;

namespace SomeBank.Domains.Accounts.Tests.Data
{
    [TestClass]
    public partial class AccountDataTests
    {
        readonly string VALIDNAME = "AC001";
        readonly double VALIDBALANCE = 0;

        [TestMethod]
        public void ShouldBeAbleToCreateAccontsStartingWithAC()
        {
            var result = Account.Create("AC001", VALIDBALANCE);
            var account = result.SucceededWith();

            Assert.AreEqual("AC001", account.Name);
        }

        [TestMethod]
        public void ShouldNotBeAbleToCreateAccontsNotStartingWithAC()
        {
            var result = Account.Create("XX001", VALIDBALANCE);
            var error = result.FailedWith();

            Assert.AreEqual(Account.Errors.NameDoesNotStartWithAC, error.Head);
        }

        [TestMethod]
        public void ShouldBeAbleToCreateAnAccountWithBalanceZero()
        {
            var result = Account.Create(VALIDNAME, 0.0);
            var account = result.SucceededWith();

            Assert.AreEqual(0.0, account.Balance);
        }

        [TestMethod]
        public void ShouldBeAbleToCreateAnAccountWithBalancePositive()
        {
            var result = Account.Create(VALIDNAME, 100);
            var account = result.SucceededWith();

            Assert.AreEqual(100.0, account.Balance);
        }

        [TestMethod]
        public void ShouldNotBeAbleToCreateAnAccountWithBalanceNegative()
        {
            var result = Account.Create(VALIDNAME, -10.0);
            var error = result.FailedWith();

            Assert.AreEqual(Account.Errors.InitialBalanceBelowZero, error.Head);
        }
    }
}
