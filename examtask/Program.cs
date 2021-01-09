using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp11
{

    class Program
    {
        public class City
        {
            public int Id { get; set; }                        // number
            public string Name { get; set; }                   // max length 128
            public int Latitude { get; set; }                  // number
            public int Longitude { get; set; }                 // number
            public ICollection<Employe> Employes { get; set; } // has many Employes

        }
        public class Doctor : Employe
        {
            public string Spesialisation { get; set; } // required
        }
        public class Employe
        {
            public int Id { get; set; }         // number
            public string Name { get; set; }    // max length 128
            public City City { get; set; }      // has one city
            public int CityId { get; set; }     // foreign key
        }
        public class Enginier : Employe
        {
            public string FavoriteVideogame { get; set; } // fav game not required
        }
      public  class FluentContext : DbContext
        {
            public DbSet<Employe> Employes { get; set; }
            public DbSet<Enginier> Enginiers { get; set; }
            public DbSet<Doctor> Doctors { get; set; }
            public DbSet<City> Cities { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                // employe
                modelBuilder.Entity<Employe>().HasKey(x => x.Id);

                modelBuilder.Entity<Employe>().Property(p => p.Name).HasMaxLength(120);

                modelBuilder.Entity<Employe>()
                    .HasRequired(e => e.City)
                    .WithMany(c => c.Employes)
                    .HasForeignKey(e => e.CityId);

                modelBuilder.Entity<Doctor>().Property(p => p.Spesialisation).IsRequired();


                modelBuilder.Entity<Employe>().ToTable("Employes");

                // city
                modelBuilder.Entity<City>().HasKey(x => x.Id);

                modelBuilder.Entity<City>().ToTable("Cities");

                base.OnModelCreating(modelBuilder);
            }
        }
        public static List<string> GetDocsSpecByCytyID(int id)
        {
            List<string> specs = new List<string>();

            using (var ctx = new FluentContext())
            {

                var doctors = from d in ctx.Doctors
                              join c in ctx.Cities on d.CityId equals c.Id
                              where c.Id == id
                              select new { Spesialisation = d.Spesialisation };

                if (doctors.Count() > 0)
                {
                    foreach (var d in doctors)
                    {
                        specs.Add(d.Spesialisation);
                    }
                    return specs;
                }
                return null;
            }
        }
        static void Main(string[] args)
        {
            int id = Convert.ToInt32(Console.ReadLine());

            var s = GetDocsSpecByCytyID(id);

            if (s == null)
            {
                Console.WriteLine("empty");
            }
            Console.WriteLine(s);
            Console.ReadLine();
        }
    }
}
