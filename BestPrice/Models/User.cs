namespace BestPrice.Models
{
    public class User
    {
            public int UserID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public int Active { get; set; }
            public int CreatedBy { get; set; }
            public int UpdatedBy { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime DateLastUpdated { get; set; }
        }
    }
