using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLIndexWizardColMap : EntityTypeConfiguration<SLIndexWizardCol>
    {
        public SLIndexWizardColMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLIndexWizardCols");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ColumnNum).HasColumnName("ColumnNum");
            this.Property(t => t.SLIndexWizardId).HasColumnName("SLIndexWizardId");
            this.Property(t => t.ColumnType).HasColumnName("ColumnType");
            this.Property(t => t.Prompt).HasColumnName("Prompt");
            this.Property(t => t.Field).HasColumnName("Field");
            this.Property(t => t.Required).HasColumnName("Required");
            this.Property(t => t.Visible).HasColumnName("Visible");
            this.Property(t => t.FixedData).HasColumnName("FixedData");
        }
    }
}
