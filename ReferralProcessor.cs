using Referral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referral
{
    class ReferralProcessor : IReferralProcessor
    {
        public void ProcessReferral(ReferralContext context, string currentUserId, string referrerId)
        {
            var existingUser = context.Users.SingleOrDefault(u => u.AuthKey == currentUserId);
            if (existingUser != null)
            {
                HandleDeadend();
                return;
            }

            var existingReferralData = context.ReferralData.SingleOrDefault(rd => rd.AuthKey == currentUserId && !string.IsNullOrEmpty(rd.Referrer));
            if (existingReferralData != null)
            {
                HandleDeadend();
                return;
            }

            var newReferralData = context.ReferralData.SingleOrDefault(rd => rd.AuthKey == currentUserId);
            if (newReferralData == null)
            {
                newReferralData = new ReferralData { AuthKey = currentUserId, Referrer = referrerId };
                context.ReferralData.Add(newReferralData);
            }
            else
            {
                newReferralData.Referrer = referrerId;
            }
            context.SaveChanges();

            var referrerData = context.ReferralData.SingleOrDefault(rd => rd.AuthKey == referrerId);
            if (referrerData == null)
            {
                HandleDeadend();
                return;
            }

            var referrals = referrerData.ReferralList;
            if (!referrals.Contains(currentUserId))
            {
                referrals.Add(currentUserId);
                referrerData.ReferralList = referrals;
                context.SaveChanges();
            }
        }

        private static void HandleDeadend()
        {
            Console.WriteLine("Deadend");
        }

    }
   
}
