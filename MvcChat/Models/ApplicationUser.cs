using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MvcChat.Models;

// Add profile data for application users by adding properties to the User class
public class ApplicationUser : IdentityUser
{
    [DataType(DataType.Text), MaxLength(32), Display(Name = "First Name")]
    public string? FirstName { get; set; }
    [DataType(DataType.Text), MaxLength(32), Display(Name = "Last Name")]
    public string? LastName { get; set; }
    public byte[]? ProfilePicture { get; set; }

    [Display(Name = "Created On")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public ICollection<ApplicationUser> Follow { get; set; } = new HashSet<ApplicationUser>();
}

