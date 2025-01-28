/*
 * Created by: Peter Weissbrod
 * Created: Tuesday, January 22, 2008
 */
using System.Collections.ObjectModel;
using BloggerApp.Models;

namespace BloggerApp.DAO
{
    /// <summary>
    /// DAO object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDAO<T> where T : BusinessObject
    {
        ObservableCollection<T> GetAll();
        void Save(T model);
        T GetByID(int id);
    }
}