using System;
using System.Collections.Generic;

namespace FitnessAPI.FitnessDB;

public partial class Branch
{
    public int IdBracnches { get; set; }

    public string NameBranches { get; set; } = null!;

    public string AddressBranches { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
