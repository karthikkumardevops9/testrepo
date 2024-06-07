using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class OneStripJobFieldMap : EntityTypeConfiguration<OneStripJobField>
    {
        public OneStripJobFieldMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("OneStripJobFields");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SetNum).HasColumnName("SetNum");
            this.Property(t => t.OneStripJobsId).HasColumnName("OneStripJobsId");
            this.Property(t => t.FieldName).HasColumnName("FieldName");
            this.Property(t => t.Format).HasColumnName("Format");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.XPos).HasColumnName("XPos");
            this.Property(t => t.YPos).HasColumnName("YPos");
            this.Property(t => t.FontSize).HasColumnName("FontSize");
            this.Property(t => t.FontName).HasColumnName("FontName");
            this.Property(t => t.FontBold).HasColumnName("FontBold");
            this.Property(t => t.FontItalic).HasColumnName("FontItalic");
            this.Property(t => t.FontUnderline).HasColumnName("FontUnderline");
            this.Property(t => t.FontStrikeThru).HasColumnName("FontStrikeThru");
            this.Property(t => t.FontTransparent).HasColumnName("FontTransparent");
            this.Property(t => t.FontOrientation).HasColumnName("FontOrientation");
            this.Property(t => t.Alignment).HasColumnName("Alignment");
            this.Property(t => t.ForeColor).HasColumnName("ForeColor");
            this.Property(t => t.BackColor).HasColumnName("BackColor");
            this.Property(t => t.BCStyle).HasColumnName("BCStyle");
            this.Property(t => t.BCBarWidth).HasColumnName("BCBarWidth");
            this.Property(t => t.BCDirection).HasColumnName("BCDirection");
            this.Property(t => t.BCUPCNotches).HasColumnName("BCUPCNotches");
            this.Property(t => t.BCWidth).HasColumnName("BCWidth");
            this.Property(t => t.BCHeight).HasColumnName("BCHeight");
            this.Property(t => t.Order).HasColumnName("Order");
            this.Property(t => t.StartChar).HasColumnName("StartChar");
            this.Property(t => t.MaxLen).HasColumnName("MaxLen");
            this.Property(t => t.SpecialFunctions).HasColumnName("SpecialFunctions");
        }
    }
}
