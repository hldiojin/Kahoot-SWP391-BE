﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public string Role { get; set; }

    public string AvatarUrl { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

   

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();

    public virtual ICollection<UserServicePack> UserServicePacks { get; set; } = new List<UserServicePack>();
}