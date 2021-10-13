using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShoppingCartGrpc.Protos;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using static ShoppingCartGrpc.Protos.ShoppingCartProtoService;

namespace ShoppingCartWorkerService
{
    public class Worker : BackgroundService
    {

        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly ShoppingCartProtoServiceClient _shoppingCartClient;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, HttpClient client,
            ShoppingCartProtoServiceClient shoppingCartClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _shoppingCartClient = shoppingCartClient ?? throw new ArgumentNullException(nameof(shoppingCartClient));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Thread.Sleep(2000);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var token = await GetTokenFromIdentityServer();

                var shoppingCart = await GetOrCreateShoppingCartAsync(token);

                Console.WriteLine(shoppingCart);

                await Task.Delay(_configuration.GetValue<int>("WorkerService:TaskInterval"), stoppingToken);
            }
        }

        private async Task<string> GetTokenFromIdentityServer()
        {
            var doc = await _client.GetDiscoveryDocumentAsync(
                _configuration.GetValue<string>("WorkerService:IdentityServerUrl"));

            if (doc.IsError)
                return string.Empty;

            var tokenResponse = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = doc.TokenEndpoint,
                ClientId = "ShoppingCartClient",
                ClientSecret = "secret",
                Scope = "ShoppingCartAPI"
            });

            return tokenResponse.IsError ? string.Empty : tokenResponse.AccessToken;
        }


        private async Task<ShoppingCartModel> GetOrCreateShoppingCartAsync(string token)
        {
            var userName = _configuration.GetValue<string>("WorkerService:UserName");
            ShoppingCartModel shoppingCartModel;
            var headers = new Metadata();
            headers.Add("Authorization", $"Bearer {token}");
            try
            {
                shoppingCartModel = await _shoppingCartClient.GetShoppingCartAsync(new GetShoppingCartRequest { UserName = userName }, headers);

            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.NotFound)
                    shoppingCartModel =
                        await _shoppingCartClient.CreateShoppingCartAsync(new ShoppingCartModel { UserName = userName });
                else
                    throw;
            }


            return shoppingCartModel;
        }
    }
}
