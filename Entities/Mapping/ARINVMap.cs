using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class ARINVMap : EntityTypeConfiguration<ARINV>
    {
        public ARINVMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.InvNum)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("ARINV");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.InvNum).HasColumnName("InvNum");
            this.Property(t => t.InvDate).HasColumnName("InvDate");
            this.Property(t => t.CustNum).HasColumnName("CustNum");
            this.Property(t => t.CustName).HasColumnName("CustName");
            this.Property(t => t.CustPo).HasColumnName("CustPo");
        }
    }
}
