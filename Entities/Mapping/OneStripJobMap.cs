using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class OneStripJobMap : EntityTypeConfiguration<OneStripJob>
    {
        public OneStripJobMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("OneStripJobs");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.Inprint).HasColumnName("Inprint");
            this.Property(t => t.OneStripFormsId).HasColumnName("OneStripFormsId");
            this.Property(t => t.UserUnits).HasColumnName("UserUnits");
            this.Property(t => t.LabelWidth).HasColumnName("LabelWidth");
            this.Property(t => t.LabelHeight).HasColumnName("LabelHeight");
            this.Property(t => t.DrawLabels).HasColumnName("DrawLabels");
            this.Property(t => t.LastCounter).HasColumnName("LastCounter");
            this.Property(t => t.SQLString).HasColumnName("SQLString");
            this.Property(t => t.SQLUpdateString).HasColumnName("SQLUpdateString");
            this.Property(t => t.LSAfterPrinting).HasColumnName("LSAfterPrinting");
            this.Property(t => t.DatabaseName).HasColumnName("DatabaseName");
            this.Property(t => t.DatabaseTableName).HasColumnName("DatabaseTableName");
            this.Property(t => t.DZNName).HasColumnName("DZNName");
            this.Property(t => t.Sampling).HasColumnName("Sampling");
        }
    }
}
