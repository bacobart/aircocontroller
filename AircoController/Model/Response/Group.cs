using System.Collections.Generic;

namespace AircoController.Model.Response
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<DeviceId> DeviceIdList { get; set; }
    }
}
