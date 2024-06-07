using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLGrabberFunctionMap : EntityTypeConfiguration<SLGrabberFunction>
    {
        public SLGrabberFunctionMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SLGrabberFunctions");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.UseParentWindowClass).HasColumnName("UseParentWindowClass");
            this.Property(t => t.ParentWindowClassName).HasColumnName("ParentWindowClassName");
            this.Property(t => t.UseParentTitleMask).HasColumnName("UseParentTitleMask");
            this.Property(t => t.ParentTitleMask).HasColumnName("ParentTitleMask");
            this.Property(t => t.UseParentSize).HasColumnName("UseParentSize");
            this.Property(t => t.ParentWidth).HasColumnName("ParentWidth");
            this.Property(t => t.ParentHeight).HasColumnName("ParentHeight");
            this.Property(t => t.UseParentPosition).HasColumnName("UseParentPosition");
            this.Property(t => t.ParentLeft).HasColumnName("ParentLeft");
            this.Property(t => t.ParentTop).HasColumnName("ParentTop");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.ViewsId).HasColumnName("ViewsId");
            this.Property(t => t.ChildTableName).HasColumnName("ChildTableName");
            this.Property(t => t.ChildViewsId).HasColumnName("ChildViewsId");
            this.Property(t => t.OneStripJobsId).HasColumnName("OneStripJobsId");
            this.Property(t => t.ShowTree).HasColumnName("ShowTree");
            this.Property(t => t.ShowImageViewer).HasColumnName("ShowImageViewer");
            this.Property(t => t.ShowTrackingViewer).HasColumnName("ShowTrackingViewer");
            this.Property(t => t.DeleteAfterPrint).HasColumnName("DeleteAfterPrint");
            this.Property(t => t.HotKeyAssignment).HasColumnName("HotKeyAssignment");
            this.Property(t => t.CanHotKeyWorkIfNotInFocus).HasColumnName("CanHotKeyWorkIfNotInFocus");
            this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.PrintAfter).HasColumnName("PrintAfter");
            this.Property(t => t.ViewGroup).HasColumnName("ViewGroup");
            this.Property(t => t.Activated).HasColumnName("Activated");
            this.Property(t => t.ParentTitle).HasColumnName("ParentTitle");
        }
    }
}
