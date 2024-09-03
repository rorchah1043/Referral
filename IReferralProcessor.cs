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
        bool UserExists(ReferralContext context, string currentUserId);
        bool ReferralDataExists(ReferralContext context, string currentUserId);
        ReferralData GetOrCreateReferralData(ReferralContext context, string currentUserId, string referrerId);
        void AddReferralIfNotExists(ReferralData referrerData, string currentUserId);
    }

}
