﻿using MerchantExpanse.SpaceTraders.Contracts;
using MerchantExpanse.SpaceTraders.Extensions;
using MerchantExpanse.SpaceTraders.Models;
using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerchantExpanse.SpaceTraders
{
	public class Client : IClient
	{
		private string Username { get; set; }
		private IRestClient RestClient { get; set; }

		public Client(string apiToken, string username, IRestClient restClient)
		{
			Username = username;
			RestClient = restClient;

			RestClient.Authenticator = new JwtAuthenticator(apiToken);
		}

		public async Task<User> GetUserAsync()
		{
			var request = new RestRequest($"users/{Username}");
			var response = await RestClient.ExecuteAsync(request);

			return response.DeserializeContent<User>("user");
		}

		#region Loans

		public async Task<IEnumerable<Loan>> GetLoansAsync()
		{
			var request = new RestRequest($"users/{Username}/loans");
			var response = await RestClient.ExecuteAsync(request);

			return response.DeserializeContent<IEnumerable<Loan>>("loans"); ;
		}

		public async Task<IEnumerable<AvailableLoan>> GetAvailableLoansAsync()
		{
			var request = new RestRequest("game/loans");
			var response = await RestClient.ExecuteAsync(request);

			return response.DeserializeContent<IEnumerable<AvailableLoan>>("loans");
		}

		public async Task<User> TakeOutLoanAsync(string type)
		{
			var request = new RestRequest($"users/{Username}/loans", Method.POST);
			request.AddParameter("type", type);
			var response = await RestClient.ExecuteAsync(request);

			return response.DeserializeContent<User>("user");
		}

		#endregion Loans

		#region Ships

		public async Task<IEnumerable<Ship>> GetShipsAsync()
		{
			var request = new RestRequest($"users/{Username}/ships");
			var response = await RestClient.ExecuteAsync(request);

			return response.DeserializeContent<IEnumerable<Ship>>("ships");
		}

		public async Task<IEnumerable<AvailableShip>> GetAvailableShipsAsync(string shipClass = null)
		{
			var request = new RestRequest("game/ships");
			if (shipClass != null)
			{
				request.AddParameter("class", shipClass);
			}
			var response = await RestClient.ExecuteAsync(request);

			return response.DeserializeContent<IEnumerable<AvailableShip>>("ships");
		}

		public async Task<User> PurchaseShipAsync(string location, string type)
		{
			var request = new RestRequest($"users/{Username}/ships", Method.POST);
			request.AddParameter("location", location);
			request.AddParameter("type", type);

			var response = await RestClient.ExecuteAsync(request);

			return response.DeserializeContent<User>("user");
		}

		#endregion Ships
	}
}