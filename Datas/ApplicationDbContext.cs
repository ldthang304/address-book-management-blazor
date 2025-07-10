using AddressBookManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace AddressBookManagement.Datas
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Master> Masters { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<TodoTask> Tasks { get; set; }
        public DbSet<Website> Websites { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //One Contact has many Phones
            modelBuilder.Entity<Phone>()
                .HasOne(p => p.Contact)
                .WithMany(c => c.Phones)
                .HasForeignKey(p => p.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            //One Contact has many Websites
            modelBuilder.Entity<Website>()
                .HasOne(w => w.Contact)
                .WithMany(c => c.Websites)
                .HasForeignKey(w => w.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            //One Contact has many Tasks
            modelBuilder.Entity<Models.TodoTask>()
                .HasOne(t => t.Contact)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            //One Contact has many Notes
            modelBuilder.Entity<Note>()
                .HasOne(n => n.Contact)
                .WithMany(c => c.Notes)
                .HasForeignKey(n => n.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            //One Organization has many Contact
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Organization)
                .WithMany(o => o.Contacts)
                .HasForeignKey(c => c.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
