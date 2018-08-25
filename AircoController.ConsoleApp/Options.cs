using CommandLine;

namespace AircoController.ConsoleApp
{
    [Verb("control", HelpText = "Controls a device.")]
    public class ControlOptions
    {
        [Option('d', "deviceId", Required = true, HelpText = "Device id to control.")]
        public string DeviceId { get; set; }

        [Option('o', "operate", Required = false, HelpText = "Sets the operate flag (turns device on=1/off=1).")]
        public int? Operate { get; set; }

        [Option('t', "temperature", Required = false, HelpText = "Sets the temperature.")]
        public decimal? Temperature { get; set; }

        [Option('m', "mode", Required = false, HelpText = "Sets the mode (auto=0,dry=1,cool=2,heat=3,fan=4)")]
        public int? OperationMode { get; set; }
    }

    [Verb("status", HelpText = "Returns information about a device.")]
    public class StatusOptions
    {
        [Option('d', "deviceId", Required = true, HelpText = "Device id to control.")]
        public string DeviceId { get; set; }
    }

    [Verb("list", HelpText = "Lists all devices.")]
    public class ListOptions
    {
    }

    [Verb("login", HelpText = "Logs in to the service.")]
    public class LoginOptions
    {
        [Option('l', "loginId", Required = true, HelpText = "Login Id.")]
        public string LoginId { get; set; }

        [Option('p', "password", Required = true, HelpText = "Password.")]
        public string Password { get; set; }
    }
}
