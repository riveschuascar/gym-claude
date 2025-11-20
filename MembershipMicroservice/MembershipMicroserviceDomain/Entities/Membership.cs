using System;
using System.Collections.Generic;
using MembershipMicroservice.MembershipMicroserviceDomain.Entities;

public class Membership
{
    public int? Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastModification { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public int? MonthlySessions { get; set; }

    public List<int> DisciplineIds { get; set; } = new();
    public List<Discipline>? Disciplines { get; set; } = new();
}
