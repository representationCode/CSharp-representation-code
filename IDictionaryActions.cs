using System.Windows.Forms;
using WebAPIConnector;

namespace sysAdminwp.PresenterComplexDictionaries
{
    /// <summary>
    /// define actions for complex dictionaries
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IDictionaryActions<T>
    {
        /// <summary>
        /// get dictionary by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetDictionary(int id);
        
        /// <summary>
        /// create dictionary
        /// </summary>
        /// <param name="sors"></param>
        /// <returns></returns>
        T Create(T sors);
        
        /// <summary>
        /// update dictionary
        /// </summary>
        /// <param name="sors"></param>
        /// <returns></returns>
        T Update(T sors);
        
        /// <summary>
        /// delete dictionary
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        bool Delete(int code);

        /// <summary>
        /// initialize control
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        UserControl InitControl(ApiConnector connector, string url);
    }
}
