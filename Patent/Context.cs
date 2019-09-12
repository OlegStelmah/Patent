namespace PatentNS
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    /// <summary>
    /// ����� Context ���������� �������� ������, ������������ ��� �������������� � ��
    /// DbSet ������������ ����� ������ ���������, ���������� � ���� ������, ����� ������� � ���������� �������������� � ��
    /// </summary>
    public partial class Context : DbContext
    {
        public Context()
            : base("name=Database")
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Patent> Patents { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Scheme> Schemes { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<Worker> Workers { get; set; }
    }
}
