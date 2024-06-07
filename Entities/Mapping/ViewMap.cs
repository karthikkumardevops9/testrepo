using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class ViewMap : EntityTypeConfiguration<View>
    {
        public ViewMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("Views");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.ViewName).HasColumnName("ViewName");
            this.Property(t => t.SQLStatement).HasColumnName("SQLStatement");
            this.Property(t => t.MaxRecsPerFetch).HasColumnName("MaxRecsPerFetch");
            this.Property(t => t.Picture).HasColumnName("Picture");
            this.Property(t => t.ReportStylesId).HasColumnName("ReportStylesId");
            this.Property(t => t.ViewOrder).HasColumnName("ViewOrder");
            this.Property(t => t.WorkFlow1).HasColumnName("WorkFlow1");
            this.Property(t => t.WorkFlow1Pic).HasColumnName("WorkFlow1Pic");
            this.Property(t => t.WorkFlowDesc1).HasColumnName("WorkFlowDesc1");
            this.Property(t => t.WorkFlowToolTip1).HasColumnName("WorkFlowToolTip1");
            this.Property(t => t.WorkFlowHotKey1).HasColumnName("WorkFlowHotKey1");
            this.Property(t => t.WorkFlow2).HasColumnName("WorkFlow2");
            this.Property(t => t.WorkFlow2Pic).HasColumnName("WorkFlow2Pic");
            this.Property(t => t.WorkFlowDesc2).HasColumnName("WorkFlowDesc2");
            this.Property(t => t.WorkFlowToolTip2).HasColumnName("WorkFlowToolTip2");
            this.Property(t => t.WorkFlowHotKey2).HasColumnName("WorkFlowHotKey2");
            this.Property(t => t.WorkFlow3).HasColumnName("WorkFlow3");
            this.Property(t => t.WorkFlow3Pic).HasColumnName("WorkFlow3Pic");
            this.Property(t => t.WorkFlowDesc3).HasColumnName("WorkFlowDesc3");
            this.Property(t => t.WorkFlowToolTip3).HasColumnName("WorkFlowToolTip3");
            this.Property(t => t.WorkFlowHotKey3).HasColumnName("WorkFlowHotKey3");
            this.Property(t => t.WorkFlow4).HasColumnName("WorkFlow4");
            this.Property(t => t.WorkFlow4Pic).HasColumnName("WorkFlow4Pic");
            this.Property(t => t.WorkFlowDesc4).HasColumnName("WorkFlowDesc4");
            this.Property(t => t.WorkFlowToolTip4).HasColumnName("WorkFlowToolTip4");
            this.Property(t => t.WorkFlowHotKey4).HasColumnName("WorkFlowHotKey4");
            this.Property(t => t.WorkFlow5).HasColumnName("WorkFlow5");
            this.Property(t => t.WorkFlow5Pic).HasColumnName("WorkFlow5Pic");
            this.Property(t => t.WorkFlowDesc5).HasColumnName("WorkFlowDesc5");
            this.Property(t => t.WorkFlowToolTip5).HasColumnName("WorkFlowToolTip5");
            this.Property(t => t.WorkFlowHotKey5).HasColumnName("WorkFlowHotKey5");
            this.Property(t => t.TablesId).HasColumnName("TablesId");
            this.Property(t => t.ViewGroup).HasColumnName("ViewGroup");
            this.Property(t => t.Visible).HasColumnName("Visible");
            this.Property(t => t.VariableColWidth).HasColumnName("VariableColWidth");
            this.Property(t => t.VariableRowHeight).HasColumnName("VariableRowHeight");
            this.Property(t => t.VariableFixedCols).HasColumnName("VariableFixedCols");
            this.Property(t => t.RowHeight).HasColumnName("RowHeight");
            this.Property(t => t.AddAllowed).HasColumnName("AddAllowed");
            this.Property(t => t.ViewType).HasColumnName("ViewType");
            this.Property(t => t.UseExactRowCount).HasColumnName("UseExactRowCount");
            this.Property(t => t.TablesDown).HasColumnName("TablesDown");
            this.Property(t => t.Printable).HasColumnName("Printable");
            this.Property(t => t.GrandTotal).HasColumnName("GrandTotal");
            this.Property(t => t.LeftIndent).HasColumnName("LeftIndent");
            this.Property(t => t.RightIndent).HasColumnName("RightIndent");
            this.Property(t => t.SubTableName).HasColumnName("SubTableName");
            this.Property(t => t.SubViewId).HasColumnName("SubViewId");
            this.Property(t => t.PrintWithoutChildren).HasColumnName("PrintWithoutChildren");
            this.Property(t => t.SuppressHeader).HasColumnName("SuppressHeader");
            this.Property(t => t.SuppressFooter).HasColumnName("SuppressFooter");
            this.Property(t => t.PrintFrozenOnly).HasColumnName("PrintFrozenOnly");
            this.Property(t => t.TrackingEverContained).HasColumnName("TrackingEverContained");
            this.Property(t => t.PrintImages).HasColumnName("PrintImages");
            this.Property(t => t.PrintImageFullPage).HasColumnName("PrintImageFullPage");
            this.Property(t => t.PrintImageFirstPageOnly).HasColumnName("PrintImageFirstPageOnly");
            this.Property(t => t.PrintImageRedlining).HasColumnName("PrintImageRedlining");
            this.Property(t => t.PrintImageLeftMargin).HasColumnName("PrintImageLeftMargin");
            this.Property(t => t.PrintImageRightMargin).HasColumnName("PrintImageRightMargin");
            this.Property(t => t.PrintImageAllVersions).HasColumnName("PrintImageAllVersions");
            this.Property(t => t.ChildColumnHeaders).HasColumnName("ChildColumnHeaders");
            this.Property(t => t.SuppressImageDataRow).HasColumnName("SuppressImageDataRow");
            this.Property(t => t.SuppressImageFooter).HasColumnName("SuppressImageFooter");
            this.Property(t => t.DisplayMode).HasColumnName("DisplayMode");
            this.Property(t => t.AutoRotateImage).HasColumnName("AutoRotateImage");
            this.Property(t => t.GrandTotalOnSepPage).HasColumnName("GrandTotalOnSepPage");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.IncludeFileRoomOrder).HasColumnName("IncludeFileRoomOrder");
            this.Property(t => t.AltViewId).HasColumnName("AltViewId");
            this.Property(t => t.DeleteGridAvail).HasColumnName("DeleteGridAvail");
            this.Property(t => t.FiltersActive).HasColumnName("FiltersActive");
            this.Property(t => t.IncludeTrackingLocation).HasColumnName("IncludeTrackingLocation");
            this.Property(t => t.InTaskList).HasColumnName("InTaskList");
            this.Property(t => t.TaskListDisplayString).HasColumnName("TaskListDisplayString");
            this.Property(t => t.PrintAttachments).HasColumnName("PrintAttachments");
            this.Property(t => t.MultiParent).HasColumnName("MultiParent");
            this.Property(t => t.SearchableView).HasColumnName("SearchableView");
            this.Property(t => t.CustomFormView).HasColumnName("CustomFormView");
        }
    }
}
