using Referral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referral
{
    class ReferralProcessor(string currentUserId, string referrerId, Action handleDeadend) : IReferralProcessor
    {
        string _currentUserId = currentUserId; 
        string _referrerId = referrerId;
        Action HandleDeadend = handleDeadend;

        public bool UserExists(ReferralContext context, string currentUserId)
        {
            return context.Users.SingleOrDefault(u => u.AuthKey == currentUserId) != null;
        }

        public bool ReferralDataExists(ReferralContext context, string currentUserId)
        {
            return context.ReferralData.SingleOrDefault(rd => rd.AuthKey == currentUserId && !string.IsNullOrEmpty(rd.Referrer)) != null;
        }

        public ReferralData GetOrCreateReferralData(ReferralContext context, string currentUserId, string referrerId)
        {
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

        public void ProcessReferral(ReferralContext context)
        {
            if (UserExists(context, _currentUserId) || ReferralDataExists(context, _currentUserId))
            {
                HandleDeadend();
                return;
            }

            GetOrCreateReferralData(context, currentUserId, referrerId);

            var referrerData = context.ReferralData.SingleOrDefault(rd => rd.AuthKey == referrerId);
            if (referrerData == null)
            {
                HandleDeadend();
                return;
            }

            AddReferralIfNotExists(referrerData, currentUserId);
            context.SaveChanges();
        }

    }
   
}
