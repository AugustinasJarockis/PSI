namespace NOTEA.Models
{
    public interface IDataService
    {
        public void SaveConspects(ConspectModel conspect);
        public ConspectListModel LoadConspects(string fileName);
    }
}
