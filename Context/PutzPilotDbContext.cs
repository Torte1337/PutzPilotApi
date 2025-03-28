using System;
using Microsoft.EntityFrameworkCore;
using PutzPilotApi.Models;
using PutzPilotApi.RequestModels;

namespace PutzPilotApi.Context;

public class PutzPilotDbContext : DbContext
{
    public DbSet<Employee> Employees {get;set;}
    public DbSet<WorkItem> Workitems {get;set;}
    public DbSet<CleaningObject> CleaningObjects {get;set;}
    public DbSet<VacationRequest> VacationRequests {get;set;}


    public PutzPilotDbContext(DbContextOptions<PutzPilotDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
