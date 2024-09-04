using Referral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referral
{
    class ReferralProcessor(ReferralContext context) : IReferralProcessor
    {
        ReferralContext _referralContext = context;

        public bool UserExists(string currentUserId)
        {
            ReferralContext context = _referralContext;
            return context.Users.SingleOrDefault(u => u.AuthKey == currentUserId) != null;
        }

        public bool ReferralDataExists(string currentUserId)
        {
            ReferralContext context = _referralContext;
            return context.ReferralData.SingleOrDefault(rd => rd.AuthKey == currentUserId && !string.IsNullOrEmpty(rd.Referrer)) != null;
        }

        public ReferralData GetOrCreateReferralData(string currentUserId, string referrerId)
        {
            ReferralContext context = _referralContext;
            var referralData = context.ReferralData.SingleOrDefault(rd => rd.AuthKey == currentUserId);
            if (referralData == null)
            {
                referralData = new ReferralData { AuthKey = currentUserId, Referrer = referrerId };
                context.ReferralData.Add(referralData);
            }
            else
            {
                referralData.Referrer = referrerId;
            }
            context.SaveChanges();
            return referralData;
        }

        public void AddReferralIfNotExists(ReferralData referrerData, string currentUserId)
        {
            var referrals = referrerData.ReferralList;
            if (!referrals.Contains(currentUserId))
            {
                referrals.Add(currentUserId);
                referrerData.ReferralList = referrals; 
            }
        }
    }
   
}
