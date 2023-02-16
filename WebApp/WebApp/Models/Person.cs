using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

/// <summary>
/// Person model
/// </summary>
public class Person
{
    /// <summary>
    /// Id of the person
    /// </summary>
    [Range(1,100)]
    public int Id { get; set; }
    /// <summary>
    /// First name of the person
    /// </summary>
    [MinLength(3)]
    [MaxLength(30)]
    [DisplayName("First Name")]
    [Required]
    public int FirstName { get; set; }
    /// <summary>
    /// Last name of the person
    /// </summary>
    [MinLength(3)]
    [MaxLength(30)]
    [DisplayName("Last Name")]
    [Required]
    public int LastName { get; set; }
    /// <summary>
    /// Mail of the person
    /// </summary>
    [EmailAddress]
    public string? Mail { get; set; }
}