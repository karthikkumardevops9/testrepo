using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class ReportStyleMap : EntityTypeConfiguration<ReportStyle>
    {
        public ReportStyleMap()
        {
            // Primary Key
            this.HasKey(t => t.ReportStylesId);

            // Properties
            this.Property(t => t.Id)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("ReportStyles");
            this.Property(t => t.ReportStylesId).HasColumnName("ReportStylesId");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.AltRowShading).HasColumnName("AltRowShading");
            this.Property(t => t.FixedLines).HasColumnName("FixedLines");
            this.Property(t => t.BlankLineSpacing).HasColumnName("BlankLineSpacing");
            this.Property(t => t.BoxWidth).HasColumnName("BoxWidth");
            this.Property(t => t.ColumnSpacing).HasColumnName("ColumnSpacing");
            this.Property(t => t.HeaderSize).HasColumnName("HeaderSize");
            this.Property(t => t.MaxLines).HasColumnName("MaxLines");
            this.Property(t => t.MinColumnWidth).HasColumnName("MinColumnWidth");
            this.Property(t => t.Orientation).HasColumnName("Orientation");
            this.Property(t => t.ShadowSize).HasColumnName("ShadowSize");
            this.Property(t => t.LineColor).HasColumnName("LineColor");
            this.Property(t => t.ShadeBoxColor).HasColumnName("ShadeBoxColor");
            this.Property(t => t.ShadedLineColor).HasColumnName("ShadedLineColor");
            this.Property(t => t.ShadowColor).HasColumnName("ShadowColor");
            this.Property(t => t.TextBackColor).HasColumnName("TextBackColor");
            this.Property(t => t.TextForeColor).HasColumnName("TextForeColor");
            this.Property(t => t.TopMargin).HasColumnName("TopMargin");
            this.Property(t => t.BottomMargin).HasColumnName("BottomMargin");
            this.Property(t => t.LeftMargin).HasColumnName("LeftMargin");
            this.Property(t => t.RightMargin).HasColumnName("RightMargin");
            this.Property(t => t.Heading1).HasColumnName("Heading1");
            this.Property(t => t.Heading2).HasColumnName("Heading2");
            this.Property(t => t.ColumnFontName).HasColumnName("ColumnFontName");
            this.Property(t => t.ColumnFontSize).HasColumnName("ColumnFontSize");
            this.Property(t => t.ColumnFontBold).HasColumnName("ColumnFontBold");
            this.Property(t => t.ColumnFontItalic).HasColumnName("ColumnFontItalic");
            this.Property(t => t.ColumnFontUnderlined).HasColumnName("ColumnFontUnderlined");
            this.Property(t => t.ColumnHeadingFontName).HasColumnName("ColumnHeadingFontName");
            this.Property(t => t.ColumnHeadingFontSize).HasColumnName("ColumnHeadingFontSize");
            this.Property(t => t.ColumnHeadingFontBold).HasColumnName("ColumnHeadingFontBold");
            this.Property(t => t.ColumnHeadingFontItalic).HasColumnName("ColumnHeadingFontItalic");
            this.Property(t => t.ColumnHeadingFontUnderlined).HasColumnName("ColumnHeadingFontUnderlined");
            this.Property(t => t.FooterFontName).HasColumnName("FooterFontName");
            this.Property(t => t.FooterFontSize).HasColumnName("FooterFontSize");
            this.Property(t => t.FooterFontBold).HasColumnName("FooterFontBold");
            this.Property(t => t.FooterFontItalic).HasColumnName("FooterFontItalic");
            this.Property(t => t.FooterFontUnderlined).HasColumnName("FooterFontUnderlined");
            this.Property(t => t.HeadingL1FontName).HasColumnName("HeadingL1FontName");
            this.Property(t => t.HeadingL1FontSize).HasColumnName("HeadingL1FontSize");
            this.Property(t => t.HeadingL1FontBold).HasColumnName("HeadingL1FontBold");
            this.Property(t => t.HeadingL1FontItalic).HasColumnName("HeadingL1FontItalic");
            this.Property(t => t.HeadingL1FontUnderlined).HasColumnName("HeadingL1FontUnderlined");
            this.Property(t => t.HeadingL2FontName).HasColumnName("HeadingL2FontName");
            this.Property(t => t.HeadingL2FontSize).HasColumnName("HeadingL2FontSize");
            this.Property(t => t.HeadingL2FontBold).HasColumnName("HeadingL2FontBold");
            this.Property(t => t.HeadingL2FontItalic).HasColumnName("HeadingL2FontItalic");
            this.Property(t => t.HeadingL2FontUnderlined).HasColumnName("HeadingL2FontUnderlined");
            this.Property(t => t.SubHeadingFontName).HasColumnName("SubHeadingFontName");
            this.Property(t => t.SubHeadingFontSize).HasColumnName("SubHeadingFontSize");
            this.Property(t => t.SubHeadingFontBold).HasColumnName("SubHeadingFontBold");
            this.Property(t => t.SubHeadingFontItalic).HasColumnName("SubHeadingFontItalic");
            this.Property(t => t.SubHeadingFontUnderlined).HasColumnName("SubHeadingFontUnderlined");
            this.Property(t => t.Heading1Left).HasColumnName("Heading1Left");
            this.Property(t => t.Heading1Center).HasColumnName("Heading1Center");
            this.Property(t => t.Heading1Right).HasColumnName("Heading1Right");
            this.Property(t => t.Heading2Center).HasColumnName("Heading2Center");
            this.Property(t => t.FooterLeft).HasColumnName("FooterLeft");
            this.Property(t => t.FooterCenter).HasColumnName("FooterCenter");
            this.Property(t => t.FooterRight).HasColumnName("FooterRight");
            this.Property(t => t.ReportCentered).HasColumnName("ReportCentered");
        }
    }
}
