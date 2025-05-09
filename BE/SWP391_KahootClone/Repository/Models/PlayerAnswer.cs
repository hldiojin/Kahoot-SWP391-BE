﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class PlayerAnswer
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public int QuestionId { get; set; }

    public DateTime AnsweredAt { get; set; }

    public bool IsCorrect { get; set; }

    public int ResponseTime { get; set; }

    public string Answer { get; set; }

    public virtual Player Player { get; set; }

    public virtual Question Question { get; set; }
}