using Referral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referral
{
    interface IReferralProcessor
    {
        bool UserExists(string currentUserId);
        bool ReferralDataExists(string currentUserId);
        ReferralData GetOrCreateReferralData(string currentUserId, string referrerId);
        void AddReferralIfNotExists(ReferralData referrerData, string currentUserId);
    }

}
