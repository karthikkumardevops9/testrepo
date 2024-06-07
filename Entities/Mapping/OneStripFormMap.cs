using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class OneStripFormMap : EntityTypeConfiguration<OneStripForm>
    {
        public OneStripFormMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("OneStripForms");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Inprint).HasColumnName("Inprint");
            this.Property(t => t.LabelOffsetX).HasColumnName("LabelOffsetX");
            this.Property(t => t.LabelOffsetY).HasColumnName("LabelOffsetY");
            this.Property(t => t.LabelWidth).HasColumnName("LabelWidth");
            this.Property(t => t.LabelHeight).HasColumnName("LabelHeight");
            this.Property(t => t.LabelWidthEdgeToEdge).HasColumnName("LabelWidthEdgeToEdge");
            this.Property(t => t.LabelHeightEdgeToEdge).HasColumnName("LabelHeightEdgeToEdge");
            this.Property(t => t.LabelsAcross).HasColumnName("LabelsAcross");
            this.Property(t => t.LabelsDown).HasColumnName("LabelsDown");
            this.Property(t => t.PageWidth).HasColumnName("PageWidth");
            this.Property(t => t.PageHeight).HasColumnName("PageHeight");
            this.Property(t => t.WindowsPrinterSetting).HasColumnName("WindowsPrinterSetting");
            this.Property(t => t.ColorPalette).HasColumnName("ColorPalette");
            this.Property(t => t.MultiDefinitionFile).HasColumnName("MultiDefinitionFile");
            this.Property(t => t.LabelOffsetColumn).HasColumnName("LabelOffsetColumn");
            this.Property(t => t.LabelOffsetRow).HasColumnName("LabelOffsetRow");
            this.Property(t => t.PrintLabelsTopToBottom).HasColumnName("PrintLabelsTopToBottom");
            this.Property(t => t.PrintLabelsLandscape).HasColumnName("PrintLabelsLandscape");
            this.Property(t => t.Label2Printable).HasColumnName("Label2Printable");
            this.Property(t => t.LabelOffsetX2).HasColumnName("LabelOffsetX2");
            this.Property(t => t.LabelOffsetY2).HasColumnName("LabelOffsetY2");
            this.Property(t => t.LabelWidth2).HasColumnName("LabelWidth2");
            this.Property(t => t.LabelHeight2).HasColumnName("LabelHeight2");
            this.Property(t => t.LabelWidthEdgeToEdge2).HasColumnName("LabelWidthEdgeToEdge2");
            this.Property(t => t.LabelHeightEdgeToEdge2).HasColumnName("LabelHeightEdgeToEdge2");
            this.Property(t => t.LabelsAcross2).HasColumnName("LabelsAcross2");
            this.Property(t => t.LabelsDown2).HasColumnName("LabelsDown2");
            this.Property(t => t.TopImage).HasColumnName("TopImage");
            this.Property(t => t.BottomImage).HasColumnName("BottomImage");
            this.Property(t => t.UseMultiDefinitionFile).HasColumnName("UseMultiDefinitionFile");
        }
    }
}
