using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class LinkScriptMap : EntityTypeConfiguration<LinkScript>
    {
        public LinkScriptMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ScriptName, t.ScriptSequence });

            // Properties
            this.Property(t => t.ScriptName)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("LinkScript");
            this.Property(t => t.ScriptName).HasColumnName("ScriptName");
            this.Property(t => t.ScriptSequence).HasColumnName("ScriptSequence");
            this.Property(t => t.Command).HasColumnName("Command");
            this.Property(t => t.ArgOne).HasColumnName("ArgOne");
            this.Property(t => t.ArgTwo).HasColumnName("ArgTwo");
            this.Property(t => t.ArgThree).HasColumnName("ArgThree");
            this.Property(t => t.Result).HasColumnName("Result");
            this.Property(t => t.Comment).HasColumnName("Comment");
        }
    }
}
