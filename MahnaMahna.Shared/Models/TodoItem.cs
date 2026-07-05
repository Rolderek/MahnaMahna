namespace MahnaMahna.Shared.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


public class TodoItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Text { get; set; }

    [Required]
    public TodoItemState State { get; set; }

    public ICollection<Category> Categories { get; set; }
}
