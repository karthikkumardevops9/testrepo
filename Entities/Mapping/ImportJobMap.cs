using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class ImportJobMap : EntityTypeConfiguration<ImportJob>
    {
        public ImportJobMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);
            
            // Table & Column Mappings
            this.ToTable("ImportJobs");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.InputFile).HasColumnName("InputFile");
            this.Property(t => t.JobName).HasColumnName("JobName");
            this.Property(t => t.ReadOrder).HasColumnName("ReadOrder");
            this.Property(t => t.LoadName).HasColumnName("LoadName");
            this.Property(t => t.UseLoadInput).HasColumnName("UseLoadInput");
            this.Property(t => t.TempInputFile).HasColumnName("TempInputFile");
        }
    }
}
