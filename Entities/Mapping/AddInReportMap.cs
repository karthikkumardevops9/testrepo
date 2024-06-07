using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class AddInReportMap : EntityTypeConfiguration<AddInReport>
    {
        public AddInReportMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("AddInReports");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Sequence).HasColumnName("Sequence");
            this.Property(t => t.MenuText).HasColumnName("MenuText");
            this.Property(t => t.CommandLine).HasColumnName("CommandLine");
        }
    }
}
