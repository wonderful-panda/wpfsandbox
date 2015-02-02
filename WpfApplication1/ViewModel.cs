using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace WpfApplication1
{
    public class ViewModel
    {
        public ObservableCollection<Item> Items { get; private set; }
        public ViewModel()
        {
            this.Items = new ObservableCollection<Item>(
                Enumerable.Range(0, 50).Select(i => new Item("Item-" + i.ToString(), "Value-" + i.ToString()))
                );
        }
    }

    public class Item : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName]string name = "")
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;
                _name = value;
                NotifyPropertyChanged();
            }
        }

        string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;
                _value = value;
                NotifyPropertyChanged();
            }
        }
        
        public Item(string name, string value)
        {
            _name = name;
            _value = value;
        }
    }
}
