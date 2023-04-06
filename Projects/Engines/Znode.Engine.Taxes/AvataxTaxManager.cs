using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Znode.Engine.Taxes
{
    class AvataxTaxManager
    {
    }

    [XmlRootAttribute("AvataxSettings", Namespace = "", IsNullable = false)]
    public class AvataxSettings
    {
        
        #region Constructor

        #endregion

        #region Properties

        public string AvalaraAccount { get; set; }
        public string AvalaraCompanyCode { get; set; }
        public string AvalaraLicense { get; set; }
        public string AvalaraServiceURL { get; set; }
        public bool AvataxIsTaxIncluded { get; set; }
        public string AvalaraFreightIdentifier { get; set; }


        #endregion
    } //end class AvataxSettings

    [XmlRootAttribute("AvataxClassIdentifierCollection", Namespace = "", IsNullable = false)]
    public class AvataxClassIdentifierCollection : IList<AvataxClassIdentifier>
    {
        #region private members
        private readonly List<AvataxClassIdentifier> _innerList;
        #endregion

        #region constructor
        public AvataxClassIdentifierCollection()
        {
            _innerList = new List<AvataxClassIdentifier>();
        }
        #endregion

        #region properties
        public List<AvataxClassIdentifier> InnerList
        {
            get { return _innerList; }
        }
        #endregion

        #region helper methods

        public void Add(AvataxClassIdentifier item)
        {
            if (item == null)
                throw new ArgumentException("Identifier cannot be nothing.");
            if (this.Contains(item))
                throw new ArgumentException("Identifier has already been added to the collection.");
            _innerList.Add(item);
        }
        public bool Remove(AvataxClassIdentifier item)
        {
            AvataxClassIdentifier i = this.FirstOrDefault(c => c.ClassIdentifier == item.ClassIdentifier);
            if (i == null)
                throw new ArgumentException("Item does not exist in collection.");
            return _innerList.Remove(item);
        }
        public void RemoveAt(int index)
        {
            _innerList.RemoveAt(index);
        }
        public bool Contains(AvataxClassIdentifier item)
        {
            return _innerList.FirstOrDefault(i => i.ClassIdentifier == item.ClassIdentifier) != null;
        }
        public int IndexOf(AvataxClassIdentifier item)
        {
            return _innerList.TakeWhile(id => item.ClassIdentifier != id.ClassIdentifier).Count();
        }
        public void Insert(int index, AvataxClassIdentifier item)
        {
            if (item == null)
                throw new ArgumentException("Identifier cannot be nothing.");
            if (this.Contains(item))
                throw new ArgumentException("Identifier has already been added to the collection.");
            _innerList.Insert(index, item);
        }
        public void Clear()
        {
            _innerList.Clear();
        }
        public int Count
        {
            get { return _innerList.Count; }
        }
        public void CopyTo(AvataxClassIdentifier[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public System.Collections.Generic.IEnumerator<AvataxClassIdentifier> GetEnumerator()
        {
            foreach (AvataxClassIdentifier i in _innerList)
                yield return i;
        }
        public AvataxClassIdentifier this[int index]
        {
            get { return _innerList[index]; }
            set
            {
                if (!this.Contains(value))
                {
                    AvataxClassIdentifier i = (AvataxClassIdentifier)value;
                    _innerList[index] = i;
                }
            }
        }

        #endregion
    }

    [XmlRootAttribute("AvataxClassIdentifier", Namespace = "", IsNullable = false)]
    public class AvataxClassIdentifier
    {        

        #region Properties

        public string ClassIdentifier { get; set; }
        public string ClassName { get; set; }

        #endregion
    }//end class AvataxClassIdentifier
}
