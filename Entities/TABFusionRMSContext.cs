using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Entities.Mapping;

namespace MSRecordsEngine.Entities
{
    public partial class TABFusionRMSContext : DbContext
    {
        static TABFusionRMSContext()
        {
            Database.SetInitializer<TABFusionRMSContext>(null);
        }

        public TABFusionRMSContext()
            : base("Name=TABFusionRMSContext")
        {
        }

		public TABFusionRMSContext(string sConnectionString)
            : base(sConnectionString)
        {
        }

        public DbSet<AddInReport> AddInReports { get; set; }
        public DbSet<Annotation> Annotations { get; set; }
        public DbSet<ARINV> ARINVs { get; set; }
        public DbSet<ARPO> ARPOes { get; set; }
        public DbSet<AssetStatu> AssetStatus { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<COMMLabelLine> COMMLabelLines { get; set; }
        public DbSet<COMMLabel> COMMLabels { get; set; }
        public DbSet<Correspondence> Correspondences { get; set; }
        public DbSet<Databas> Databases { get; set; }
        public DbSet<DBVersion> DBVersions { get; set; }
        public DbSet<Directory> Directories { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<GridColumn> GridColumns { get; set; }
        public DbSet<GridSetting> GridSettings { get; set; }
        public DbSet<ImagePointer> ImagePointers { get; set; }
        public DbSet<ImageTablesList> ImageTablesLists { get; set; }
        public DbSet<ImportField> ImportFields { get; set; }
        public DbSet<ImportJob> ImportJobs { get; set; }
        public DbSet<ImportLoad> ImportLoads { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<LinkScript> LinkScripts { get; set; }
        public DbSet<LinkScriptFeature> LinkScriptFeatures { get; set; }
        public DbSet<LinkScriptHeader> LinkScriptHeaders { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<LookupType> LookupTypes { get; set; }
        public DbSet<MobileDetail> MobileDetails { get; set; }
        public DbSet<OfficeDocType> OfficeDocTypes { get; set; }
        public DbSet<OneStripForm> OneStripForms { get; set; }
        public DbSet<OneStripJobField> OneStripJobFields { get; set; }
        public DbSet<OneStripJob> OneStripJobs { get; set; }
        public DbSet<OutputSetting> OutputSettings { get; set; }
        public DbSet<PackingSlip> PackingSlips { get; set; }
        public DbSet<PCFilesPointer> PCFilesPointers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<RecordType> RecordTypes { get; set; }
        public DbSet<RelationShip> RelationShips { get; set; }
        public DbSet<ReportStyle> ReportStyles { get; set; }
        public DbSet<Requisition> Requisitions { get; set; }
        public DbSet<s_SavedChildrenFavorite> s_SavedChildrenFavorite { get; set; }
        public DbSet<s_SavedChildrenQuery> s_SavedChildrenQuery { get; set; }
        public DbSet<s_SavedCriteria> s_SavedCriteria { get; set; }
        public DbSet<ScanBatch> ScanBatches { get; set; }
        public DbSet<ScanList> ScanLists { get; set; }
        public DbSet<ScanRule> ScanRules { get; set; }
        public DbSet<SecureGroup> SecureGroups { get; set; }
        public DbSet<SecureObject> SecureObjects { get; set; }
        public DbSet<SecureObjectPermission> SecureObjectPermissions { get; set; }
        public DbSet<SecurePermission> SecurePermissions { get; set; }
        public DbSet<SecurePermissionDescription> SecurePermissionDescriptions { get; set; }
        public DbSet<SecureUser> SecureUsers { get; set; }
        public DbSet<SecureUserGroup> SecureUserGroups { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<SLAuditConfData> SLAuditConfDatas { get; set; }
        public DbSet<SLAuditFailedLogin> SLAuditFailedLogins { get; set; }
        public DbSet<SLAuditLogin> SLAuditLogins { get; set; }
        public DbSet<SLAuditUpdate> SLAuditUpdates { get; set; }
        public DbSet<SLAuditUpdChildren> SLAuditUpdChildrens { get; set; }
        public DbSet<SLBatchRequest> SLBatchRequests { get; set; }
        public DbSet<SLColdArchive> SLColdArchives { get; set; }
        public DbSet<SLColdPointer> SLColdPointers { get; set; }
        public DbSet<SLColdSetupCol> SLColdSetupCols { get; set; }
        public DbSet<SLColdSetupForm> SLColdSetupForms { get; set; }
        public DbSet<SLColdSetupRow> SLColdSetupRows { get; set; }
        public DbSet<SLCollectionItem> SLCollectionItems { get; set; }
        public DbSet<SLCollection> SLCollections { get; set; }
        public DbSet<SLDestructCertItem> SLDestructCertItems { get; set; }
        public DbSet<SLDestructionCert> SLDestructionCerts { get; set; }
        public DbSet<SLGrabberControl> SLGrabberControls { get; set; }
        public DbSet<SLGrabberField> SLGrabberFields { get; set; }
        public DbSet<SLGrabberFldPart> SLGrabberFldParts { get; set; }
        public DbSet<SLGrabberFunction> SLGrabberFunctions { get; set; }
        public DbSet<SLIndexer> SLIndexers { get; set; }
        public DbSet<SLIndexerCache> SLIndexerCaches { get; set; }
        public DbSet<SLIndexWizard> SLIndexWizards { get; set; }
        public DbSet<SLIndexWizardCol> SLIndexWizardCols { get; set; }
        public DbSet<SLPullList> SLPullLists { get; set; }
        public DbSet<SLRequestor> SLRequestors { get; set; }
        public DbSet<SLRetentionCitaCode> SLRetentionCitaCodes { get; set; }
        public DbSet<SLRetentionCitation> SLRetentionCitations { get; set; }
        public DbSet<SLRetentionCode> SLRetentionCodes { get; set; }
        public DbSet<SLRetentionInactive> SLRetentionInactives { get; set; }
        public DbSet<SLServiceTask> SLServiceTasks { get; set; }
        public DbSet<SLServiceTaskItem> SLServiceTaskItem { get; set; }
        public DbSet<SLSignature> SLSignatures { get; set; }
        public DbSet<SLTableFileRoomOrder> SLTableFileRoomOrders { get; set; }
        public DbSet<SLTextSearchItem> SLTextSearchItems { get; set; }
        public DbSet<SLTrackingSelectData> SLTrackingSelectDatas { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }
        public DbSet<SysNextTrackable> SysNextTrackables { get; set; }
        public DbSet<System> Systems { get; set; }
        public DbSet<SystemAddress> SystemAddresses { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<TableTab> TableTabs { get; set; }
        public DbSet<TabSet> TabSets { get; set; }
        public DbSet<Trackable> Trackables { get; set; }
        public DbSet<TrackingHistory> TrackingHistories { get; set; }
        public DbSet<TrackingStatu> TrackingStatus { get; set; }
        public DbSet<Userlink> Userlinks { get; set; }
        public DbSet<ViewColumn> ViewColumns { get; set; }
        public DbSet<ViewFilter> ViewFilters { get; set; }
        public DbSet<View> Views { get; set; }
        public DbSet<Volume> Volumes { get; set; }
        public DbSet<vwColumnsAll> vwColumnsAlls { get; set; }
        public DbSet<vwGetOutputSetting> vwGetOutputSettings { get; set; }
        public DbSet<vwGridSetting> vwGridSettings { get; set; }
        public DbSet<vwTablesAll> vwTablesAlls { get; set; }
        public DbSet<SLUserDashboard> UserDashboards { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AddInReportMap());
            modelBuilder.Configurations.Add(new AnnotationMap());
            modelBuilder.Configurations.Add(new ARINVMap());
            modelBuilder.Configurations.Add(new ARPOMap());
            modelBuilder.Configurations.Add(new AssetStatuMap());
            modelBuilder.Configurations.Add(new AttributeMap());
            modelBuilder.Configurations.Add(new COMMLabelLineMap());
            modelBuilder.Configurations.Add(new COMMLabelMap());
            modelBuilder.Configurations.Add(new CorrespondenceMap());
            modelBuilder.Configurations.Add(new DatabasMap());
            modelBuilder.Configurations.Add(new DBVersionMap());
            modelBuilder.Configurations.Add(new DirectoryMap());
            modelBuilder.Configurations.Add(new DocumentMap());
            modelBuilder.Configurations.Add(new DocumentTypeMap());
            modelBuilder.Configurations.Add(new GridColumnMap());
            modelBuilder.Configurations.Add(new GridSettingMap());
            modelBuilder.Configurations.Add(new ImagePointerMap());
            modelBuilder.Configurations.Add(new ImageTablesListMap());
            modelBuilder.Configurations.Add(new ImportFieldMap());
            modelBuilder.Configurations.Add(new ImportJobMap());
            modelBuilder.Configurations.Add(new ImportLoadMap());
            modelBuilder.Configurations.Add(new InvoiceMap());
            modelBuilder.Configurations.Add(new LinkScriptMap());
            modelBuilder.Configurations.Add(new LinkScriptFeatureMap());
            modelBuilder.Configurations.Add(new LinkScriptHeaderMap());
            modelBuilder.Configurations.Add(new LocationMap());
            modelBuilder.Configurations.Add(new LookupTypeMap());
            modelBuilder.Configurations.Add(new MobileDetailMap());
            modelBuilder.Configurations.Add(new OfficeDocTypeMap());
            modelBuilder.Configurations.Add(new OneStripFormMap());
            modelBuilder.Configurations.Add(new OneStripJobFieldMap());
            modelBuilder.Configurations.Add(new OneStripJobMap());
            modelBuilder.Configurations.Add(new OutputSettingMap());
            modelBuilder.Configurations.Add(new PackingSlipMap());
            modelBuilder.Configurations.Add(new PCFilesPointerMap());
            modelBuilder.Configurations.Add(new PurchaseOrderMap());
            modelBuilder.Configurations.Add(new RecordTypeMap());
            modelBuilder.Configurations.Add(new RelationShipMap());
            modelBuilder.Configurations.Add(new ReportStyleMap());
            modelBuilder.Configurations.Add(new RequisitionMap());
            modelBuilder.Configurations.Add(new s_SavedChildrenFavoriteMap());
            modelBuilder.Configurations.Add(new s_SavedChildrenQueryMap());
            modelBuilder.Configurations.Add(new s_SavedCriteriaMap());
            modelBuilder.Configurations.Add(new ScanBatchMap());
            modelBuilder.Configurations.Add(new ScanListMap());
            modelBuilder.Configurations.Add(new ScanRuleMap());
            modelBuilder.Configurations.Add(new SecureGroupMap());
            modelBuilder.Configurations.Add(new SecureObjectMap());
            modelBuilder.Configurations.Add(new SecureObjectPermissionMap());
            modelBuilder.Configurations.Add(new SecurePermissionMap());
            modelBuilder.Configurations.Add(new SecurePermissionDescriptionMap());
            modelBuilder.Configurations.Add(new SecureUserMap());
            modelBuilder.Configurations.Add(new SecureUserGroupMap());
            modelBuilder.Configurations.Add(new SettingMap());
            modelBuilder.Configurations.Add(new SLAuditConfDataMap());
            modelBuilder.Configurations.Add(new SLAuditFailedLoginMap());
            modelBuilder.Configurations.Add(new SLAuditLoginMap());
            modelBuilder.Configurations.Add(new SLAuditUpdateMap());
            modelBuilder.Configurations.Add(new SLAuditUpdChildrenMap());
            modelBuilder.Configurations.Add(new SLBatchRequestMap());
            modelBuilder.Configurations.Add(new SLColdArchiveMap());
            modelBuilder.Configurations.Add(new SLColdPointerMap());
            modelBuilder.Configurations.Add(new SLColdSetupColMap());
            modelBuilder.Configurations.Add(new SLColdSetupFormMap());
            modelBuilder.Configurations.Add(new SLColdSetupRowMap());
            modelBuilder.Configurations.Add(new SLCollectionItemMap());
            modelBuilder.Configurations.Add(new SLCollectionMap());
            modelBuilder.Configurations.Add(new SLDestructCertItemMap());
            modelBuilder.Configurations.Add(new SLDestructionCertMap());
            modelBuilder.Configurations.Add(new SLGrabberControlMap());
            modelBuilder.Configurations.Add(new SLGrabberFieldMap());
            modelBuilder.Configurations.Add(new SLGrabberFldPartMap());
            modelBuilder.Configurations.Add(new SLGrabberFunctionMap());
            modelBuilder.Configurations.Add(new SLIndexerMap());
            modelBuilder.Configurations.Add(new SLIndexerCacheMap());
            modelBuilder.Configurations.Add(new SLIndexWizardMap());
            modelBuilder.Configurations.Add(new SLIndexWizardColMap());
            modelBuilder.Configurations.Add(new SLPullListMap());
            modelBuilder.Configurations.Add(new SLRequestorMap());
            modelBuilder.Configurations.Add(new SLRetentionCitaCodeMap());
            modelBuilder.Configurations.Add(new SLRetentionCitationMap());
            modelBuilder.Configurations.Add(new SLRetentionCodeMap());
            modelBuilder.Configurations.Add(new SLRetentionInactiveMap());
            modelBuilder.Configurations.Add(new SLServiceTaskMap());
            modelBuilder.Configurations.Add(new SLServiceTaskItemMap());
            modelBuilder.Configurations.Add(new SLSignatureMap());
            modelBuilder.Configurations.Add(new SLTableFileRoomOrderMap());
            modelBuilder.Configurations.Add(new SLTextSearchItemMap());
            modelBuilder.Configurations.Add(new SLTrackingSelectDataMap());
            modelBuilder.Configurations.Add(new StatusHistoryMap());
            modelBuilder.Configurations.Add(new SysNextTrackableMap());
            modelBuilder.Configurations.Add(new SystemMap());
            modelBuilder.Configurations.Add(new SystemAddressMap());
            modelBuilder.Configurations.Add(new TableMap());
            modelBuilder.Configurations.Add(new TableTabMap());
            modelBuilder.Configurations.Add(new TabSetMap());
            modelBuilder.Configurations.Add(new TrackableMap());
            modelBuilder.Configurations.Add(new TrackingHistoryMap());
            modelBuilder.Configurations.Add(new TrackingStatuMap());
            modelBuilder.Configurations.Add(new UserlinkMap());
            modelBuilder.Configurations.Add(new ViewColumnMap());
            modelBuilder.Configurations.Add(new ViewFilterMap());
            modelBuilder.Configurations.Add(new ViewMap());
            modelBuilder.Configurations.Add(new VolumeMap());
            modelBuilder.Configurations.Add(new vwColumnsAllMap());
            modelBuilder.Configurations.Add(new vwGetOutputSettingMap());
            modelBuilder.Configurations.Add(new vwGridSettingMap());
            modelBuilder.Configurations.Add(new vwTablesAllMap());
            modelBuilder.Configurations.Add(new SLUserDashboardMap());
        }
    }
}
