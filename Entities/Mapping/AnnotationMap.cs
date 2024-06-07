using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class AnnotationMap : EntityTypeConfiguration<Annotation>
    {
        public AnnotationMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("Annotations");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.Table).HasColumnName("Table");
            this.Property(t => t.Annotation1).HasColumnName("Annotation");
            this.Property(t => t.DeskOf).HasColumnName("DeskOf");
            this.Property(t => t.NoteDateTime).HasColumnName("NoteDateTime");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.NewAnnotation).HasColumnName("NewAnnotation");
            this.Property(t => t.NewAnnotationComplete).HasColumnName("NewAnnotationComplete");
        }
    }
}
