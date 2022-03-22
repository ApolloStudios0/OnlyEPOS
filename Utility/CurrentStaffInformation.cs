using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlyEPOS.Utility
{
    internal class CurrentStaffInformation
    {
        // This class holds all of the inforamation related to the currently logged in staff member.
        // It is edited when a staff members logs in.

        public static string StaffMemberName { get; set; }
        
        public static string StaffUUID { get; set; }
        
        public static string StaffProfilePicture { get; set; }
        
        public static string StaffColor { get; set; }
        
    }
}
