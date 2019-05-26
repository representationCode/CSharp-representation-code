using System.Windows.Forms;
using ipsFormsDesign.Model;
using ipsFormsDesign.Services;
using sysAdminwp.ComplexDictionariesBaseControls;
using WebAPIConnector;

namespace sysAdminwp.PresenterComplexDictionaries.DefineComplexLayers
{
    /// <summary>
    /// Layer - define befaviour for BaseCreativeroleTypeControl
    /// </summary>
    class CreativeroleTypeLayer : IDictionaryActions<CreativeRoleType>
    {
        private readonly DictionaryService _dictService = DictionaryService.GetInstance();
        
        //initialize control
        public UserControl InitControl(ApiConnector connector, string url)
        {
            return new BaseCreativeroleTypeControl(connector, url);
        }

        //get dictionary by id
        public CreativeRoleType GetDictionary(int id)
        {
            return _dictService.GetCreativeRoleType(id);
        }

        //create new dictionary
        public CreativeRoleType Create(CreativeRoleType sors)
        {
            return _dictService.CreateCreativeRoleType(sors);
        }

        //update current dictionary
        public CreativeRoleType Update(CreativeRoleType sors)
        {
            return _dictService.UpdateCreativeRoleType(sors);
        }

        //delete dictionary by id
        public bool Delete(int id)
        {
            return _dictService.DeleteCreativeRoleType(id);
        }
    }
}
