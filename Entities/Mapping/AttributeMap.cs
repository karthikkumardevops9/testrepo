using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class AttributeMap : EntityTypeConfiguration<Attribute>
    {
        public AttributeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("Attributes");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.HeadBackColor1).HasColumnName("HeadBackColor1");
            this.Property(t => t.HeadBackColor2).HasColumnName("HeadBackColor2");
            this.Property(t => t.HeadBackColor3).HasColumnName("HeadBackColor3");
            this.Property(t => t.HeadForeColor1).HasColumnName("HeadForeColor1");
            this.Property(t => t.HeadForeColor2).HasColumnName("HeadForeColor2");
            this.Property(t => t.HeadForeColor3).HasColumnName("HeadForeColor3");
            this.Property(t => t.InactiveBackColor1).HasColumnName("InactiveBackColor1");
            this.Property(t => t.InactiveBackColor2).HasColumnName("InactiveBackColor2");
            this.Property(t => t.InactiveBackColor3).HasColumnName("InactiveBackColor3");
            this.Property(t => t.InactiveForeColor1).HasColumnName("InactiveForeColor1");
            this.Property(t => t.InactiveForeColor2).HasColumnName("InactiveForeColor2");
            this.Property(t => t.InactiveForeColor3).HasColumnName("InactiveForeColor3");
            this.Property(t => t.GridBackColor1).HasColumnName("GridBackColor1");
            this.Property(t => t.GridBackColor2).HasColumnName("GridBackColor2");
            this.Property(t => t.GridBackColor3).HasColumnName("GridBackColor3");
            this.Property(t => t.GridForeColor1).HasColumnName("GridForeColor1");
            this.Property(t => t.GridForeColor2).HasColumnName("GridForeColor2");
            this.Property(t => t.GridForeColor3).HasColumnName("GridForeColor3");
            this.Property(t => t.TabBackColor1).HasColumnName("TabBackColor1");
            this.Property(t => t.TabBackColor2).HasColumnName("TabBackColor2");
            this.Property(t => t.TabBackColor3).HasColumnName("TabBackColor3");
            this.Property(t => t.TabForeColor1).HasColumnName("TabForeColor1");
            this.Property(t => t.TabForeColor2).HasColumnName("TabForeColor2");
            this.Property(t => t.TabForeColor3).HasColumnName("TabForeColor3");
            this.Property(t => t.SortFieldBackColor1).HasColumnName("SortFieldBackColor1");
            this.Property(t => t.SortFieldBackColor2).HasColumnName("SortFieldBackColor2");
            this.Property(t => t.SortFieldBackColor3).HasColumnName("SortFieldBackColor3");
            this.Property(t => t.SortFieldForeColor1).HasColumnName("SortFieldForeColor1");
            this.Property(t => t.SortFieldForeColor2).HasColumnName("SortFieldForeColor2");
            this.Property(t => t.SortFieldForeColor3).HasColumnName("SortFieldForeColor3");
            this.Property(t => t.ColorName).HasColumnName("ColorName");
        }
    }
}
