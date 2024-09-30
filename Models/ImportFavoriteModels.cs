using MSRecordsEngine.Models.FusionModels;
using Smead.Security;

namespace MSRecordsEngine.Models
{

    public class ImportFavoriteDDLChangeParam
    {
        public ImportFavoriteReqModel importFavoriteReqModel { get; set; }
        public Passport Passport { get; set; }
    }

    public class StartInsertDataToFavoriteParam
    {
        public ImportFavoriteModel m { get; set; }
        public Passport Passport { get; set; }
        //public string table { get; set; }
        //public string pkey { get; set; }
        //public string ConnectionString { get; set; }
        //public int UserId { get; set; }
        public string SessionDelmiter { get; set; }
    }

    public class StartInsertDataToFavorite_Response
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
        public string ErrorType { get; set; }
    }
}
