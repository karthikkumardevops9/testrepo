using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class ViewFilterMap : EntityTypeConfiguration<ViewFilter>
    {
        public ViewFilterMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("ViewFilters");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Sequence).HasColumnName("Sequence");
            this.Property(t => t.ViewsId).HasColumnName("ViewsId");
            this.Property(t => t.ColumnNum).HasColumnName("ColumnNum");
            this.Property(t => t.OpenParen).HasColumnName("OpenParen");
            this.Property(t => t.Operator).HasColumnName("Operator");
            this.Property(t => t.FilterData).HasColumnName("FilterData");
            this.Property(t => t.CloseParen).HasColumnName("CloseParen");
            this.Property(t => t.JoinOperator).HasColumnName("JoinOperator");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.DisplayColumnNum).HasColumnName("DisplayColumnNum");
            this.Property(t => t.PartOfView).HasColumnName("PartOfView");
        }
    }
}
