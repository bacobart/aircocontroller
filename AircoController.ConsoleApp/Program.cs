using System;
using System.Threading.Tasks;
using CommandLine;
using AircoController.Model;
using Hanssens.Net;

namespace AircoController.ConsoleApp
{
    class Program
    {
        private static readonly AircoManager _manager = new AircoManager();
        private static readonly LocalStorage _localStorage = new LocalStorage();
        public static async Task<int> Main(string[] args)
        {
            var parser = new Parser(with => { with.CaseInsensitiveEnumValues = true; with.HelpWriter = Console.Out; });

            return await parser.ParseArguments<ControlOptions, ListOptions, StatusOptions, LoginOptions>(args)
                               .MapResult(
                                    async (ControlOptions opts) => await Control(opts),
                                    async (ListOptions opts) => await List(opts),
                                    async (StatusOptions opts) => await Status(opts),
                                    async (LoginOptions opts) => await Login(opts),
                                    errs => Task.FromResult(1)
                                );
        }

        static async Task<int> Control(ControlOptions options)
        {
            await ValidateToken();

            await _manager.ControlDevice(options.DeviceId, 
                operate: options.Operate != null ? (OperateType)options.Operate : (OperateType?)null,
                temperature: options.Temperature,
                operationMode: options.OperationMode != null ? (OperationModeType)options.OperationMode : (OperationModeType?)null);

            return 0;
        }

        static async Task<int> List(ListOptions options)
        {
            await ValidateToken();

            var groups = await _manager.GetDeviceGroups();

            foreach (var group in groups.GroupList)
            {
                Console.WriteLine($"Group: {group.GroupName}");

                foreach (var x in group.DeviceIdList)
                {
                    Console.WriteLine($"Device: {x.DeviceName} => {x.DeviceGuid}");
                }
            }

            return 0;
        }

        static async Task<int> Status(StatusOptions options)
        {
            await ValidateToken();

            var status = await _manager.GetDeviceStatus(options.DeviceId);

            Console.WriteLine($"OperationMode: {status.Parameters.OperationMode.ToString()}");
            Console.WriteLine($"InsideTemperature: {status.Parameters.InsideTemperature}");
            Console.WriteLine($"OutTemperature: {status.Parameters.OutTemperature}");
            Console.WriteLine($"TemperatureSet: {status.Parameters.TemperatureSet}");

            return 0;
        }

        static async Task<int> Login(LoginOptions options)
        {
            var response = await _manager.Login("0", options.LoginId, options.Password);

            Console.WriteLine($"Login succesful. Storing token {response.UToken}");

            SetToken(response.UToken);

            return 0;
        }

        static Task<int> ValidateToken()
        {
            var token = GetToken();

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("No token found, use login command first.");
                Environment.Exit(-1);
            }

            _manager.SetAuthorizationToken(token);

            return Task.FromResult(0);
        }

        static string GetToken()
        {
            if (_localStorage.Count == 0)
                return null;

            return _localStorage.Get<string>("token");
        }

        static void SetToken(string token)
        {
            _localStorage.Store("token", token);
            _localStorage.Persist();
        }
    }
}
