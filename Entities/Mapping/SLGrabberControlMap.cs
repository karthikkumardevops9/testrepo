using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLGrabberControlMap : EntityTypeConfiguration<SLGrabberControl>
    {
        public SLGrabberControlMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SLGrabberControls");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SLGrabberFunctionsId).HasColumnName("SLGrabberFunctionsId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.UseControlId).HasColumnName("UseControlId");
            this.Property(t => t.ControlId).HasColumnName("ControlId");
            this.Property(t => t.UseClassName).HasColumnName("UseClassName");
            this.Property(t => t.ClassName).HasColumnName("ClassName");
            this.Property(t => t.UseSize).HasColumnName("UseSize");
            this.Property(t => t.Width).HasColumnName("Width");
            this.Property(t => t.Height).HasColumnName("Height");
            this.Property(t => t.UsePosition).HasColumnName("UsePosition");
            this.Property(t => t.Left).HasColumnName("Left");
            this.Property(t => t.Top).HasColumnName("Top");
        }
    }
}
