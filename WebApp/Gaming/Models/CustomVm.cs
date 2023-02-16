using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gaming.Models;

public class CustomVm
{
    [Key] public string Login { get; set; }
    [Required] [DataType(DataType.Password)] public string? Password { get; set; }
    [ReadOnly(true)] public string? Name { get; set; }
    [ReadOnly(true)] public string? Ip { get; set; }
    [ReadOnly(true)] public bool IsStarted { get; set; }
    
}