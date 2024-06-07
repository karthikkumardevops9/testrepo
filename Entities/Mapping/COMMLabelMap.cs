using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class COMMLabelMap : EntityTypeConfiguration<COMMLabel>
    {
        public COMMLabelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("COMMLabels");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.OneStripJobsId).HasColumnName("OneStripJobsId");
            this.Property(t => t.LabelStartString).HasColumnName("LabelStartString");
            this.Property(t => t.LabelEndString).HasColumnName("LabelEndString");
            this.Property(t => t.LowercaseChars).HasColumnName("LowercaseChars");
            this.Property(t => t.RemovedChars).HasColumnName("RemovedChars");
            this.Property(t => t.OutputType).HasColumnName("OutputType");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.NoTimeOut).HasColumnName("NoTimeOut");
            this.Property(t => t.TimeOut).HasColumnName("TimeOut");
        }
    }
}
