﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDdd.Nhibernate.TestHelpers;
using CoreDdd.Nhibernate.UnitOfWorks;
using CoreDddShared.Domain;
using CoreDddShared.Dtos;
using IntegrationTestsShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using TestsShared;

namespace AspNetCoreMvcApp.IntegrationTests.Controllers.ManageShipsControllers
{
    [TestFixture]
    public class when_viewing_index
    {
        private NhibernateUnitOfWork _unitOfWork;
        private ServiceProvider _serviceProvider;
        private IServiceScope _serviceScope;

        private IActionResult _actionResult;
        private Ship _newShip;

        [SetUp]
        public async Task Context()
        {
            _serviceProvider = new ServiceProviderHelper().BuildServiceProvider();
            _serviceScope = _serviceProvider.CreateScope();

            _unitOfWork = _serviceProvider.GetService<NhibernateUnitOfWork>();
            _unitOfWork.BeginTransaction();

            _newShip = new ShipBuilder().Build();
            _unitOfWork.Save(_newShip);

            var manageShipsController = new ManageShipsControllerBuilder(_serviceProvider).Build();

            _actionResult = await manageShipsController.Index();
        }

        [Test]
        public void action_result_is_view_result()
        {
            _actionResult.ShouldBeOfType<ViewResult>();
        }

        [Test]
        public void view_model_is_collection_of_ship_dtos()
        {
            var viewResult = (ViewResult)_actionResult;
            (viewResult.Model as IEnumerable<ShipDto>).ShouldNotBeNull();
            var shipDtos = ((IEnumerable<ShipDto>)viewResult.Model).ToList();
            shipDtos.Count.ShouldBe(1);
            shipDtos.Single().Id.ShouldBe(_newShip.Id);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork.Rollback();
            _serviceScope.Dispose();
            _serviceProvider.Dispose();
        }
    }
}