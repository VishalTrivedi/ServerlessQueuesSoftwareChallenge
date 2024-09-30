using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public string SVGData { get; set; }

    public bool IsUserValid => 
        (FirstName is not null
            && LastName is not null);
}

