﻿using CoreDdd.Nhibernate.Repositories;
using CoreDdd.Nhibernate.TestHelpers;
using CoreDddShared.Commands;
using CoreDddShared.Domain;
using NUnit.Framework;
using Shouldly;

namespace CoreDddShared.IntegrationTests.Commands
{
    [TestFixture]
    public class when_creating_new_ship
    {
        private PersistenceTestHelper _p;
        private Ship _persistedShip;
        private int _generatedShipId;

        [SetUp]
        public void Context()
        {
            _p = new PersistenceTestHelper(new CoreDddSharedNhibernateConfigurator());
            _p.BeginTransaction();

            var createNewShipCommand = new CreateNewShipCommand
            {
                ShipName = "ship name",
                Tonnage = 23.45678m
            };
            var createNewShipCommandHandler = new CreateNewShipCommandHandler(new NhibernateRepository<Ship>(_p.UnitOfWork));
            createNewShipCommandHandler.CommandExecuted += args => _generatedShipId = (int) args.Args;
#if NET40
            createNewShipCommandHandler.Execute(createNewShipCommand);
#endif
#if !NET40
            createNewShipCommandHandler.ExecuteAsync(createNewShipCommand).Wait();
#endif

            _p.Clear();

            _persistedShip = _p.Get<Ship>(_generatedShipId);
        }

        [Test]
        public void ship_can_be_retrieved_and_data_are_persisted_correctly()
        {
            _persistedShip.ShouldNotBeNull();
            _persistedShip.Name.ShouldBe("ship name");
            _persistedShip.Tonnage.ShouldBe(23.45678m);
        }

        [TearDown]
        public void TearDown()
        {
            _p.Rollback();
        }
    }
}