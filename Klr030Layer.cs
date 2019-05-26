using ipsFormsDesign.Model;
using ipsFormsDesign.Services;
using sysAdminwp.ComplexDictionariesBaseControls;
using System.Windows.Forms;
using WebAPIConnector;

namespace sysAdminwp.PresenterComplexDictionaries.DefineComplexLayers
{
    /// <summary>
    /// Layer - define behaviour for BaseKlr030Control
    /// </summary>
    class Klr030Layer : IDictionaryActions<KlR030>
    {
        private readonly DictionaryService _dictService = DictionaryService.GetInstance();

        //initialize control
        public UserControl InitControl(ApiConnector connector, string url)
        {
            return new BaseKlr030Control(connector, url);
        }

        //get dictionary by id
        public KlR030 GetDictionary(int id)
        {
            return _dictService.GetKlR030(id);
        }

        //create new dictionary
        public KlR030 Create(KlR030 sors)
        {
            return _dictService.CreateKlR030(sors);
        }

        //update current dictionary
        public KlR030 Update(KlR030 sors)
        {
            return _dictService.UpdateKlR030(sors);
        }

        //delete dictionary by id
        public bool Delete(int id)
        {
            return _dictService.DeleteKlR030(id);
        }
    }
}
