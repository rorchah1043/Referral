using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Referral.Models
{
    public class ReferralData
    {

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string AuthKey { get; set; }

        [Column("data", TypeName = "jsonb")]
        public string Referrals { get; set; } = "[]";

        public string Referrer { get; set; }

        [NotMapped]
        public List<string> ReferralList
        {
            get
            {
                return JsonConvert.DeserializeObject<List<string>>(this.Referrals);
            }
            set
            {
                this.Referrals = JsonConvert.SerializeObject(value);
            }
        }
    }
}