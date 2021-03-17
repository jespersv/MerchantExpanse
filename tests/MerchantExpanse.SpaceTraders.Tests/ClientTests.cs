﻿using MerchantExpanse.SpaceTraders.Models;
using MerchantExpanse.SpaceTraders.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MerchantExpanse.SpaceTraders.Tests
{
	[TestClass]
	public class ClientTests
	{
		[TestMethod]
		public async Task GetUserAsync_ReturnsUser()
		{
			var user = new User()
			{
				Username = "test"
			};
			var mockRestClient = RestSharpMocks.BuildMockRestClient(HttpStatusCode.OK, "user", user);
			var client = new Client("apitoken", "username", mockRestClient.Object);

			var result = await client.GetUserAsync();

			mockRestClient.Verify();
			Assert.IsNotNull(result);
			Assert.AreEqual(user.Username, result.Username);
		}

		#region Loans

		[TestMethod]
		public async Task GetLoansAsync_ReturnsLoans()
		{
			var loans = new List<Loan>()
			{
				new Loan()
				{
					Id = "1",
					Due = DateTime.UtcNow,
					RepaymentAmount = 1000,
					Status = "CURRENT",
					Type = "STARTUP"
				}
			};
			var mockRestClient = RestSharpMocks.BuildMockRestClient(HttpStatusCode.OK, "loans", loans);
			var client = new Client("apitoken", "username", mockRestClient.Object);

			var result = await client.GetLoansAsync();

			mockRestClient.Verify();
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count());
		}

		[TestMethod]
		public async Task GetAvailableLoansAsync_ReturnsLoans()
		{
			var loans = new List<AvailableLoan>()
			{
				new AvailableLoan()
				{
					Amount = 1000,
					CollateralRequired = false,
					Rate = 40,
					TermInDays = 2,
					Type = "STARTUP"
				}
			};
			var mockRestClient = RestSharpMocks.BuildMockRestClient(HttpStatusCode.OK, "loans", loans);
			var client = new Client("apitoken", "username", mockRestClient.Object);

			var result = await client.GetAvailableLoansAsync();

			mockRestClient.Verify();
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count());
		}

		[TestMethod]
		public async Task TakeOutLoanAsync_ReturnsUser()
		{
			var expectedLoanType = "STARTUP";
			var user = new User()
			{
				Loans = new List<Loan>()
				{
					new Loan()
					{
						Type = expectedLoanType
					}
				}
			};
			var mockRestClient = RestSharpMocks.BuildMockRestClient(HttpStatusCode.OK, "user", user);
			var client = new Client("apitoken", "username", mockRestClient.Object);

			var result = await client.TakeOutLoanAsync("STARTUP");

			mockRestClient.Verify(m => m.ExecuteAsync(It.Is<IRestRequest>(request =>
				request.Parameters[0].Value.Equals(expectedLoanType)
				&& request.Parameters[0].Name.Equals("type"))
				, It.IsAny<CancellationToken>()), Times.Once);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Loans.Count());
		}

		#endregion Loans

		#region Ships

		[TestMethod]
		public async Task GetShipsAsync_ReturnsShips()
		{
			var ships = new List<Ship>()
			{
				new Ship()
				{
					Id = "1"
				}
			};
			var mockRestClient = RestSharpMocks.BuildMockRestClient(HttpStatusCode.OK, "ships", ships);
			var client = new Client("apitoken", "username", mockRestClient.Object);

			var result = await client.GetShipsAsync();

			mockRestClient.Verify();
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count());
		}

		[TestMethod]
		public async Task GetAvailableShipsAsync_ReturnsShips()
		{
			var ships = new List<Ship>()
			{
				new Ship()
				{
					Id = "1"
				}
			};
			var mockRestClient = RestSharpMocks.BuildMockRestClient(HttpStatusCode.OK, "ships", ships);
			var client = new Client("apitoken", "username", mockRestClient.Object);

			var result = await client.GetAvailableShipsAsync();

			mockRestClient.Verify();
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count());
		}

		[TestMethod]
		public async Task GetAvailableShipsAsync_WithClassFilter_ReturnsShips()
		{
			var expectedShipClass = "MK-1";
			var ships = new List<Ship>()
			{
				new Ship()
			};

			var mockRestClient = RestSharpMocks.BuildMockRestClient(HttpStatusCode.OK, "ships", ships);
			var client = new Client("apitoken", "username", mockRestClient.Object);

			var result = await client.GetAvailableShipsAsync(expectedShipClass);

			mockRestClient.Verify(m => m.ExecuteAsync(It.Is<IRestRequest>(request =>
				request.Parameters[0].Value.Equals(expectedShipClass)
				&& request.Parameters[0].Name.Equals("class"))
				, It.IsAny<CancellationToken>()), Times.Once);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count());
		}

		[TestMethod]
		public async Task PurchaseShipAsync_ReturnsUser()
		{
			var expectedLocation = "OE";
			var expectedType = "OE-1";

			var user = new User()
			{
				Ships = new List<Ship>()
				{
					new Ship()
				}
			};
			var mockRestClient = RestSharpMocks.BuildMockRestClient(HttpStatusCode.Created, "user", user);
			var client = new Client("apitoken", "username", mockRestClient.Object);

			var result = await client.PurchaseShipAsync(expectedLocation, expectedType);

			mockRestClient.Verify(m => m.ExecuteAsync(It.Is<IRestRequest>(request =>
				request.Parameters[0].Name.Equals("location")
				&& request.Parameters[0].Value.Equals(expectedLocation)
				&& request.Parameters[1].Name.Equals("type")
				&& request.Parameters[1].Value.Equals(expectedType))
				, It.IsAny<CancellationToken>()), Times.Once);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Ships.Count());
		}

		#endregion Ships
	}
}