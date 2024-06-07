using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class ViewColumnMap : EntityTypeConfiguration<ViewColumn>
    {
        public ViewColumnMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("ViewColumns");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ViewsId).HasColumnName("ViewsId");
            this.Property(t => t.ColumnNum).HasColumnName("ColumnNum");
            this.Property(t => t.FieldName).HasColumnName("FieldName");
            this.Property(t => t.Heading).HasColumnName("Heading");
            this.Property(t => t.LookupType).HasColumnName("LookupType");
            this.Property(t => t.ColumnWidth).HasColumnName("ColumnWidth");
            this.Property(t => t.ColumnVisible).HasColumnName("ColumnVisible");
            this.Property(t => t.ColumnOrder).HasColumnName("ColumnOrder");
            this.Property(t => t.ColumnStyle).HasColumnName("ColumnStyle");
            this.Property(t => t.EditMask).HasColumnName("EditMask");
            this.Property(t => t.Picture).HasColumnName("Picture");
            this.Property(t => t.LookupIdCol).HasColumnName("LookupIdCol");
            this.Property(t => t.SortField).HasColumnName("SortField");
            this.Property(t => t.SortableField).HasColumnName("SortableField");
            this.Property(t => t.FilterField).HasColumnName("FilterField");
            this.Property(t => t.CountColumn).HasColumnName("CountColumn");
            this.Property(t => t.SubtotalColumn).HasColumnName("SubtotalColumn");
            this.Property(t => t.PrintColumnAsSubheader).HasColumnName("PrintColumnAsSubheader");
            this.Property(t => t.RestartPageNumber).HasColumnName("RestartPageNumber");
            this.Property(t => t.UseAsPrintId).HasColumnName("UseAsPrintId");
            this.Property(t => t.DropDownSuggestionOnly).HasColumnName("DropDownSuggestionOnly");
            this.Property(t => t.SuppressPrinting).HasColumnName("SuppressPrinting");
            this.Property(t => t.ValueCount).HasColumnName("ValueCount");
            this.Property(t => t.AlternateFieldName).HasColumnName("AlternateFieldName");
            this.Property(t => t.DefaultLookupValue).HasColumnName("DefaultLookupValue");
            this.Property(t => t.DropDownFilterIdField).HasColumnName("DropDownFilterIdField");
            this.Property(t => t.DropDownFilterMatchField).HasColumnName("DropDownFilterMatchField");
            this.Property(t => t.DropDownFlag).HasColumnName("DropDownFlag");
            this.Property(t => t.DropDownReferenceColNum).HasColumnName("DropDownReferenceColNum");
            this.Property(t => t.DropDownReferenceValue).HasColumnName("DropDownReferenceValue");
            this.Property(t => t.DropDownTargetField).HasColumnName("DropDownTargetField");
            this.Property(t => t.EditAllowed).HasColumnName("EditAllowed");
            this.Property(t => t.FormColWidth).HasColumnName("FormColWidth");
            this.Property(t => t.FreezeOrder).HasColumnName("FreezeOrder");
            this.Property(t => t.InputMask).HasColumnName("InputMask");
            this.Property(t => t.MaskClipMode).HasColumnName("MaskClipMode");
            this.Property(t => t.MaskInclude).HasColumnName("MaskInclude");
            this.Property(t => t.MaskPromptChar).HasColumnName("MaskPromptChar");
            this.Property(t => t.MaxPrintLines).HasColumnName("MaxPrintLines");
            this.Property(t => t.PageBreakField).HasColumnName("PageBreakField");
            this.Property(t => t.PrinterColWidth).HasColumnName("PrinterColWidth");
            this.Property(t => t.SortOrder).HasColumnName("SortOrder");
            this.Property(t => t.SortOrderDesc).HasColumnName("SortOrderDesc");
            this.Property(t => t.SuppressDuplicates).HasColumnName("SuppressDuplicates");
            this.Property(t => t.VisibleOnForm).HasColumnName("VisibleOnForm");
            this.Property(t => t.VisibleOnPrint).HasColumnName("VisibleOnPrint");
            this.Property(t => t.AlternateSortColumn).HasColumnName("AlternateSortColumn");
            this.Property(t => t.LabelLeft).HasColumnName("LabelLeft");
            this.Property(t => t.LabelTop).HasColumnName("LabelTop");
            this.Property(t => t.LabelWidth).HasColumnName("LabelWidth");
            this.Property(t => t.LabelHeight).HasColumnName("LabelHeight");
            this.Property(t => t.ControlLeft).HasColumnName("ControlLeft");
            this.Property(t => t.ControlTop).HasColumnName("ControlTop");
            this.Property(t => t.ControlWidth).HasColumnName("ControlWidth");
            this.Property(t => t.ControlHeight).HasColumnName("ControlHeight");
            this.Property(t => t.TabOrder).HasColumnName("TabOrder");
            this.Property(t => t.LabelJustify).HasColumnName("LabelJustify");
        }
    }
}
