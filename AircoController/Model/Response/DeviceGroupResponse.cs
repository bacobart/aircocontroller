using System.Collections.Generic;

namespace AircoController.Model.Response
{
    public class DeviceGroupResponse
    {
        public int GroupCount { get; set; }
        public List<Group> GroupList { get; set; }
    }
}
